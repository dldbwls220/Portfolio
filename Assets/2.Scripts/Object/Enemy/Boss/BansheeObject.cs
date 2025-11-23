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
    [SerializeField] GameObject _defaultSpriteObj;
    [SerializeField] GameObject _angrySpriteObj;

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

    void Start()
    {
        NoteManager._instance.OnBeat += OnBeat;
        InitMonster(6);
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

        _playerController = _targetTF.GetComponent<PlayerController>();
        _anim = GetComponent<Animator>();
        _anim.speed = (115f / 60f);
        _isAttack = false;
        _isDamaged = false;
        _isAngry = false;
    }


    void OnBeat()
    {
        if (_isMoving) return;

        _myBeat += 1;
        if (_myBeat > 4)
            _myBeat = 1;

        if (_path != null)
            SetTile(_path[1], true);

        _startNode = _tileManager.NodeFromWorldPos(transform.position);

        SetMovementCost(5, true);

        _targetNode = _tileManager.NodeFromWorldPos(_targetTF.position);
        _path = _pFinder.FindPath(_startNode._worldPosition, _targetNode._worldPosition);

        SetTile(_path[1], false);
        ReserveNextTile();

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
                StartCoroutine(Attack(_path[1], 0.1f));
        }
        else if (_path != null && _path.Count == 2)       
        {
            if (!_isAttack)
                StartCoroutine(Attack(_path[1], 0.1f));
        }
        //else if (isOtherReserved(_path[1]) && _path[1]._walkable)
        //{
        //    Debug.Log("지나가지 못함");
        //    return;
        //}
        else if (_isDamaged)
        {
            StopAllCoroutines();
            StartCoroutine(KnockBack());
            SetTile(_path[1], true);
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

            SoundManager._instance._bgmDESC._volum = 1;
            SoundManager._instance._bansheeDESC._volum = 0;

            _isDamaged = false;
            _dead = true;
        }
        else
        {
            _isDamaged = true;

            StartCoroutine(KnockBack());

            int rnd = Random.Range((int)SFXName.Banshee_hurt_01, (int)SFXName.Banshee_hurt_03 + 1);
            SoundManager._instance.PlaySFX((SFXName)rnd);

            SoundManager._instance._bgmDESC._volum = 0;
            SoundManager._instance._bansheeDESC._volum = 1;

            _angrySpriteObj.SetActive(true);
            _defaultSpriteObj.SetActive(false);
        }
    }

    IEnumerator MoveToNode(Node nextNode)
    {
        _isMoving = true;

        Vector3 targetPos = nextNode._worldPosition;

        SetTile(nextNode, true);
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
        ReleaseOldTile(nextNode);
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
        //else if (isOtherReserved(nextNode) && nextNode._walkable)
        //{
        //    StartCoroutine(AttackFrontBack(nextNode));
        //}
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

    void SetTile(Node nextNode, bool isWalkable)
    {
        _startNode._walkable = isWalkable;
    }
    void ReserveNextTile()
    {
        if (_path == null || _path.Count < 2)
            return;

        Node nextNode = _path[1];

        if (nextNode._isReserved && nextNode._reserveBy != this)
            return;

        nextNode._isReserved = true;
        nextNode._reserveBy = this;
    }

    void ReleaseOldTile(Node oldNode)
    {
        if (oldNode._reserveBy == this)
        {
            oldNode._isReserved = false;
            oldNode._reserveBy = null;
        }
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
