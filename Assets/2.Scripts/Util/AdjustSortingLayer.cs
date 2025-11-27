using DefineEnum;
using UnityEngine;
using UnityEngine.UIElements;

public class AdjustSortingLayer : MonoBehaviour
{
    SpriteRenderer _sRenderer;
    [SerializeField] float _pixel = 26;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        _sRenderer = GetComponent<SpriteRenderer>();
        _sRenderer.sortingOrder = (int)(transform.position.y * -_pixel);
    }
}
