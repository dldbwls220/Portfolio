using UnityEngine;
using DefineEnum;

public class WeaponObj : ItemBase
{
    [SerializeField] WeaponName _weapon;

    private void OnEnable()
    {
        _priceTxt.text = _price.ToString() + " G";
    }

    protected override void GetItem()
    {
        _playerController.BuyWeapon(_price, _weapon);
    }
}
