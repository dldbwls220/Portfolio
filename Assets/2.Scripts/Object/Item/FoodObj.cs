using UnityEngine;
using DefineEnum;
using UnityEngine.SceneManagement;
using Protocols;

public class FoodObj : ItemBase
{
    [SerializeField] int heal;

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "GamePlayScene")
        {
            _priceTxt.text = _price.ToString() + " G";
            if (_diamond != null)
                _diamond.enabled = false;
        }

        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            if (_diamond != null)
                _diamond.enabled = true;
            _priceTxt.text = _diamondPrice.ToString();
        }
    }

    protected override void GetItem()
    {
        if (SceneManager.GetActiveScene().name == "GamePlayScene")
        {
            _playerController.BuyFood(_price, heal);
            gameObject.SetActive(false);
        }
        else if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            _isUnlocked = true;

            if (!DataManger._instance._totalData._unlockDate._unlockedItem.Contains(_itemID))
            {
                DataManger._instance._totalData._unlockDate._unlockedItem.Add(_itemID);
                NetManager._instance.UpdateItemUnlock(_itemID);
            }

            DataManger._instance._totalData._currentDiamond._totalDiamond -= _diamondPrice;
            gameObject.SetActive(false);           
        }
    }
}
