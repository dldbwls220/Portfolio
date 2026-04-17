using UnityEngine;
using DefineEnum;
using UnityEngine.SceneManagement;

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
            _unlockedI._unlockedItem.Add(_itemID);
            gameObject.SetActive(false);
            DataManger._instance.SaveData();
        }
    }
}
