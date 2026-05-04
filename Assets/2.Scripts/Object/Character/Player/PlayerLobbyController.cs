using DefineEnum;
using System.Collections;
using UnityEngine;

public class PlayerLobbyController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpHeight = 0.5f;
    [SerializeField] float _animSpeed = 1f;

    [SerializeField] GameObject _characterBody;
    [SerializeField] GameObject _weaponUI;

    [SerializeField] LayerMask _stopMovement;

    [SerializeField] Transform _movePoint;
    [SerializeField] Transform _characterPos;

    [SerializeField] NumberUI _numberUI;

    Camera _followCamera;
    HealthBarManager _healthBarManager;
    WeaponUI _weaponSelectUI;

    bool _isMoving;

    LookDir _myDir;
    public WeaponName _weaponName;

    float _baseY;

    public LookDir _checkDir { get { return _myDir; } }
    public WeaponName _currentWeapon { get { return _weaponName; } }

    private void Start()
    {
        InitCharacter();
    }

    void InitCharacter()
    {
        gameObject.SetActive(true);

        _isMoving = false;

        _movePoint.parent = null;
        _baseY = _characterBody.transform.localPosition.y;
        _followCamera = Camera.main;
        _followCamera.transform.position = _movePoint.position + new Vector3(0, 0, -10);
        _myDir = LookDir.Left;
        _weaponName = WeaponName.DaggerN;

        //_weaponSelectUI.InitWeaponUI();
        //_weaponSelectUI.ChangeWeapon(_weaponName);
        //_weaponSelectUI.SetInfoText(_weaponName);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneControlManager._instance._isBeginning) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        transform.position = Vector3.MoveTowards(transform.position, _movePoint.position, _moveSpeed * Time.deltaTime);


        if (Vector3.Distance(transform.position, _movePoint.position) <= 0.05f)
        {
            Move(horizontal, vertical);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DataManger._instance._totalData._currentDiamond._totalDiamond += 1;

            LobbyUI._instance.UpdateDiamondCountUI();
        }

    }

    void LateUpdate()
    {
        Vector3 desiredPosition = _movePoint.position + new Vector3(0, 0, -10);
        _followCamera.transform.position = Vector3.Lerp(_followCamera.transform.position, desiredPosition, _moveSpeed * Time.deltaTime);
    }

    void Move(float horizontal, float vertical)
    {
        if (_isMoving) return;


        if (Mathf.Abs(horizontal) == 1f)
        {

            if (horizontal > 0f)
            {
                StartCoroutine(MoveCooldown());
                _myDir = LookDir.Right;
                _characterBody.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
                _characterBody.transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;

            }
            else if (horizontal < 0f)
            {
                StartCoroutine(MoveCooldown());
                _myDir = LookDir.Left;
                _characterBody.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
                _characterBody.transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = true;
            }
            if (!Physics2D.OverlapCircle(_movePoint.position + new Vector3(horizontal, 0f, 0f), 0.1f, _stopMovement))
            {
                _movePoint.position += new Vector3(horizontal, 0f, 0f);
                StartCoroutine(MoveJump());
            }


            CheckBoardTileMap._instance.ChangTile();
        }
        else if (Mathf.Abs(vertical) == 1f)
        {

            if (vertical > 0f)
            {
                StartCoroutine(MoveCooldown());
                _myDir = LookDir.Up;
            }
            else if (vertical < 0f)
            {
                StartCoroutine(MoveCooldown());
                _myDir = LookDir.Down;
            }

            if (!Physics2D.OverlapCircle(_movePoint.position + new Vector3(0f, vertical, 0f), 0.1f, _stopMovement))
            {
                _movePoint.position += new Vector3(0f, vertical, 0f);
                StartCoroutine(MoveJump());
            }


            CheckBoardTileMap._instance.ChangTile();
        }

    }

    IEnumerator MoveJump()
    {
        float t = 0;
        float moveTime = 1 / _moveSpeed;

        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;
            float height = Mathf.Sin(t * Mathf.PI) * _jumpHeight;
            Vector3 charPos = _characterPos.localPosition;
            charPos.y = _baseY + height;
            _characterPos.localPosition = charPos;
            yield return null;
        }
        _characterPos.localPosition = new Vector3(_characterPos.localPosition.x, _baseY, _characterPos.localPosition.z);
    }

    IEnumerator MoveCooldown()
    {
        _isMoving = true;
        yield return new WaitForSeconds(0.1f);
        _isMoving = false;
    }

}
