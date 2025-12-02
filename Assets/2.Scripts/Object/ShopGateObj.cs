using DefineEnum;
using UnityEngine;

public class ShopGateObj : MonoBehaviour
{
    PlayerController _playerController;
    ExitGateObj _exitGate;
    GameObject _playerMovePoint;
    GameObject _otherGate;

    [SerializeField] float _minDistance = 2f;
    [SerializeField] float _maxDistance = 15f;

    private void Start()
    {
        _playerController = GameObject.Find("PlayerCharacter").GetComponent<PlayerController>();
        _playerMovePoint = GameObject.Find("PlayerMovePoint");
        _otherGate = GameObject.Find("OtherGatePos");
        _exitGate = GameObject.Find("ExitGate").GetComponent<ExitGateObj>();
    }

    void Update()
    {
        if (IngameManager._instance._bansheeSound) return;

        float distance = Vector3.Distance(_playerController.transform.position, transform.position);
        
        float t = Mathf.InverseLerp(_minDistance, _maxDistance, distance);

        float baseShopVol = 0.5f;

        float volumeShop = Mathf.Lerp(0.6f, 0f, t);

        float volumeLoop = Mathf.Lerp(0f, baseShopVol, t);

        SoundManager._instance._loopDESC._volum = volumeLoop;
        SoundManager._instance._shopkeeperDESC._volum = volumeShop;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerMovePoint.transform.position = _otherGate.transform.position;
            _playerController.transform.position = _otherGate.transform.position;
            Camera.main.transform.position = _otherGate.transform.position + new Vector3(0,0,-10);
            _exitGate.GetVector(transform.position + Vector3.up);
            _playerController._isInShop = true;

            int rnd = Random.Range((int)SFXName.Cadence_teleport_01, (int)SFXName.Cadence_teleport_05 + 1);
            SoundManager._instance.PlaySFX((SFXName)rnd);

            gameObject.SetActive(false);

            Debug.Log("¿Ãµø¡ﬂ");
        }
    }
}
