using UnityEngine;

public class ShopGateObj : MonoBehaviour
{
    PlayerController _playerController;
    ExitGateObj _exitGate;
    GameObject _playerMovePoint;
    GameObject _otherGate;
    Vector3 _gatePos;

    private void Start()
    {
        _playerController = GameObject.Find("PlayerCharacter").GetComponent<PlayerController>();
        _playerMovePoint = GameObject.Find("PlayerMovePoint");
        _otherGate = GameObject.Find("OtherGatePos");
        _exitGate = GameObject.Find("ExitGate").GetComponent<ExitGateObj>();
        _gatePos = transform.position + Vector3.up;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerMovePoint.transform.position = _otherGate.transform.position;
            _playerController.transform.position = _otherGate.transform.position;
            Camera.main.transform.position = _otherGate.transform.position + new Vector3(0,0,-10);
            _exitGate.GetVector(_gatePos);
            _playerController._isInShop = true;

            gameObject.SetActive(false);

            Debug.Log("¿Ãµø¡ﬂ");
        }
    }
}
