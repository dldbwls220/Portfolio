using UnityEngine;
using UnityEngine.UI;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField] protected int _price;
    [SerializeField] protected Text _priceTxt;
    protected PlayerController _playerController;

    private void Start()
    {
        _playerController = GameObject.Find("PlayerCharacter").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_playerController._goldContain >= _price)
            GetItem();

    }

    protected abstract void GetItem();
}
