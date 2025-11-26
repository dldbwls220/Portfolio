using UnityEngine;
using DefineEnum;

public class ItemObj : ItemBase
{
    [SerializeField] Upgrade _upgradeType;

    private void OnEnable()
    {
        _priceTxt.text = _price.ToString() + " G";
    }

    protected override void GetItem()
    {

        switch(_upgradeType)
        {
            case Upgrade.StrUp:
                _playerController.BuyStrUp(_price, 1);
                break;
            case Upgrade.HealthUp:
                _playerController.BuyHeart(_price, 1);
                break;
        }
        
        _price += 20;
    }
}
