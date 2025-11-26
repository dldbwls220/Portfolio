using UnityEngine;
using DefineEnum;

public class FoodObj : ItemBase
{
    [SerializeField] int heal;

    private void OnEnable()
    {
        _priceTxt.text = _price.ToString() + " G";
    }

    protected override void GetItem()
    {
        _playerController.BuyFood(_price, heal);
        gameObject.SetActive(false);
    }
}
