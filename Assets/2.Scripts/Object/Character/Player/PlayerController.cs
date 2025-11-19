using DefineEnum;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerController : CharBase
{

    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpHeight = 0.5f;
    [SerializeField] float _animSpeed = 1f;

    [SerializeField] GameObject _characterBody;
    [SerializeField] GameObject _slashAnimPrefab;
    [SerializeField] GameObject _musicNotePrefab;
    [SerializeField] GameObject _attackRangeParent;
    [SerializeField] GameObject _monsterSlashFX;

    [SerializeField] BoxCollider2D[] _attackColliders;

    [SerializeField]LayerMask _stopMovement;

    [SerializeField] Transform _movePoint;
    [SerializeField] Transform _collisionPoint;
    [SerializeField] Transform _characterPos;

    [SerializeField] MonsterDetectorParent _monsterDetectorParent;
    
    TimingManager _tm;
    CheckAttackRange[] _weaponCheck;

    Camera _followCamera;
    GameObject _myAttackEffect;
    GameObject _myMusicNote;
    Animator _slashAnim;
    Animator _monsterSlashAnim;

    bool _isFlipY;
    bool _isAttack;
    bool _isMoving;
    bool _isDelayEnd;

    public LookDir _myDir;
    public WeaponName _weaponName;

    float _baseY;

    private void Start()
    {
       InitCharacter();
    }

    void InitCharacter()
    {
        InitBaseSet("Cadence", 1, 2, 0, 0);



        _isFlipY = false;
        _isAttack = false;
        _isMoving = false;
        _isDelayEnd = false;

        _movePoint.parent = null;
        _baseY = transform.position.y;
        _followCamera = Camera.main;
        _myDir = LookDir.Left;
        _weaponName = WeaponName.SwordB;

        _myAttackEffect = Instantiate(_slashAnimPrefab, transform.position, Quaternion.identity, transform);
        _slashAnim = _myAttackEffect.GetComponent<Animator>();
        _slashAnim.speed = _animSpeed;
        _monsterSlashAnim = _monsterSlashFX.GetComponent<Animator>();

        _myMusicNote = _musicNotePrefab; //Instantiate(_musicNotePrefab, GameObject.Find("Canvas").transform);
        _tm = _myMusicNote.GetComponent<TimingManager>();

        Debug.Log(_attackRangeParent.transform.childCount);

        _attackColliders = new BoxCollider2D[_attackRangeParent.transform.childCount];
        _weaponCheck = new CheckAttackRange[_attackRangeParent.transform.childCount];

        for (int i = 0; i < _attackRangeParent.transform.childCount; i++)
        {        
            _attackColliders[i] = _attackRangeParent.transform.GetChild(i).GetComponent<BoxCollider2D>();
            _weaponCheck[i] = _attackColliders[i].GetComponent<CheckAttackRange>();
            _weaponCheck[i].InitSetRange(this);
        }

       
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
                _monsterSlashFX.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX=false;
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
                _monsterSlashFX.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
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
        StartCoroutine(CameraShaker(0.05f, 0.3f));
        Debug.Log("АјАн!");
    }

    public void OnHitting(float dmg)
    {
        if ((_nowHp -= dmg) <= 0)
        {
            _nowHp = 0;
        }
        else
        {
            StartCoroutine(GetDamageBlink());
            StartCoroutine(CameraShaker(0.05f, 0.3f));
            _monsterSlashAnim.SetTrigger("E_Attack");
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

    IEnumerator GetDamageBlink()
    {
        float time = 0;
        float _blinkDuration = 0.4f;
        float _blinkSpeed = 0.05f;

        SpriteRenderer head = _characterBody.transform.GetChild(1).GetComponent<SpriteRenderer>();
        SpriteRenderer body = _characterBody.transform.GetChild(0).GetComponent<SpriteRenderer>();

        while (time < _blinkDuration)
        {
            head.enabled = !head.enabled;
            body.enabled = !body.enabled;
            time += Time.deltaTime / _blinkSpeed;
            yield return new WaitForSeconds(_blinkSpeed);
        }

        head.enabled = true;
        body.enabled = true;
    }

    IEnumerator CameraShaker(float shakeAmount, float shakeTime)
    {
        float time = 0;
        while (time < shakeTime)
        {
            _followCamera.transform.position = (Vector3)Random.insideUnitSphere * shakeAmount + (new Vector3(0, 0, -10) + transform.position);
            time += Time.deltaTime;
            yield return null;
        }

        _followCamera.transform.position = new Vector3(0, 0, -10) + transform.position;
    }
}
