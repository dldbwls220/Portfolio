using UnityEngine;
using UnityEngine.Tilemaps;

public class CheckBoardTileMap : MonoBehaviour
{
    static CheckBoardTileMap _uniqueInstance;

    [SerializeField] Tilemap targetTilemap;
    [SerializeField] Tilemap feverTilemap;
    [SerializeField] Tile tileA;
    [SerializeField] Tile tileB;
    [SerializeField] Tile feverTileA;
    [SerializeField] Tile feverTileB;
    [SerializeField] GameObject _target;
    [SerializeField] GameObject _fever;


    Tile changeTileA;
    Tile changeTileB;

    Tile changeFeverA;
    Tile changeFeverB;

    bool isChange;
    [SerializeField] bool isFever;
    public Vector2Int gridSize = new Vector2Int(10, 10);

    public Vector2 shopGridSize = new Vector2Int(10, 10);

    public static CheckBoardTileMap _instance {  get { return _uniqueInstance; } }

    void Awake()
    {
        _uniqueInstance = this;
    }

    void Start()
    {
        GenerateCheckerboard();
        GenerateFeverboard();
        CreateShopTile();
    }

    private void Update()
    {
        if (IngameManager._instance != null)
        {
            if (IngameManager._instance._isCombo)
            {
                _target.SetActive(false);
                _fever.SetActive(true);
            }
            else
            {
                _target.SetActive(true);
                _fever.SetActive(false);
            }
        }    
    }

    void GenerateCheckerboard()
    {
        if (targetTilemap == null || tileA == null || tileB == null)
        {
            Debug.LogError("Assign Tilemap and Tile assets in the Inspector!");
            return;
        }

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                // Determine which tile to place based on the sum of coordinates
                // (x + y) % 2 will alternate between 0 and 1
                if ((x + y) % 2 == 0)
                {
                    targetTilemap.SetTile(position, tileA);
                }
                else
                {
                    targetTilemap.SetTile(position, tileB);
                }
            }
        }      

        isChange = true;
    }

    void GenerateFeverboard()
    {
        if (feverTilemap == null || feverTileA == null || feverTileB == null)
        {
            Debug.LogError("Assign Tilemap and Tile assets in the Inspector!");
            return;
        }

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                // Determine which tile to place based on the sum of coordinates
                // (x + y) % 2 will alternate between 0 and 1
                if ((x + y) % 2 == 0)
                {
                    feverTilemap.SetTile(position, feverTileA);
                }
                else
                {
                    feverTilemap.SetTile(position, tileA);
                }
            }
        }
    }

    void CreateShopTile()
    {
        int offsetx = gridSize.x + 30;

        for (int x = offsetx; x < offsetx + shopGridSize.x; x++)
        {
            for (int y = 0; y < shopGridSize.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                // Determine which tile to place based on the sum of coordinates
                // (x + y) % 2 will alternate between 0 and 1
                if ((x + y) % 2 == 0)
                {
                    targetTilemap.SetTile(position, tileA);
                }
                else
                {
                    targetTilemap.SetTile(position, tileB);
                }
            }
        }

        for (int x = offsetx; x < offsetx + shopGridSize.x; x++)
        {
            for (int y = 0; y < shopGridSize.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                // Determine which tile to place based on the sum of coordinates
                // (x + y) % 2 will alternate between 0 and 1
                if ((x + y) % 2 == 0)
                {
                    feverTilemap.SetTile(position, feverTileA);
                }
                else
                {
                    feverTilemap.SetTile(position, tileA);
                }
            }
        }
    }

    public void ChangTile()
    {
        isChange = !isChange;

        if (isChange)
        {
            changeTileA = tileA;
            changeTileB = tileB;

            changeFeverA = feverTileA;
            changeFeverB = tileA;
        }
        else
        {
                changeTileA = tileB;
                changeTileB = tileA;

                changeFeverA = tileA;
                changeFeverB = feverTileB;
        }

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                // Determine which tile to place based on the sum of coordinates
                // (x + y) % 2 will alternate between 0 and 1

                if ((x + y) % 2 == 0)
                {
                    targetTilemap.SetTile(position, changeTileA);
                }
                else
                {
                    targetTilemap.SetTile(position, changeTileB);
                }
            }
        }

        if (feverTileA != null)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);

                    // Determine which tile to place based on the sum of coordinates
                    // (x + y) % 2 will alternate between 0 and 1

                    if ((x + y) % 2 == 0)
                    {
                        feverTilemap.SetTile(position, changeFeverA);
                    }
                    else
                    {
                        feverTilemap.SetTile(position, changeFeverB);
                    }
                }
            }
        }

        int offsetx = gridSize.x + 30;

        for (int x = offsetx; x < offsetx + shopGridSize.x; x++)
        {
            for (int y = 0; y < shopGridSize.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                // Determine which tile to place based on the sum of coordinates
                // (x + y) % 2 will alternate between 0 and 1
                if ((x + y) % 2 == 0)
                {
                    targetTilemap.SetTile(position, changeTileA);
                }
                else
                {
                    targetTilemap.SetTile(position, changeTileB);
                }
            }
        }

        for (int x = offsetx; x < offsetx + shopGridSize.x; x++)
        {
            for (int y = 0; y < shopGridSize.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                // Determine which tile to place based on the sum of coordinates
                // (x + y) % 2 will alternate between 0 and 1
                if ((x + y) % 2 == 0)
                {
                    feverTilemap.SetTile(position, changeFeverA);
                }
                else
                {
                    feverTilemap.SetTile(position, changeFeverB);
                }
            }
        }
    }
}
