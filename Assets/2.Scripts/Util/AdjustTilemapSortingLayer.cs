using DefineEnum;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AdjustTilemapSortingLayer : MonoBehaviour
{
    TilemapRenderer _tRenderer;
    [SerializeField] float _pixel = 26;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tRenderer = GetComponent<TilemapRenderer>();
        _tRenderer.sortingOrder = (int)(transform.position.y * _pixel);
    }
}
