using UnityEngine;
using UnityEngine.UI;
using DefineEnum;

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
        {
            GetItem();
            SoundManager._instance.PlaySFX(SFXName.sfx_pickup_purchase);
        }
        else
            SoundManager._instance.PlaySFX(SFXName.sfx_error_ST);

    }

    protected abstract void GetItem();
}
