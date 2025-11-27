using UnityEngine;

public class ExitGateObj : MonoBehaviour
{
    Vector3 _sendVector;
    PlayerController _playerController;
    GameObject _playerMovePoint;
    SpawnItemManager _spawnItemManager;
    ShopGateManager _gateManger;

    void Start()
    {
        _playerController = GameObject.Find("PlayerCharacter").GetComponent<PlayerController>();
        _playerMovePoint = GameObject.Find("PlayerMovePoint");
        _spawnItemManager = GameObject.Find("SpawnItem").GetComponent<SpawnItemManager>();
        _gateManger = GameObject.Find("ShopGates").GetComponent<ShopGateManager>();
    }

    public void GetVector(Vector3 vec)
    {
        _sendVector = vec;
        Debug.Log(vec);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerMovePoint.transform.position = _sendVector;
            _playerController.transform.position = _sendVector;
            Camera.main.transform.position = _sendVector + new Vector3(0, 0, -10);
            _spawnItemManager.MakeItemEnable();
            _spawnItemManager.SetSellingItems();
            _gateManger.SpawnGate();
            _playerController._isInShop = false;
        }    
    }

}
