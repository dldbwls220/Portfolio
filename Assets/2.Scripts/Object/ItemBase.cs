using UnityEngine;
using UnityEngine.UI;
using DefineEnum;
using UnityEngine.SceneManagement;
using DefineStructure;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField] protected int _price;
    [SerializeField] protected int _diamondPrice;
    [SerializeField] protected string _itemID;
    [SerializeField] protected Text _priceTxt;
    [SerializeField] protected Image _diamond;
    protected PlayerController _playerController;
    protected UnlockedItem _unlockedI;

    public bool _isUnlocked;

    public string _thisItemID { get { return _itemID; } }

    private void Start()
    {
        _playerController = GameObject.Find("PlayerCharacter").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SceneManager.GetActiveScene().name == "GamePlayScene")
        {
            if (_playerController._goldContain >= _price)
            {
                GetItem();
                SoundManager._instance.PlaySFX(SFXName.sfx_pickup_purchase);
            }
            else
                SoundManager._instance.PlaySFX(SFXName.sfx_error_ST);
        }
        else if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            if (DataManger._instance._totalData._currentDiamond._totalDiamond >= _diamondPrice)
            {
                GetItem();
                LobbyUI._instance.UpdateDiamondCountUI();
                SoundManager._instance.PlaySFX(SFXName.sfx_pickup_purchase);
            }
            else
                SoundManager._instance.PlaySFX(SFXName.sfx_error_ST);
        }

    }

    protected abstract void GetItem();
}
