using DefineEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BansheeObject : MonsterBase
{
    [SerializeField] PathFinding _pFinder;
    [SerializeField] TileMapGridManager _tileManager;
    [SerializeField] Transform _targetTF;
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] Transform _characterPos;
    [SerializeField] GameObject _heartUI;
    [SerializeField] GameObject _defaultSprite;
    [SerializeField] GameObject _angrySprite;

    List<Node> _path;
    Node _startNode;
    Node _targetNode;
    int _myBeat = 0;
    bool _isMoving = false;


    Animator _anim;

    bool _isAttack;
    bool _isDamaged;
    bool _isAngry;

    PlayerController _playerController;
    HealthBarManager _healthBarManager;

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
        if (_isAngry)
        {
            _defaultSprite.SetActive(false);
            _angrySprite.SetActive(true);
        }
        else
        {
            _defaultSprite.SetActive(true);
            _angrySprite.SetActive(false);
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
        _healthBarManager = _heartUI.GetComponent<HealthBarManager>();

        _playerController = _targetTF.GetComponent<PlayerController>();
        _anim = GetComponent<Animator>();
        _anim.speed = (115f / 60f);
        _isAttack = false;
        _isDamaged = false;
        _isAngry = false;

        _healthBarManager.ClearHeart();
        _healthBarManager.CreateEmptyHeart(_maxHP);
        _healthBarManager.DrawHearts(_nowHp);
    }

    void InitMonsterStat()
    {
        _nowHp = _maxHP;
        _healthBarManager.ClearHeart();
        _healthBarManager.CreateEmptyHeart(_maxHP);
        _healthBarManager.DrawHearts(_nowHp);
    }
    void OnBeat()
    {
        if (_isMoving || _playerController._isDead || _playerController._isInShop || IngameManager._instance._gameEnd) return;

        _myBeat += 1;
        if (_myBeat > 4)
            _myBeat = 1;

        if (_path != null)
            SetTile(_path[1], true, false, 0);

        _startNode = _tileManager.NodeFromWorldPos(transform.position);

        SetMovementCost(5, true);

        _targetNode = _tileManager.NodeFromWorldPos(_targetTF.position);
        _path = _pFinder.FindPath(_startNode._worldPosition, _targetNode._worldPosition);

        SetTile(_path[1], false, true, 5);

        _tileManager.SetDebugPath(gameObject.name, _path);

        _anim.SetTrigger(_myBeat + "Beat");

        if(_isDamaged)
        {
            _isDamaged = false;
            return;
        }
        else if (_path != null && _path.Count == 3)
        {
            if (!_isAttack)
                StartCoroutine(Attack(_path[1], 0.15f));
        }
        else if (_path != null && _path.Count == 2)       
        {
            if (!_isAttack)
                StartCoroutine(Attack(_path[1], 0.15f));
        }
        else if (isOtherReserved(_path[1]) && _path[1]._walkable)
        {
            Debug.Log("지나가지 못함");
            return;
        }
        else if (_isDamaged)
        {
            StopAllCoroutines();
            StartCoroutine(KnockBack());
            SetTile(_path[1], true, false, 0);
        }
        else if (_path != null && _path.Count > 1)   
        {
            StartCoroutine(MoveToNode(_path[1]));   
        }
        
    }

    public void OnHitting(float dmg)
    {
        if ((_nowHp -= dmg) <= 0)
        {
            _nowHp = 0;
            SoundManager._instance.PlaySFX(SFXName.Banshee_death);

            SoundManager._instance._loopDESC._mute = false;
            SoundManager._instance._shopkeeperDESC._mute = false;
            SoundManager._instance._bansheeDESC._volum = 0;
            
            _isAngry = false;
            _dead = true;
            SetTile(_path[1], true, false, 0);
            SpawnGold(_gold);
            IngameManager._instance.KillCount();
            IngameManager._instance.BossCount();
            IngameManager._instance.UpgradeMonster();
            StartCoroutine(Yeah());
            ObjectPool._instance._bansheeQueue.Enqueue(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            _isAngry = true;
            _isDamaged = true;
            StartCoroutine(KnockBack());

            int rnd = Random.Range((int)SFXName.Banshee_hurt_01, (int)SFXName.Banshee_hurt_03 + 1);
            SoundManager._instance.PlaySFX((SFXName)rnd);

            SoundManager._instance._loopDESC._volum = 0;
            SoundManager._instance._shopkeeperDESC._mute = false;
            SoundManager._instance._bansheeDESC._mute = false;

            _healthBarManager.DrawHearts(_nowHp);
        }
    }

    IEnumerator MoveToNode(Node nextNode)
    {
        _isMoving = true;

        Vector3 targetPos = nextNode._worldPosition;

        SetTile(nextNode, true, false, 0);
        SetMovementCost(5, false);

        float diffX = nextNode._worldPosition.x - transform.position.x;

        if (diffX > 0)
            transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
        else if (diffX < 0)
            transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;

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

    IEnumerator KnockBack()
    {
        Vector3 origin = transform.position;
        Vector3 dir = (_targetTF.position - origin).normalized;

        Vector3 targetPos = Vector3.zero;

        if (_playerController._checkDir == LookDir.Up)
        {
            targetPos = Vector3.up + origin;
        }
        else if(_playerController._checkDir == LookDir.Down)
        {
            targetPos = Vector3.down + origin;
        }
        else if (_playerController._checkDir == LookDir.Right)
        {
            targetPos = Vector3.right + origin;
        }
        else if (_playerController._checkDir == LookDir.Left)
        {
            targetPos = Vector3.left + origin;
        }

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
        
    }

    IEnumerator Attack(Node nextNode, float delay)
    {
        _isAttack = true;

        Node targetAttackNode = nextNode;

        yield return new WaitForSeconds(delay);

        Node playerNowNode = _tileManager.NodeFromWorldPos(_targetTF.position);

        if (playerNowNode == targetAttackNode)
        {
            StartCoroutine(AttackFrontBack(nextNode));
            Debug.Log("공격 성공! 플레이어가 공격 경로로 들어옴");
            // TODO: 데미지 처리

            _playerController.OnHitting(_strength);
        }
        else if (isOtherReserved(nextNode) && nextNode._walkable)
        {
            StartCoroutine(AttackFrontBack(nextNode));
        }
        else
        {
            Debug.Log("공격 실패 → 이동");
            StartCoroutine(MoveToNode(nextNode));
        }

        _isAttack = false;
    }

    IEnumerator AttackFrontBack(Node nextNode)
    {
        Vector3 origin = transform.position;
        Vector3 dir = (nextNode._worldPosition - origin).normalized;
        float jumpHeight = 0;


        if (dir == Vector3.down)
            jumpHeight = 1;
        else jumpHeight = 0.5f;
        float t = 0;
        float moveTime = 1 / _moveSpeed;

        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;

            float move = Mathf.Sin(t * Mathf.PI);
            Vector3 offset = dir * move * jumpHeight;

            transform.position = origin + offset;

            yield return null;
        }

        transform.position = origin;
    }

    IEnumerator Yeah()
    {
        yield return new WaitForSeconds(0.6f);
        int rnd = Random.Range((int)SFXName.Cadence_yeah_01, (int)SFXName.Cadence_yeah_05 + 1);

        SoundManager._instance.PlaySFX((SFXName)rnd);
    }
  

    void SetMovementCost(int cost, bool isSet)
    {
        Node up = _tileManager.NodeFromWorldPos(_startNode._worldPosition + Vector3.up);
        Node down = _tileManager.NodeFromWorldPos(_startNode._worldPosition + Vector3.down);
        Node left = _tileManager.NodeFromWorldPos(_startNode._worldPosition + Vector3.left);
        Node right = _tileManager.NodeFromWorldPos(_startNode._worldPosition + Vector3.right);

        if (isSet)
        {
            up._movementCost = cost;
            down._movementCost = cost;
            left._movementCost = cost;
            right._movementCost = cost;
        }
        else
        {
            up._movementCost = 0;
            down._movementCost = 0;
            left._movementCost = 0;
            right._movementCost = 0;
        }

    }

    void SetTile(Node nextNode, bool isWalkable, bool isResrve, int reserveCost)
    {
        _startNode._walkable = isWalkable;

        nextNode._BansheeNode = isResrve;
        nextNode._movementCost = reserveCost;
    }

    bool isOtherReserved(Node nextNode)
    {
        if (nextNode._GolemNode == true ||
             nextNode._SlimeNode == true ||
             nextNode._BatNode == true ||
             nextNode._SkeletonNode == true ||
             nextNode._RedDragonNode == true)
            return true;
        else return false;
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
