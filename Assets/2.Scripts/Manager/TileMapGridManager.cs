using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapGridManager : MonoBehaviour
{
    [System.Serializable]
    public class TilePenaltyData
    {
        public TileBase _tile;
        public int _penalty;
        public bool _isWalkable = true;
    }

    [Header("Tilemaps")]
    public Tilemap _terrainTilemap;
    public Tilemap _obstacleTilemap;

    [Header("Penalty Settings")]
    public List<TilePenaltyData> _tilePenaltyList;

    [Header("Grid Settings")]
    public float _nodeRadius = 0.5f;

    Node[,] _grid;

    int _gridSizeX;
    int _gridSizeY;

    float nodeDiameter;

    Dictionary<TileBase, TilePenaltyData> _tilePenaltyMap;

    int _penaltyMin = int.MaxValue;
    int _penaltyMax = int.MinValue;

    public int _gridSizeMax => _gridSizeX * _gridSizeY;

    List<Node> _debugPath;

    BoundsInt _bounds;

    [SerializeField] bool _DrawGizzmo;

    void Awake()
    {
        nodeDiameter = _nodeRadius * 2f;

        _tilePenaltyMap = new Dictionary<TileBase, TilePenaltyData>();
        foreach (var data in _tilePenaltyList)
        {
            if (data._tile != null && !_tilePenaltyMap.ContainsKey(data._tile))
                _tilePenaltyMap.Add(data._tile, data);
        }
    }

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        _bounds = _terrainTilemap.cellBounds;   // 타일맵 실제 타일 범위

        _gridSizeX = _bounds.size.x;
        _gridSizeY = _bounds.size.y;

        _grid = new Node[_gridSizeX, _gridSizeY];

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                // 타일맵 실제 셀 좌표
                Vector3Int cell = new Vector3Int(_bounds.xMin + x, _bounds.yMin + y, 0);

                // 타일의 월드 좌표(중앙)
                Vector3 worldPos = _terrainTilemap.CellToWorld(cell) + new Vector3(0.5f, 0.5f, 0f);

                TileBase terrainTile = _terrainTilemap.GetTile(cell);
                TileBase obstacleTile = _obstacleTilemap != null ? _obstacleTilemap.GetTile(cell) : null;

                bool walkable = true;
                int moveCost = 0;

                // 장애물이 있는 경우
                if (obstacleTile != null)
                {
                    walkable = false;
                    moveCost += 5; // 완전 막힘
                }

                // 일반 타일 패널티 적용
                if (terrainTile != null && _tilePenaltyMap.TryGetValue(terrainTile, out var data))
                {
                    moveCost += data._penalty;
                    if (!data._isWalkable)
                        walkable = false;
                }

                // 타일이 없는 경우 = 보통 벽
                if (terrainTile == null && obstacleTile == null)
                {
                    walkable = false;
                }

                _grid[x, y] = new Node(walkable, worldPos, x, y, moveCost);
            }
        }

        BlurPenaltyMap(1);
    }

    void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int extents = (kernelSize - 1) / 2;

        int[,] horizontal = new int[_gridSizeX, _gridSizeY];
        int[,] vertical = new int[_gridSizeX, _gridSizeY];

        // 가로 방향 블러
        for (int y = 0; y < _gridSizeY; y++)
        {
            for (int x = 0; x < _gridSizeX; x++)
            {
                int sum = 0;
                for (int i = -extents; i <= extents; i++)
                {
                    int sampleX = Mathf.Clamp(x + i, 0, _gridSizeX - 1);
                    sum += _grid[sampleX, y]._movementCost;
                }
                horizontal[x, y] = sum;
            }
        }

        // 세로 방향 블러
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                int sum = 0;
                for (int i = -extents; i <= extents; i++)
                {
                    int sampleY = Mathf.Clamp(y + i, 0, _gridSizeY - 1);
                    sum += horizontal[x, sampleY];
                }

                int blurred = Mathf.RoundToInt((float)sum / (kernelSize * kernelSize));
                _grid[x, y]._movementCost = blurred;

                _penaltyMin = Mathf.Min(_penaltyMin, blurred);
                _penaltyMin = Mathf.Max(_penaltyMin, blurred);
            }
        }
    }

    public Node NodeFromWorldPos(Vector3 worldPos)
    {
        Vector3Int cell = _terrainTilemap.WorldToCell(worldPos);

        int x = cell.x - _bounds.xMin;
        int y = cell.y - _bounds.yMin;

        x = Mathf.Clamp(x, 0, _gridSizeX - 1);
        y = Mathf.Clamp(y, 0, _gridSizeY - 1);

        return _grid[x, y];
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        int[,] dir = new int[,]
        {
            {1,0}, //오른쪽
            {-1,0},//왼쪽
            {0,1}, //위
            {0,-1} //아래
        };

        for (int i = 0; i < 4; i++)
        {
            int nx = node._grideX + dir[i, 0];
            int ny = node._grideY + dir[i, 1];

            if (nx >= 0 && nx < _gridSizeX && ny >= 0 && ny < _gridSizeY)
            {
                neighbors.Add(_grid[nx, ny]);
            }
        }

        return neighbors;
    }

    public void SetDebugPath(List<Node> path)
    {
        _debugPath = path;
    }

    void OnDrawGizmos()
    {
        if (_grid != null && _DrawGizzmo)
        {
            foreach (Node n in _grid)
            {
                Gizmos.color = n._walkable
                    ? Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(_penaltyMin, _penaltyMax, n._movementCost))
                    : Color.red;

                Gizmos.DrawCube(n._worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }

            if (_debugPath != null)
            {
                Gizmos.color = Color.yellow;
                foreach (var n in _debugPath)
                {
                    Gizmos.DrawCube(n._worldPosition, Vector3.one * (nodeDiameter - 0.12f));
                }
            }
        } 
    }
}
