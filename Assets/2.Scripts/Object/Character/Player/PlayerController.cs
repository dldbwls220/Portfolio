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
    [SerializeField] GameObject _musicNotePrefab;

    [SerializeField]LayerMask _stopMovement;

    [SerializeField] Transform _movePoint;
    [SerializeField] Transform _collisionPoint;
    [SerializeField] Transform _characterPos;

    [SerializeField] MonsterDetectorParent _monsterDetectorParent;
    TimingManager _tm;

    Camera _followCamera;
    GameObject _myAttackEffect;
    GameObject _myMusicNote;
    Animator _slashAnim;

    bool _isFlipY;
    bool _isAttack;
    bool _isMoving;

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
        _isAttack = false;
        _isMoving = false;

        _movePoint.parent = null;
        _baseY = transform.position.y;
        _followCamera = Camera.main;
        _myDir = LookDir.Left;

        _myAttackEffect = Instantiate(_slashAnimPrefab, transform.position, Quaternion.identity, transform);
        _slashAnim = _myAttackEffect.GetComponent<Animator>();
        _slashAnim.speed = _animSpeed;

        _myMusicNote = _musicNotePrefab; //Instantiate(_musicNotePrefab, GameObject.Find("Canvas").transform);
        _tm = _myMusicNote.GetComponent<TimingManager>();
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
            MoveNAttack(horizontal, vertical);
        }

    }

    void LateUpdate()
    {
        Vector3 desiredPosition = _movePoint.position + new Vector3(0, 0, -10);
        _followCamera.transform.position = Vector3.Lerp(_followCamera.transform.position, desiredPosition, _moveSpeed * Time.deltaTime);
    }

    void MoveNAttack(float horizontal, float vertical)
    {
        if (_isMoving) return;
        

        if (Mathf.Abs(horizontal) == 1f)
        {
            
            if (horizontal > 0f)
            {
                if (!_tm.CheckTiming()) return;
                StartCoroutine(MoveCooldown());
                _myDir = LookDir.Right;
                _characterBody.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
                _characterBody.transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
                if (_monsterDetectorParent.IsMonsterInDirection(_myDir.ToString()))
                {
                    Attack();
                    return;
                }

            }
            else if (horizontal < 0f)
            {
                if (!_tm.CheckTiming()) return;
                StartCoroutine(MoveCooldown());
                _myDir = LookDir.Left;
                _characterBody.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
                _characterBody.transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = true;
                if (_monsterDetectorParent.IsMonsterInDirection(_myDir.ToString()))
                {
                    Attack();
                    return;
                }
            }
            if (!Physics2D.OverlapCircle(_movePoint.position + new Vector3(horizontal, 0f, 0f), 0.1f, _stopMovement))
            {
                _movePoint.position += new Vector3(horizontal, 0f, 0f);
                StartCoroutine(MoveJump());

            }
        }
        else if (Mathf.Abs(vertical) == 1f)
        {
            
            if (vertical > 0f)
            {
                if (!_tm.CheckTiming()) return;
                StartCoroutine(MoveCooldown());
                _myDir = LookDir.Up;

                if (_monsterDetectorParent.IsMonsterInDirection(_myDir.ToString()))
                {
                    Attack();
                    return;
                }
            }
            else if (vertical < 0f)
            {
                if (!_tm.CheckTiming()) return;
                StartCoroutine(MoveCooldown());
                _myDir = LookDir.Down;

                if (_monsterDetectorParent.IsMonsterInDirection(_myDir.ToString()))
                {
                    Attack();
                    return;
                }
            }
            if (!Physics2D.OverlapCircle(_movePoint.position + new Vector3(0f, vertical, 0f), 0.1f, _stopMovement))
            {
                _movePoint.position += new Vector3(0f, vertical, 0f);
                StartCoroutine(MoveJump());
            }
        }          
    }

    void Attack()
    {
        if (_isAttack) return;
        StartCoroutine(AttackCooldown());

        switch (_myDir)
        {
            case LookDir.Up:
               _myAttackEffect.transform.position = transform.position + new Vector3(0, 1, 0);
                _myAttackEffect.transform.rotation = Quaternion.Euler(0, 0, 90);
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = _isFlipY = _isFlipY == false ? true : false;
                _slashAnim.SetTrigger(_weaponName.ToString());
                break;
            case LookDir.Down:
                _myAttackEffect.transform.position = transform.position + new Vector3(0, -1, 0);
                _myAttackEffect.transform.rotation = Quaternion.Euler(0, 0, -90);
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = _isFlipY = _isFlipY == false ? true : false;
                _slashAnim.SetTrigger(_weaponName.ToString());
                break;
            case LookDir.Left:
                _myAttackEffect.transform.position = transform.position + new Vector3(-1, 0, 0);
                _myAttackEffect.transform.rotation = Quaternion.identity;
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;            
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = _isFlipY = _isFlipY == false ? true : false;
                _slashAnim.SetTrigger(_weaponName.ToString());
                break;
            case LookDir.Right:
                _myAttackEffect.transform.position = transform.position + new Vector3(1, 0, 0);
                _myAttackEffect.transform.rotation = Quaternion.identity;
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
                _myAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = _isFlipY = _isFlipY == false ? true : false;
                _slashAnim.SetTrigger(_weaponName.ToString());
                break;
        }

        Debug.Log("АјАн!");
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

    IEnumerator AttackCooldown()
    {
        _isAttack = true;
        yield return new WaitForSeconds(0.3f);
        _isAttack = false;
    }

    IEnumerator MoveCooldown()
    {
        _isMoving = true;
        yield return new WaitForSeconds(0.3f);
        _isMoving = false;
    }
}
