using DefineEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDragonObject : MonsterBase
{
    [SerializeField] PathFinding _pFinder;
    [SerializeField] TileMapGridManager _tileManager;
    [SerializeField] Transform _targetTF;
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] Transform _characterPos;
    [SerializeField] GameObject _breathObj;
    [SerializeField] GameObject _fireSpriteObj;
    [SerializeField] GameObject _heartUI;

    [SerializeField] LayerMask _wallDetect;

    List<Node> _path;
    Node _startNode;
    Node _targetNode;
    int _myBeat = 0;
    bool _isMoving = false;
    int _fireLength;

    Animator _anim;
    Animator _fireAnim;
    PlayerController _playerController;
    SpriteRenderer[] _fireSprite;
    HealthBarManager _healthBarManager;

    bool _isKeepFire;
    bool _isFire;
    bool _isAttack;

    void OnEnable()
    {

        NoteManager._instance.OnBeat += OnBeat;
       
        if (_isDead)
        {
            InitMonsterStat();
            _dead = false;
        }
    }

    private void OnDisable()
    {

        NoteManager._instance.OnBeat -= OnBeat;
        
    }

    void Update()
    {
        CheckPlayerinRange();
        DetectAttack();
        if (_path != null)
        {
            Vector3 dir = new Vector3(_path[1]._worldPosition.x - transform.position.x, 0, 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, _fireSpriteObj.transform.childCount, _wallDetect);
            Debug.DrawRay(transform.position, dir * hit.distance, Color.yellow);
            if (hit.collider != null)
            {
                _fireLength = (int)hit.distance;
            }
            else
            {
                _fireLength = _fireSpriteObj.transform.childCount;
            }
        }
       
    }

    public void InitMonster(int enemyIndex)
    {
        TableBase table = GameTableManager._instance.Get(TableName.MonsterInfoList);
        string name = table.ToS(enemyIndex, "Name");
        float hp = table.ToF(enemyIndex, "HP");
        int str = table.ToI(enemyIndex, "Strength");
        int gold = table.ToI(enemyIndex, "Gold");
        int beat = table.ToI(enemyIndex, "Beat");

        InitBaseSet(name, str, hp, gold, beat);

        _pFinder = GameObject.Find("GridManager").GetComponent<PathFinding>();
        _tileManager = GameObject.Find("GridManager").GetComponent<TileMapGridManager>();
        _targetTF = GameObject.Find("PlayerCharacter").transform;
        _fireAnim = _breathObj.GetComponent<Animator>();
        _fireSprite = new SpriteRenderer[_fireSpriteObj.transform.childCount];
        _healthBarManager = _heartUI.GetComponent<HealthBarManager>();

        for (int i = 0; i < _fireSpriteObj.transform.childCount; i++)
        {
            _fireSprite[i] = _fireSpriteObj.transform.GetChild(i).GetComponent<SpriteRenderer>();
        }

        _playerController = _targetTF.GetComponent<PlayerController>();
        _anim = GetComponent<Animator>();
        _anim.speed = (IngameManager._instance._myBPM / 60f);
        _isAttack = false;
        _isFire = false;
        _isKeepFire = true;
        _monsterP = MonsterPriority.RedDragon;

        _healthBarManager.ClearHeart();
        _healthBarManager.CreateEmptyHeart(_maxHP);
        _healthBarManager.DrawHearts(_nowHp);
    }

    void InitMonsterStat()
    {
        _isMoving = false;
        _nowHp = _maxHP;
        _myBeat = 0;
        _healthBarManager.ClearHeart();
        _healthBarManager.CreateEmptyHeart(_maxHP);
        _healthBarManager.DrawHearts(_nowHp);
    }
    void OnBeat()
    {
        if (_isMoving || _playerController._isDead || _playerController._isInShop || IngameManager._instance._gameEnd) return;
        _isAttack = true;
        _myBeat += 1;
        if (_myBeat > 4)
            _myBeat = 1;
        _anim.SetTrigger(_myBeat + "Beat");

        if (_path != null)
        {
            InitFireLength();
        }

        _startNode = _tileManager.NodeFromWorldPos(transform.position);
        _targetNode = _tileManager.NodeFromWorldPos(_targetTF.position);
        
        _startNode._walkable = false;

        _path = _pFinder.FindPath(_startNode._worldPosition, _targetNode._worldPosition, this);

        if (_path == null)
        {
            // 예약 무시한 순수 A* 다시 시도
            _path = _pFinder.FindPath(_startNode._worldPosition, _targetNode._worldPosition, null);
        }

        if (_path == null || _path.Count < 2)
            return;

        Node nextNode = _path[1];

        if (!CanReserve(nextNode))
        {
            // 우회 경로 재탐색
            _path = _pFinder.FindPath(_startNode._worldPosition, _targetNode._worldPosition, this);

            if (_path == null || _path.Count < 2)
                return;

            nextNode = _path[1];

            if (!CanReserve(nextNode))
                return;
        }

        nextNode._reservedBy = this;

        _tileManager.SetDebugPath(gameObject.name, _path);

        int diff = (int)_startNode._worldPosition.y - (int)_targetNode._worldPosition.y;

        switch (_myBeat)
        {
            case 1:
                if(_startNode._worldPosition.y == _targetNode._worldPosition.y && _path.Count > 1 && _path.Count < 9)
                {
                    _isFire = true;
                    _anim.SetBool("isFire", true);
                    SoundManager._instance.PlaySFX(SFXName.Dragon_attack_prefire);
                }
                else if(Mathf.Abs(diff) == 1 && _path.Count > 2 && _path.Count < 9)
                {
                    _isFire = true;
                    _anim.SetBool("isFire", true);
                    SoundManager._instance.PlaySFX(SFXName.Dragon_attack_prefire);
                    StartCoroutine(MoveToNode(_path[1]));
                }
                break;
            case 2:

                if (_isFire)
                {
                    SoundManager._instance.PlaySFX(SFXName.Dragon_attack_fire);
                    _fireAnim.SetTrigger("Fire");
                    if (_playerController._isPlayerAttackable && _startNode._worldPosition.y == _targetNode._worldPosition.y)
                        AttackPlayer();
                }             
                else
                    StartCoroutine(MoveToNode(_path[1]));
                break;
            case 3:
                _isFire = false;
                _anim.SetBool("isFire", false);
                break;
            case 4:              
                StartCoroutine(MoveToNode(_path[1]));
                break;
        }
        
    }

    public void OnHitting(float dmg)
    {
        if ((_nowHp -= dmg) <= 0)
        {
            _nowHp = 0;
            SoundManager._instance.PlaySFX(SFXName.Dragon_death);

            _healthBarManager.ClearHeart();

            if (_path != null && _path.Count > 1)
            {
                ReleaseReservation(_path[1]);
                _startNode._walkable = true;
            }

            _dead = true;
            IngameManager._instance.KillCount();
            SpawnGold(_gold);
            IngameManager._instance.BossCount();
            IngameManager._instance.UpgradeMonster();
            StartCoroutine(Yeah());
            ObjectPool._instance._redDragonQueue.Enqueue(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            int rnd = Random.Range((int)SFXName.Dragon_hurt_01, (int)SFXName.Dragon_hurt_03 + 1);
            SoundManager._instance.PlaySFX((SFXName)rnd);

            _healthBarManager.DrawHearts(_nowHp);
        }
    }

    void InitFireLength()
    {
        for (int i = 0; i < _fireSprite.Length; i++)
            _fireSprite[i].enabled = false;

        Vector3 dir = new Vector3(_path[1]._worldPosition.x - transform.position.x, 0, 0);

        for (int i = 0; i < _fireLength; i++)
            _fireSprite[i].enabled = true;      
    }

    protected override void CheckPlayerinRange()
    {
        base.CheckPlayerinRange();
    }

    IEnumerator MoveToNode(Node nextNode)
    {
        _isMoving = true;

        Vector3 targetPos = nextNode._worldPosition;

        Node currentNode = _tileManager.NodeFromWorldPos(transform.position);
        _startNode._walkable = true;
        ReleaseReservation(currentNode);

        float diffX = _targetNode._worldPosition.x - transform.position.x;

        if (diffX > 0)
        {
            for (int i = 0; i < _breathObj.transform.GetChild(0).transform.childCount; i++)
            {
                _fireSprite[i].flipX = true;
            }
            _fireSpriteObj.transform.rotation = Quaternion.Euler(0, 0, 180);
            _fireSpriteObj.transform.localPosition = new Vector3(1, 0, 0);
            transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (diffX < 0)
        {
            for (int i = 0; i < _breathObj.transform.GetChild(0).transform.childCount; i++)
            {
                _fireSprite[i].flipX = false;
            }
            _fireSpriteObj.transform.rotation = Quaternion.Euler(0, 0, 0);
            _fireSpriteObj.transform.localPosition = new Vector3(-1, 0, 0);
            transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
        }


        StartCoroutine(MoveJump());
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                _moveSpeed * Time.deltaTime
            );

            yield return null;
        }
        transform.position = targetPos;

        if (_detectPlayer)
        {
            int rnd = Random.Range((int)SFXName.Dragon_walk_01, (int)SFXName.Dragon_walk_03 + 1);
            SoundManager._instance.PlaySFX((SFXName)rnd);
        }

        ReleaseReservation(nextNode);

        _isMoving = false;
    }

    IEnumerator AttackFrontBack(Node nextNode)
    {
        _isAttack = false;

        Vector3 origin = _path[0]._worldPosition;
        Vector3 dir = (nextNode._worldPosition - origin).normalized;

        float t = 0;
        float moveTime = 1 / _moveSpeed;
        AttackPlayer();

        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;

            float move = Mathf.Sin(t * Mathf.PI);
            Vector3 offset = dir * move /** jumpHeight*/;

            transform.position = origin + offset;

            yield return null;
        }

    }

    IEnumerator MoveJump()
    {
        float t = 0;
        float moveTime = 1 / _moveSpeed;

        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;
            float height = Mathf.Sin(t * Mathf.PI) * 0.5f;
            Vector3 charPos = _characterPos.localPosition;
            charPos.y = 0 + height;
            _characterPos.localPosition = charPos;
            yield return null;
        }
        _characterPos.localPosition = new Vector3(_characterPos.localPosition.x, 0, _characterPos.localPosition.z);

    }

    IEnumerator Yeah()
    {
        yield return new WaitForSeconds(0.6f);
        int rnd = Random.Range((int)SFXName.Cadence_yeah_01, (int)SFXName.Cadence_yeah_05 + 1);

        SoundManager._instance.PlaySFX((SFXName)rnd);
    }

    void DetectAttack()
    {
        Node playerNowNode = _tileManager.NodeFromWorldPos(_targetTF.position);
        Node MonsterNowNode = _tileManager.NodeFromWorldPos(transform.position);

        if (MonsterNowNode == playerNowNode && _isAttack && _playerController._isPlayerAttackable)
        {
            StartCoroutine(AttackFrontBack(_path[1]));
        }

    }

    void AttackPlayer()
    {
        Debug.Log("공격 성공! 플레이어가 공격 경로로 들어옴");
        // TODO: 데미지 처리
        SoundManager._instance.PlaySFX(SFXName.Skel_attack_melee);
        _playerController.OnHitting(_strength);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PWeapon"))
        {
            CheckAttackRange car = collision.GetComponent<CheckAttackRange>();
            PlayerController pc = car.GetOwner<PlayerController>();
            OnHitting(pc._str);
        }
    }
}
