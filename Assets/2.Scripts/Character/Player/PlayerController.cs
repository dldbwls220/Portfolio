using System.Collections;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : MonoBehaviour
{
    public enum LookDir
    {
        Up, 
        Down, 
        Left, 
        Right
    }

    public enum WeaponName
    {
        DaggerN,
        DaggerB,
        DaggerT,
        DaggerO1,
        DaggerO2,
        DaggerO3,

        SwordN,
        SwordB,
        SwordT,
        SwordO,

        SpearN,
        SpearB,
        SpearT,
        SpearO1,
        SpearO2,
        SpearO3,
    }

    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpHeight = 0.5f;
    [SerializeField] float _animSpeed = 1f;

    [SerializeField] GameObject _characterBody;
    [SerializeField] GameObject _slashAnimPrefab;

    [SerializeField]LayerMask _stopMovement;

    [SerializeField] Transform _movePoint;
    [SerializeField] Transform _collisionPoint;
    [SerializeField] Transform _characterPos;

    Camera _followCamera;
    GameObject _myAttackEffect;
    Animator _slashAnim;

    bool _isFlipY;

    public LookDir _myDir;
    public WeaponName _weaponName;

    float _baseY;

    private void Start()
    {
       InitCharacter();
    }

    void InitCharacter()
    {
        _isFlipY = false;

        _movePoint.parent = null;
        _baseY = transform.position.y;
        _followCamera = Camera.main;
        _myDir = LookDir.Left;

        _myAttackEffect = Instantiate(_slashAnimPrefab, transform.position, Quaternion.identity, transform);
        _slashAnim = _myAttackEffect.GetComponent<Animator>();
        _slashAnim.speed = _animSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        transform.position = Vector3.MoveTowards(transform.position, _movePoint.position, _moveSpeed * Time.deltaTime);
        
        _collisionPoint.position = _movePoint.position;

        if (Vector3.Distance(transform.position, _movePoint.position) <= 0.05f)
        {
            

            if (Mathf.Abs(horizontal) == 1f)
            {
                InitAttack();

                if (!Physics2D.OverlapCircle(_movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), 0.2f, _stopMovement))
                {
                    Move(horizontal, vertical);

                    StartCoroutine(MoveJump());
                }
            }
            else if (Mathf.Abs(vertical) == 1f)
            {
                InitAttack();

                if (!Physics2D.OverlapCircle(_movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), 0.2f, _stopMovement))
                {
                    Move(horizontal, vertical);

                    StartCoroutine(MoveJump());
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Attack();
        }
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = _movePoint.position + new Vector3(0, 0, -10);
        _followCamera.transform.position = Vector3.Lerp(_followCamera.transform.position, desiredPosition, _moveSpeed * Time.deltaTime);
    }


    void InitAttack()
    {
        _myAttackEffect.transform.rotation = Quaternion.identity;
        _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
        _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = false;
        _isFlipY = false;
    }

    void Move(float horizontal, float vertical)
    {
        _movePoint.position += new Vector3(horizontal, vertical, 0f);

        if (horizontal > 0f)
        {
            _characterBody.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
            _characterBody.transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
            _myDir = LookDir.Right;
        }
        else if(horizontal < 0f)
        {
            _characterBody.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
            _characterBody.transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = true;
            _myDir = LookDir.Left;
        }
        else if (vertical > 0f)
        {
            _myDir = LookDir.Up;
        }
        else if (vertical < 0f)
        {
            _myDir = LookDir.Down;
        }

        Debug.Log(_myDir);

        StartCoroutine(MoveJump());
    }

    void Attack()
    {
        switch (_myDir)
        {
            case LookDir.Up:
               _myAttackEffect.transform.position = transform.position + new Vector3(0, 1, 0);
                _myAttackEffect.transform.rotation = Quaternion.Euler(0, 0, 90);
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = _isFlipY = _isFlipY == false ? true : false;
                _slashAnim.SetTrigger(_weaponName.ToString());
                break;
            case LookDir.Down:
                _myAttackEffect.transform.position = transform.position + new Vector3(0, -1, 0);
                _myAttackEffect.transform.rotation = Quaternion.Euler(0, 0, -90);
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = _isFlipY = _isFlipY == false ? true : false;
                _slashAnim.SetTrigger(_weaponName.ToString());
                break;
            case LookDir.Left:
                _myAttackEffect.transform.position = transform.position + new Vector3(-1, 0, 0);
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;            
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = _isFlipY = _isFlipY == false ? true : false;
                _slashAnim.SetTrigger(_weaponName.ToString());
                break;
            case LookDir.Right:
                _myAttackEffect.transform.position = transform.position + new Vector3(1, 0, 0);
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = _isFlipY = _isFlipY == false ? true : false;
                _slashAnim.SetTrigger(_weaponName.ToString());
                break;
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
}
