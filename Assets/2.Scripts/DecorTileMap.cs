using UnityEngine;
using UnityEngine.Tilemaps;

public class DecorTileMap : MonoBehaviour
{
    [SerializeField] Tilemap _wallTilemap;
    [SerializeField] Tilemap _decorTilemap;
    [SerializeField] Tile[] _decorTiles;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GeneerateDecorTile();
    }

   
    void GeneerateDecorTile()
    {
        BoundsInt bound = _wallTilemap.cellBounds;

        for (int x = bound.xMin; x < bound.xMax; x++)
        {
            for (int y = bound.yMin; y < bound.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                for (int i = 0; i < _decorTiles.Length; i++)
                {
                    Sprite sprite = _wallTilemap.GetSprite(pos);

                    if (sprite != null && sprite.name == "Walls_" + i)
                    {
                        Vector3Int decorPos = new Vector3Int(x, y + 1, 0);
                        _decorTilemap.SetTile(decorPos, _decorTiles[i]);
                    }                                  
                }
            }
        }

    }
}
