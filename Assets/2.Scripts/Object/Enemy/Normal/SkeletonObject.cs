using DefineEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonObject : MonsterBase
{
    [SerializeField] PathFinding _pFinder;
    [SerializeField] TileMapGridManager _tileManager;
    [SerializeField] Transform _targetTF;
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] Transform _characterPos;
    [SerializeField] GameObject _heartUI;

    List<Node> _path;
    Node _startNode;
    Node _targetNode;

    int _myBeat = 0;
    bool _isMoving = false;
    PlayerController _playerController;
    Animator _anim;
    HealthBarManager _healthBarManager;

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
        _targetTF = GameObject.Find("PlayerCharacter").transform;
        _healthBarManager = _heartUI.GetComponent<HealthBarManager>();
        _playerController = _targetTF.GetComponent<PlayerController>();
        _anim = GetComponent<Animator>();
        _anim.speed = (IngameManager._instance._myBPM / 60f);
        

        _healthBarManager.ClearHeart();
        _healthBarManager.CreateEmptyHeart(_maxHP);
        _healthBarManager.DrawHearts(_nowHp);
    }

    void InitMonsterStat()
    {
        _isMoving = false;
        _nowHp = _maxHP;
        _healthBarManager.ClearHeart();
        _healthBarManager.CreateEmptyHeart(_maxHP);
        _healthBarManager.DrawHearts(_currentHp);
    }

    void OnBeat()
    {
        if (_isMoving || _playerController._isInShop || _playerController._isDead || IngameManager._instance._gameEnd) return;

        _isAttack = true;
        _myBeat += 1;
        if (_myBeat > 4)
            _myBeat = 1;

        if(_path != null)
            SetTile(_path[1], true, false, 0);

        _startNode = _tileManager.NodeFromWorldPos(transform.position);        

        _targetNode = _tileManager.NodeFromWorldPos(_targetTF.position);
        _path = _pFinder.FindPath(_startNode._worldPosition, _targetNode._worldPosition);
        
        SetTile(_path[1], false, true, 5);

        _tileManager.SetDebugPath(gameObject.name, _path);

        _anim.SetTrigger(_myBeat + "Beat");

        if (_myBeat == 2 || _myBeat == 4)
        {        
            StartCoroutine(MoveToNode(_path[1]));
        }

        Debug.Log(_myBeat);
    }

    public void OnHitting(float dmg)
    {
        if ((_nowHp -= dmg) <= 0)
        {
            _nowHp = 0;
            SoundManager._instance.PlaySFX(SFXName.Skel_death);
            SetTile(_path[1], true, false, 0);
            _dead = true;
            IngameManager._instance.KillCount();
            SpawnGold(_gold);
            ObjectPool._instance._skeletonQueue.Enqueue(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            int rnd = Random.Range((int)SFXName.Skel_hurt_01, (int)SFXName.Skel_hurt_03 + 1);
            SoundManager._instance.PlaySFX((SFXName)rnd);
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

        nextNode._SkeletonNode = isResrve;
        nextNode._movementCost = reserveCost;
    }

    bool isOtherReserved(Node nextNode)
    {
        if(nextNode._GolemNode == true ||
            nextNode._SlimeNode == true ||
            nextNode._BatNode == true ||
             nextNode._BansheeNode == true ||
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
