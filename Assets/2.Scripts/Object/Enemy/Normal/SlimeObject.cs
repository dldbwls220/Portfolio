using DefineEnum;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlimeObject : MonsterBase
{
    [SerializeField] PathFinding _pFinder;
    [SerializeField] TileMapGridManager _tileManager;
    [SerializeField] GameObject _playerObj;
    [SerializeField] GameObject _spriteObj;
    [SerializeField] GameObject _HeartUI;

    [SerializeField] float _moveSpeed = 5f;

    Transform _targetTF;
    PlayerController _playerController;

    HealthBarManager _healthBarManager;

    List<Node> _path;
    Node _startNode;
    Node _upDownNode;
    LookDir _myDir;
    int _myBeat;
    bool _isMoving;
    bool _isAttack;
    bool _isHitable;

    Vector2Int[] dir = new Vector2Int[]
    {
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

        _pFinder = GameObject.Find("GridManager").GetComponent<PathFinding>();
        _tileManager = GameObject.Find("GridManager").GetComponent<TileMapGridManager>();
        _playerObj = GameObject.Find("PlayerCharacter");
        _healthBarManager = _HeartUI.GetComponent<HealthBarManager>();

        _targetTF = _playerObj.transform;
        _playerController = _targetTF.GetComponent<PlayerController>();

        _animController = GetComponent<Animator>();
        _animController.speed = (IngameManager._instance._myBPM / 60f);
        _myBeat = 0;
        _isAttack = false;
        _isMoving = false;
        _isHitable = true;
        _myDir = LookDir.Down;
        _monsterP = MonsterPriority.Slime;

        _healthBarManager.ClearHeart();
        _healthBarManager.CreateEmptyHeart(_maxHP);
        _healthBarManager.DrawHearts(_nowHp);
    }

    void InitMonsterStat()
    {
        _nowHp = _maxHP;
        _myBeat = 0;
        _healthBarManager.ClearHeart();
        _healthBarManager.CreateEmptyHeart(_maxHP);
        _healthBarManager.DrawHearts(_nowHp);
        _isMoving = false;
    }

    void OnBeat()
    {
        if (_isMoving || _playerController._isInShop || _playerController._isDead || IngameManager._instance._gameEnd) return;
        _isAttack = true;
        _isHitable = true;

        _myBeat += 1;
        if (_myBeat > 4) _myBeat = 1;

        if (_path != null && _path.Count > 1)
        {
            ReleaseReservation(_path[1]);
        }

        _startNode = _tileManager.NodeFromWorldPos(transform.position);     

        if (_myBeat == 1 || _myBeat == 3)
            _upDownNode = GetUpDownNode();

        _path = _pFinder.FindPath(_startNode._worldPosition, _upDownNode._worldPosition, this);

        if (_path == null)
        {
            // 예약 무시한 순수 A* 다시 시도
            _path = _pFinder.FindPath(_startNode._worldPosition, _upDownNode._worldPosition, null);
        }

        if (_path == null || _path.Count < 2)
            return;


        Node nextNode = _path[1];

        if (!CanReserve(nextNode))
        {
            if (_myDir == LookDir.Up)
                _myDir = LookDir.Down;
            else
                _myDir = LookDir.Up;
            return;
        }

        nextNode._reservedBy = this;

        _tileManager.SetDebugPath(gameObject.name, _path);

        if (_path == null || _path.Count <= 1)
            return;

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
            IngameManager._instance.KillCount();
            SpawnGold(_gold);
            int rnd = Random.Range((int)SFXName.Slime_death_01, (int)SFXName.Slime_death_03 + 1);
            SoundManager._instance.PlaySFX((SFXName)rnd);

            if (_path != null && _path.Count > 1)
            {
                ReleaseReservation(_path[1]);
                _startNode._walkable = true;
            }
            
            _dead = true;
            ObjectPool._instance._slimeQueue.Enqueue(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            _isHitable = false;
            int rnd = Random.Range((int)SFXName.Slime_hurt_01, (int)SFXName.Slime_hurt_03 + 1);
            SoundManager._instance.PlaySFX((SFXName)rnd);
            _healthBarManager.DrawHearts(_nowHp);
        }
    }

    IEnumerator MoveToNode(Node nextNode)
    {
        _isMoving = true;

        Vector3 targetPos = nextNode._worldPosition;

        Node currentNode = _tileManager.NodeFromWorldPos(transform.position);
        _startNode._walkable = true;
        ReleaseReservation(currentNode);

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
            Vector3 charPos = _spriteObj.transform.localPosition;
            charPos.y = 0 + height;
            _spriteObj.transform.localPosition = charPos;
            yield return null;
        }
        _spriteObj.transform.localPosition = new Vector3(_spriteObj.transform.localPosition.x, 0, _spriteObj.transform.localPosition.z);

    }

    Node GetUpDownNode()
    {
        // 0 => 위 1 => 아래
        Vector2Int[] arr = dir;
        Node nextNode;
        Vector3 nextPos = Vector3.zero;

        if (_myDir == LookDir.Up)
        {
            nextPos = new Vector3(transform.position.x, arr[1].y + transform.position.y, 0);
            _myDir = LookDir.Down;
        }
        else if (_myDir == LookDir.Down)
        {
            nextPos = new Vector3(transform.position.x, arr[0].y + transform.position.y, 0);
            _myDir = LookDir.Up;
        }

        nextNode = _tileManager.NodeFromWorldPos(nextPos);

        return nextNode;

    }

    void DetectAttack()
    {
        Node playerNowNode = _tileManager.NodeFromWorldPos(_targetTF.position);
        Node MonsterNowNode = _tileManager.NodeFromWorldPos(transform.position);

        if (MonsterNowNode == playerNowNode && _isAttack && _playerController._isPlayerAttackable)
        {
            StartCoroutine(AttackFrontBack(_path[1]));
            if (_myDir == LookDir.Up)
                _myDir = LookDir.Down;
            else
                _myDir = LookDir.Up;

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
