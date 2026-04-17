using UnityEngine;
using DefineEnum;
using UnityEngine.SceneManagement;

public class WeaponObj : ItemBase
{
    [SerializeField] WeaponName _weapon;

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
            _playerController.BuyWeapon(_price, _weapon);
            gameObject.SetActive(false);
            ObjectPool._instance._weaponList.Remove(gameObject);
        }
    }
        
}
