using UnityEngine;
using UnityEngine.Tilemaps;

public class CheckBoardTileMap : MonoBehaviour
{
    [SerializeField] Tilemap targetTilemap;
    [SerializeField] Tile tileA;
    [SerializeField] Tile tileB;
    [SerializeField] Tile feverTileA;
    [SerializeField] Tile feverTileB;

    Tile changeTileA;
    Tile changeTileB;
    bool isChange;
    [SerializeField] bool isFever;
    public Vector2Int gridSize = new Vector2Int(10, 10);

    public Vector2 shopGridSize = new Vector2Int(10, 10);

    void Start()
    {
        NoteManager._instance.OnBeat += OnBeat;
        GenerateCheckerboard();
        CreateShopTile();
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
    }

    void OnBeat()
    {
        isChange = !isChange;

        if (isChange)
        {
            if (!isFever)
            {
                changeTileA = tileA;
                changeTileB = tileB;
            }
            else
            {
                changeTileA = feverTileA;
                changeTileB = tileA;
            }
        }
        else
        {
            if (!isFever)
            {
                changeTileA = tileB;
                changeTileB = tileA;
            }
            else
            {
                changeTileA = tileA;
                changeTileB = feverTileB;
            }
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
    }
}
