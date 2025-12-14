using DefineEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BatObject : MonsterBase
{
    [SerializeField] PathFinding _pFinder;
    [SerializeField] TileMapGridManager _tileManager;
    [SerializeField] GameObject _playerObj;
    [SerializeField] GameObject _spriteObj;
    [SerializeField] GameObject _heartUI;

    [SerializeField] float _moveSpeed = 5f;

    Transform _targetTF;
    PlayerController _playerController;
    HealthBarManager _heartManager;


    List<Node> _path;
    Node _startNode;
    Node _randomNode;


    int _myBeat;
    bool _isMoving;
    bool _isAttack;
    bool _isHitable;

    Vector2Int[] dir = new Vector2Int[]
    {
        new Vector2Int (1,0), //오른쪽
        new Vector2Int (-1,0),//왼쪽
        new Vector2Int (0,1), //위
        new Vector2Int (0,-1) //아래
    };

    Animator _animController;

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

    private void Update()
    {
        DetectAttack();
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

        Debug.Log(str+"힘");

        _pFinder = GameObject.Find("GridManager").GetComponent<PathFinding>();
        _tileManager = GameObject.Find("GridManager").GetComponent<TileMapGridManager>();
        _playerObj = GameObject.Find("PlayerCharacter");

        _targetTF = _playerObj.transform;
        _playerController = _targetTF.GetComponent<PlayerController>();
        _heartManager = _heartUI.GetComponent<HealthBarManager>();
        
        _animController = GetComponent<Animator>();
        _animController.speed = (IngameManager._instance._myBPM / 60f);
        _myBeat = 0;
        _isAttack = false;
        _isMoving = false;
        _isHitable = true;

        if (name == "DireBat")
            _monsterP = MonsterPriority.DireBat;
        else
            _monsterP = MonsterPriority.Bat;

        _heartManager.ClearHeart();
        _heartManager.CreateEmptyHeart(_maxHP);
        _heartManager.DrawHearts(_nowHp);
    }

    void InitMonsterStat()
    {
        _isMoving = false;
        _isHitable = true;
        _myBeat = 0;
        _nowHp = _maxHP;
        _heartManager.ClearHeart();
        _heartManager.CreateEmptyHeart(_maxHP);
        _heartManager.DrawHearts(_nowHp);
    }
    void OnBeat()
    {
        if (_isMoving || _playerController._isInShop || _playerController._isDead || IngameManager._instance._gameEnd) return;
        _isAttack = true;
        _myBeat += 1;
        if(_myBeat > 4) _myBeat = 1;

        if (_path != null && _path.Count > 1)
        {
            ReleaseReservation(_path[1]);
        }

        _startNode = _tileManager.NodeFromWorldPos(transform.position);
        _randomNode = GetRandomNode(null);

        _startNode._walkable = false;

        _path = _pFinder.FindPath(_startNode._worldPosition, _randomNode._worldPosition, this);

        if (_path == null)
        {
            // 예약 무시한 순수 A* 다시 시도
            _path = _pFinder.FindPath(_startNode._worldPosition, _randomNode._worldPosition, null);
        }

        if (_path == null || _path.Count < 2)
            return;

        Node nextNode = _path[1];

        if (!CanReserve(nextNode))
        {
            _randomNode = GetRandomNode(nextNode);

            // 우회 경로 재탐색
            _path = _pFinder.FindPath(_startNode._worldPosition, _randomNode._worldPosition, this);

            if (_path == null || _path.Count < 2)
                return;

            nextNode = _path[1];

            if (!CanReserve(nextNode))
                return;
        }

        nextNode._reservedBy = this;

        _tileManager.SetDebugPath(gameObject.name, _path);   

        _animController.SetTrigger(_myBeat + "Beat");

        if (_myBeat == 2 || _myBeat == 4)
        {           
            StartCoroutine(MoveToNode(_path[1]));

        }
    }

    public void OnHitting(float dmg)
    {
        if (!_isHitable) return;

        if ((_nowHp -= dmg) <= 0)
        {
            _nowHp = 0;
            StartCoroutine(HitCoolDown());

            if (_path != null && _path.Count > 1)
            {
                ReleaseReservation(_path[1]);
                _startNode._walkable = true;
            }

            _dead = true;
            IngameManager._instance.KillCount();
            SpawnGold(_gold);

            if (_name == "DireBat")
            {
                SoundManager._instance.PlaySFX(SFXName.Direbat_death);
                IngameManager._instance.UpgradeMonster();
                IngameManager._instance.BossCount();
                ObjectPool._instance._direBatQueue.Enqueue(gameObject);
            }
            else
            {
                SoundManager._instance.PlaySFX(SFXName.Bat_death);
                ObjectPool._instance._batQueue.Enqueue(gameObject);
            }
            
            
            gameObject.SetActive(false);
        }
        else
        {
            if (_name == "DireBat")
            {
                int rnd = Random.Range((int)SFXName.Direbat_hit_01, (int)SFXName.Direbat_hit_03 + 1);
                SoundManager._instance.PlaySFX((SFXName)rnd);
            }
            else
                SoundManager._instance.PlaySFX(SFXName.Bat_hit);
            StartCoroutine(HitCoolDown());
            _heartManager.DrawHearts(_nowHp);
            Debug.Log(_nowHp);
        }
    }

    protected override void CheckPlayerinRange()
    {
        base.CheckPlayerinRange();
    }

    IEnumerator MoveToNode(Node nextNode)
    {
        _isMoving = true;
        Vector3 origin = transform.position;
        Vector3 targetPos = nextNode._worldPosition;
        
        Vector3 dir = (nextNode._worldPosition - origin).normalized;

        Node currentNode = _tileManager.NodeFromWorldPos(transform.position);
        _startNode._walkable = true;
        ReleaseReservation(currentNode);

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
            Vector3 offset = dir * move;

            transform.position = origin + offset;

            yield return null;
        }
    }

    Node GetRandomNode(Node reserved)
    {
        Vector2Int[] arr = dir;
        Node rndNode;
        while (true)
        {
            int rnd = Random.Range(0, arr.Length);

            Vector3 rndPos = new Vector3(arr[rnd].x + transform.position.x, arr[rnd].y + transform.position.y, 0);

            rndNode = _tileManager.NodeFromWorldPos(rndPos);

            if (rndNode._walkable && reserved != rndNode)
                break;
        }
       return rndNode;

    }

    IEnumerator HitCoolDown()
    {
        _isHitable = false;

        yield return new WaitForSeconds(0.15f);

        _isHitable = true;
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
