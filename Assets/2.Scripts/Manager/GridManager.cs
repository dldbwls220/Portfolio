using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [System.Serializable]
    public class TileType
    {
        public LayerMask _tileType;
        public int _tilePenalty;
    }
    [SerializeField] TileType[] _walkableTiles;
    [SerializeField] LayerMask _unwalkableTiles;
    [SerializeField] Vector2 _tileWorldSize;
    [SerializeField] float _nodeRadius;

    [SerializeField] int _obstacleProximityPenalty = 10;

    Node[,] _grid;

    float _nodeDiameter;
    int _tileSizeX, _tileSizeY;

    int _panaltyMin = int.MaxValue;
    int _panaltyMax = int.MinValue;

    Dictionary<int, int> _walkableRegionsDictionary;
    LayerMask _walkableMask;

    List<Node> _path;

    public int _maxSize => _tileSizeX * _tileSizeY;

    public List<Node> _findPath
    {
        set => _path = value;
    }

    void Awake()
    {
        _nodeDiameter = _nodeRadius * 2;                                            // Ä­ ÇÏ³ªÀÇ Å©±â
        _tileSizeX = Mathf.RoundToInt(_tileWorldSize.x / _nodeDiameter);            // x Ä­ ¼ö
        _tileSizeY = Mathf.RoundToInt(_tileWorldSize.y / _nodeDiameter);            // y Ä­ ¼ö
        _walkableRegionsDictionary = new Dictionary<int, int>();
        foreach (TileType region in _walkableTiles)
        {
            _walkableMask.value |= region._tileType.value;
            _walkableRegionsDictionary.Add((int)Mathf.Log(region._tileType.value, 2), region._tilePenalty);
        }
    }

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        _grid = new Node[_tileSizeX, _tileSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * _tileWorldSize.x / 2 - Vector3.up * _tileWorldSize.y / 2;

        for (int x = 0; x < _tileSizeX; x++)
        {
            for (int y = 0; y < _tileSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + _nodeRadius) + Vector3.up * (y * _nodeDiameter + _nodeRadius);

                bool isWalkable = !(Physics2D.OverlapCircle(worldPoint, _nodeRadius, _unwalkableTiles));

                int movementCost = 0;
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector3.down, 1f, _walkableMask);

                if (hit.collider != null)
                {
                    _walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementCost);
                }
                if (!isWalkable)
                    movementCost += _obstacleProximityPenalty;

                _grid[x, y] = new Node(isWalkable, worldPoint, x, y, movementCost);
            }
        }
        BlurPenaltyMap(1);
    }

    void BlurPenaltyMap(int blueSize)
    {
        int kernelSize = blueSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] penaltiesHorizontalPass = new int[_tileSizeX, _tileSizeY];
        int[,] penaltiesVerticalPass = new int[_tileSizeX, _tileSizeY];

        for (int y = 0; y < _tileSizeY; y++)
        {
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltiesHorizontalPass[0, y] += _grid[sampleX, y]._movementCost;
            }
            for (int x = 1; x < _tileSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, _tileSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, _tileSizeX - 1);
                penaltiesHorizontalPass[x, y]
                    = penaltiesHorizontalPass[x - 1, y] - _grid[removeIndex, y]._movementCost + _grid[addIndex, y]._movementCost;
            }
        }
        for (int x = 0; x < _tileSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }
            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelExtents * kernelSize));
            _grid[x, 0]._movementCost = blurredPenalty;
            for (int y = 1; y < _tileSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, _tileSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, _tileSizeY - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];

                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                _grid[x, y]._movementCost = blurredPenalty;
                if (blurredPenalty > _panaltyMax)
                    _panaltyMax = blurredPenalty;
                if (blurredPenalty < _panaltyMin)
                    _panaltyMin = blurredPenalty;
            }
        }
    }
    //== Ãß°¡(end)


    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node._grideX + x;
                int checkY = node._grideY + y;
                if (checkX >= 0 && checkX < _tileSizeX && checkY >= 0 && checkY < _tileSizeY)
                {
                    neighbors.Add(_grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }


    public Node NodeFromWorldPosition(Vector3 position)
    {
        float percentX = (position.x + _tileWorldSize.x / 2) / _tileWorldSize.x;
        float percentY = (position.y + _tileWorldSize.y / 2) / _tileWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((_tileSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_tileSizeY - 1) * percentY);

        return _grid[x, y];
    }
}
