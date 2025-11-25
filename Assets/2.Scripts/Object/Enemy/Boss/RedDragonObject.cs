using DefineEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDragonObject : CharBase
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

    bool _isFire;
    bool _isAttack;

    void OnEnable()
    {
        NoteManager._instance.OnBeat += OnBeat;
    }

    private void OnDisable()
    {

        NoteManager._instance.OnBeat -= OnBeat;
        if (_isDead)
        {
            InitMonsterStat();
            _dead = false;
        }
    }

    void Update()
    {
        CheckPlayerinRange();

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
        _anim.speed = (115f / 60f);
        _isAttack = false;
        _isFire = false;

        _healthBarManager.ClearHeart();
        _healthBarManager.CreateEmptyHeart(_maxHP);
        _healthBarManager.DrawHearts(_nowHp);
    }

    void InitMonsterStat()
    {
        _nowHp = _maxHP;
        _healthBarManager.DrawHearts(_nowHp);
    }
    void OnBeat()
    {
        if (_isMoving) return;

        _myBeat += 1;
        if (_myBeat > 4)
            _myBeat = 1;

        if (_path != null)
        {
            InitFireLength();
            SetTile(_path[1], true, false, 0);
        }

        _startNode = _tileManager.NodeFromWorldPos(transform.position);

        SetMovementCost(5, true);
        
        _targetNode = _tileManager.NodeFromWorldPos(_targetTF.position);
        _path = _pFinder.FindPath(_startNode._worldPosition, _targetNode._worldPosition);

        SetTile(_path[1], false, true, 5);

        _tileManager.SetDebugPath(gameObject.name, _path);

        _anim.SetTrigger(_myBeat + "Beat");

        switch (_myBeat)
        {
            case 1:
                if(_startNode._worldPosition.y == _targetNode._worldPosition.y && _path.Count > 1 && _path.Count < 9)
                {
                    _isFire = true;
                    _anim.SetBool("isFire", true);
                    //InitFireLength();
                    SoundManager._instance.PlaySFX(SFXName.Dragon_attack_prefire);
                }
                break;
            case 2:

                if (_isFire)
                {
                    SoundManager._instance.PlaySFX(SFXName.Dragon_attack_fire);
                    _fireAnim.SetTrigger("Fire");
                    StartCoroutine(Attack(_path[1], 0.1f));
                }
                else if (_path != null && _path.Count == 3)
                {
                    if (!_isAttack)
                        StartCoroutine(Attack(_path[1], 0.1f));
                }
                else if (_path != null && _path.Count == 2)       //바로 앞에 타겟이 있으면 공격
                {
                    if (!_isAttack)
                        StartCoroutine(Attack(_path[1], 0.1f));
                }
                else if (isOtherReserved(_path[1]) && _path[1]._walkable)
                {
                    _myBeat -= 2;
                    StartCoroutine(MoveJump());
                    return;
                }
                else if (_path != null && _path.Count > 1)   // 경로가 있고 1칸 이상이라면 다음 칸으로 이동
                {
                    StartCoroutine(MoveToNode(_path[1]));   // path[0]은 startNode 이므로 path[1]이 다음 칸
                }
                break;
            case 3:
                _isFire = false;
                _anim.SetBool("isFire", false);
                break;
            case 4:

                if (_path != null && _path.Count == 3)
                {
                    if (!_isAttack)
                        StartCoroutine(Attack(_path[1], 0.1f));
                }
                else if (_path != null && _path.Count == 2)       //바로 앞에 타겟이 있으면 공격
                {
                    if (!_isAttack)
                        StartCoroutine(Attack(_path[1], 0.1f));
                }
                else if (isOtherReserved(_path[1]) && _path[1]._walkable)
                {
                    _myBeat -= 2;
                    StartCoroutine(MoveJump());
                    return;
                }
                else if (_path != null && _path.Count > 1)   // 경로가 있고 1칸 이상이라면 다음 칸으로 이동
                {
                    StartCoroutine(MoveToNode(_path[1]));   // path[0]은 startNode 이므로 path[1]이 다음 칸
                }
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

            _dead = true;
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

        SetTile(nextNode, true, false, 0);
        SetMovementCost(5, false);
               
       
        float diffX = nextNode._worldPosition.x - transform.position.x;

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

        _isMoving = false;
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

            SoundManager._instance.PlaySFX(SFXName.Dragon_attack_melee);
            _playerController.OnHitting(_strength);
        }
        else if(_startNode._worldPosition.y == playerNowNode._worldPosition.y && _isFire)
        {
            _playerController.OnHitting(_strength - 1);
            Debug.Log("불에 맞음");
        }
        else if (isOtherReserved(nextNode) && nextNode._walkable && !_isFire)
        {
            Debug.Log("4번");
            StartCoroutine(AttackFrontBack(nextNode));
        }
        else if(!_isFire)
        {
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
        StartCoroutine(MoveJump());

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
        if (nextNode._GolemNode == true ||
            nextNode._SlimeNode == true ||
            nextNode._BatNode == true)
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
