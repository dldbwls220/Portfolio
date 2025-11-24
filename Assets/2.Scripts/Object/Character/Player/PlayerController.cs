using DefineEnum;
using System.Collections;
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
    [SerializeField] GameObject _myAttackEffect;
    [SerializeField] GameObject _heartUI;

    [SerializeField] BoxCollider2D[] _attackColliders;

    [SerializeField]LayerMask _stopMovement;

    [SerializeField] Transform _movePoint;
    [SerializeField] Transform _collisionPoint;
    [SerializeField] Transform _characterPos;

    [SerializeField] MonsterDetectorParent _monsterDetectorParent;
    
    TimingManager _tm;
    CheckAttackRange[] _weaponCheck;

    Camera _followCamera;
    GameObject _myMusicNote;
    Animator _slashAnim;
    Animator _monsterSlashAnim;
    HealthBarManager _healthBarManager;

    bool _isFlipY;
    bool _isAttack;
    bool _isMoving;
    bool _isDelayEnd;

    LookDir _myDir;
    public WeaponName _weaponName;
    public LookDir _checkDir { get { return _myDir; } }

    int _combo;
    float _baseY;

    public float _str { get { return _strength; } }

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
        _baseY = _characterBody.transform.localPosition.y;
        _followCamera = Camera.main;
        _myDir = LookDir.Left;
        _weaponName = WeaponName.SwordO;

        _myAttackEffect = Instantiate(_slashAnimPrefab, transform.position, Quaternion.identity, transform);
        _slashAnim = _myAttackEffect.GetComponent<Animator>();
        _slashAnim.speed = _animSpeed;
        _monsterSlashAnim = _monsterSlashFX.GetComponent<Animator>();
        _healthBarManager = _heartUI.GetComponent<HealthBarManager>();

        _myMusicNote = _musicNotePrefab; //Instantiate(_musicNotePrefab, GameObject.Find("Canvas").transform);
        _tm = _myMusicNote.GetComponent<TimingManager>();

        Debug.Log(_attackRangeParent.transform.childCount);

        _attackColliders = new BoxCollider2D[_attackRangeParent.transform.childCount];
        _weaponCheck = new CheckAttackRange[_attackRangeParent.transform.childCount];

        _healthBarManager.ClearHeart();
        _healthBarManager.CreateEmptyHeart(_maxHP, false);
        _healthBarManager.DrawHearts(_nowHp);

        for (int i = 0; i < _attackRangeParent.transform.childCount; i++)
        {        
            _attackColliders[i] = _attackRangeParent.transform.GetChild(i).GetComponent<BoxCollider2D>();
            _weaponCheck[i] = _attackColliders[i].GetComponent<CheckAttackRange>();
            _weaponCheck[i].InitSetRange(this);
            _attackColliders[i].enabled = false;
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

            }
            else if (horizontal < 0f)
            {
                if (!_tm.CheckTiming()) return;
                StartCoroutine(MoveCooldown());
                _myDir = LookDir.Left;
                _characterBody.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
                _characterBody.transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = true;
                _monsterSlashFX.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
            }
            if (CheckMonster())
            {
                Attack();
            }
            else if (!Physics2D.OverlapCircle(_movePoint.position + new Vector3(horizontal, 0f, 0f), 0.1f, _stopMovement))
            {
                _movePoint.position += new Vector3(horizontal, 0f, 0f);
                StartCoroutine(MoveJump());
                initCombo();
            }
        }
        else if (Mathf.Abs(vertical) == 1f)
        {
            
            if (vertical > 0f)
            {
                if (!_tm.CheckTiming()) return;
                StartCoroutine(MoveCooldown());
                _myDir = LookDir.Up;          
            }
            else if (vertical < 0f)
            {
                if (!_tm.CheckTiming()) return;
                StartCoroutine(MoveCooldown());
                _myDir = LookDir.Down;              
            }
            if (CheckMonster())
            {
                Attack();              
            }
            else if (!Physics2D.OverlapCircle(_movePoint.position + new Vector3(0f, vertical, 0f), 0.1f, _stopMovement))
            {
                _movePoint.position += new Vector3(0f, vertical, 0f);
                StartCoroutine(MoveJump());
                initCombo();
            }
        }          
    }

    void Attack()
    {
        if (_isAttack) return;
        StartCoroutine(AttackCooldown());

        switch (_combo)
        {
            case 0:
                SoundManager._instance.PlaySFX(SFXName.Cadence_Attack_Combo_01);
                _combo++;
                break;
            case 1:
                SoundManager._instance.PlaySFX(SFXName.Cadence_Attack_Combo_02);
                _combo++;
                break;
            case 2:
                SoundManager._instance.PlaySFX(SFXName.Cadence_Attack_Combo_03);
                _combo++;
                break;
            case 3:
                SoundManager._instance.PlaySFX(SFXName.Cadence_Attack_Combo_04);
                initCombo();
                break;
        }

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
       
        Debug.Log("공격!");
    }

    public void EnableWeaponCollider(int index)
    {
        Debug.Log(index);

        switch (index)
        {
            case (int)WeaponName.DaggerN:
                if (_myDir == LookDir.Up)
                    _attackColliders[(int)LookDir.Up].enabled = true;
                else if (_myDir == LookDir.Down)
                    _attackColliders[(int)LookDir.Down].enabled = true;
                else if (_myDir == LookDir.Left)
                    _attackColliders[(int)LookDir.Left].enabled = true;
                else if (_myDir == LookDir.Right)
                    _attackColliders[(int)LookDir.Right].enabled = true;
                break;
            case (int)WeaponName.SwordN:
                if (_myDir == LookDir.Up)
                {
                    _attackColliders[(int)LookDir.Up].enabled = true;
                    _attackColliders[(int)LookDir.UpLeft].enabled = true;
                    _attackColliders[(int)LookDir.UpRight].enabled = true;
                }
                else if (_myDir == LookDir.Down)
                {
                    _attackColliders[(int)LookDir.Down].enabled = true;
                    _attackColliders[(int)LookDir.DownRight].enabled = true;
                    _attackColliders[(int)LookDir.DownLeft].enabled = true;
                }
                else if (_myDir == LookDir.Left)
                {
                    _attackColliders[(int)LookDir.Left].enabled = true;
                    _attackColliders[(int)LookDir.UpLeft].enabled = true;
                    _attackColliders[(int)LookDir.DownLeft].enabled = true;
                }
                else if (_myDir == LookDir.Right)
                {
                    _attackColliders[(int)LookDir.Right].enabled = true;
                    _attackColliders[(int)LookDir.UpRight].enabled = true;
                    _attackColliders[(int)LookDir.DownRight].enabled = true;
                }
                break;
        }
    }

    public void DisableWeaponCollider()
    {
        _attackColliders[(int)LookDir.Up].enabled = false;
        _attackColliders[(int)LookDir.UpRight].enabled = false;
        _attackColliders[(int)LookDir.UpLeft].enabled = false;
        _attackColliders[(int)LookDir.Down].enabled = false;
        _attackColliders[(int)LookDir.DownRight].enabled = false;
        _attackColliders[(int)LookDir.DownLeft].enabled = false;
        _attackColliders[(int)LookDir.Left].enabled = false;
        _attackColliders[(int)LookDir.Right].enabled = false;
    }

    bool CheckMonster()
    {
        bool ischeck = false;

        switch (_weaponName)
        {
            case WeaponName.DaggerN:
            case WeaponName.DaggerB:
            case WeaponName.DaggerT:
            case WeaponName.DaggerO1:
            case WeaponName.DaggerO2:
            case WeaponName.DaggerO3:
                switch (_myDir)
                {
                    case LookDir.Up:
                        ischeck = _monsterDetectorParent.IsMonsterInDirection(_myDir.ToString());
                        break;
                    case LookDir.Down:
                        ischeck = _monsterDetectorParent.IsMonsterInDirection(_myDir.ToString());
                        break;
                    case LookDir.Left:
                        ischeck = _monsterDetectorParent.IsMonsterInDirection(_myDir.ToString());
                        break;
                    case LookDir.Right:
                        ischeck = _monsterDetectorParent.IsMonsterInDirection(_myDir.ToString());
                        break;
                }
                break;
            case WeaponName.SwordN:
            case WeaponName.SwordB:
            case WeaponName.SwordT:
            case WeaponName.SwordO:
                switch (_myDir)
                {
                    case LookDir.Up:
                        ischeck = (_monsterDetectorParent.IsMonsterInDirection(_myDir.ToString()) || _monsterDetectorParent.IsMonsterInDirection(LookDir.UpLeft.ToString()) || _monsterDetectorParent.IsMonsterInDirection(LookDir.UpRight.ToString()));
                        break;
                    case LookDir.Down:
                        ischeck = (_monsterDetectorParent.IsMonsterInDirection(_myDir.ToString()) || _monsterDetectorParent.IsMonsterInDirection(LookDir.DownLeft.ToString()) || _monsterDetectorParent.IsMonsterInDirection(LookDir.DownRight.ToString()));
                        break;
                    case LookDir.Left:
                        ischeck = (_monsterDetectorParent.IsMonsterInDirection(_myDir.ToString()) || _monsterDetectorParent.IsMonsterInDirection(LookDir.DownLeft.ToString()) || _monsterDetectorParent.IsMonsterInDirection(LookDir.UpLeft.ToString()));
                        break;
                    case LookDir.Right:
                        ischeck = (_monsterDetectorParent.IsMonsterInDirection(_myDir.ToString()) || _monsterDetectorParent.IsMonsterInDirection(LookDir.DownRight.ToString()) || _monsterDetectorParent.IsMonsterInDirection(LookDir.UpRight.ToString()));
                        break;
                }
                break;
        }

        return ischeck;
    }

    void initCombo()
    {
        _combo = 0;
    }

    public void OnHitting(float dmg)
    {
        if ((_nowHp -= dmg) <= 0)
        {
            _nowHp = 0;
            int rnd = Random.Range((int)SFXName.Cadence_death_01, (int)SFXName.Cadence_death_03 + 1);
            SoundManager._instance.PlaySFX((SFXName)rnd);
            SoundManager._instance.PlaySFX(SFXName.sfx_player_death_ST);
            _monsterSlashAnim.SetTrigger("E_Attack");
            _healthBarManager.DrawHearts(_nowHp);
            _dead = true;
        }
        else
        {
            StartCoroutine(GetDamageBlink());
            StartCoroutine(CameraShaker(0.05f, 0.3f));

            int rnd = Random.Range((int)SFXName.Cadence_hurt_01, (int)SFXName.Cadence_hurt_06+1);
            SoundManager._instance.PlaySFX((SFXName)rnd);
            SoundManager._instance.PlaySFX(SFXName.sfx_player_hit_ST);
            _monsterSlashAnim.SetTrigger("E_Attack");
            _healthBarManager.DrawHearts(_nowHp);
            Debug.Log(dmg+"데미지");
        }

    }

    void AdjustSortingLayer()
    {

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
