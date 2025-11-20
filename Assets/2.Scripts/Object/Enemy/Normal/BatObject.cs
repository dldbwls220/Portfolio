using DefineEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BatObject : CharBase
{
    [SerializeField] PathFinding _pFinder;
    [SerializeField] TileMapGridManager _tileManager;
    [SerializeField] GameObject _playerObj;
    [SerializeField] GameObject _spriteObj;

    [SerializeField] float _moveSpeed = 5f;

    Transform _targetTF;
    PlayerController _playerController;



    List<Node> _path;
    Node _startNode;
    Node _targetNode;
    Node _randomNode;


    int _myBeat;
    bool _isMoving;
    bool _isAttack;

    Vector2Int[] dir = new Vector2Int[]
    {
        new Vector2Int (1,0), //오른쪽
        new Vector2Int (-1,0),//왼쪽
        new Vector2Int (0,1), //위
        new Vector2Int (0,-1) //아래
    };

    Animator _animController;

    private void Start()
    {
        NoteManager._instance.OnBeat += OnBeat;
        InitMonster(2);
    }

    private void Update()
    {
        _targetNode = _tileManager.NodeFromWorldPos(_targetTF.position);
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

        
        _animController = GetComponent<Animator>();
        _myBeat = 0;
        _isAttack = false;
        _isMoving = false;
        
    }

    void OnBeat()
    {
        if (_isMoving) return;

        _myBeat += 1;
        if(_myBeat > 4) _myBeat = 1;

        if (_path != null)
            SetTile(_path[1], true, false, 0);

        _startNode = _tileManager.NodeFromWorldPos(transform.position);
        _randomNode = GetRandomNode();
        _path = _pFinder.FindPath(_startNode._worldPosition, _randomNode._worldPosition);
        
        SetTile(_path[1], false, true, 5);
        
        _tileManager.SetDebugPath(gameObject.name, _path);

        if (_path == null || _path.Count <= 1)
            return;
        Node nextNode = _path[1];

        int distance = Mathf.Abs(_startNode._grideX - _targetNode._grideX) + Mathf.Abs(_startNode._grideY - _targetNode._grideY);

        _animController.SetTrigger(_myBeat + "Beat");

        if (_myBeat == 2 || _myBeat == 4)
        {

            if (distance == 2)
            {
                if (!_isAttack)
                    StartCoroutine(Attack(_path[1], 0.1f));
            }
            else if (distance == 1)
            {
                if (!_isAttack)
                    StartCoroutine(Attack(_path[1], 0.15f));

            }
            else if (isOtherReserved(_path[1]) && _path[1]._walkable)
            {
                
                return;
            }
            else
            {
                StartCoroutine(MoveToNode(_path[1]));
            }          
        }   
    }

    public void OnHitting(float dmg)
    {
        if ((_nowHp -= dmg) <= 0)
        {
            _nowHp = 0;
            SoundManager._instance.PlaySFX(SFXName.Bat_death);
            _dead = true;
        }
        else
        {
            if(_name == "DireBat")
                SoundManager._instance.PlaySFX(SFXName.Bat_minibpss_hit);
            else
                SoundManager._instance.PlaySFX(SFXName.Bat_hit);
        }
    }

    void SetTile(Node nextNode, bool isWalkable, bool isResrve, int reserveCost)
    {
        _startNode._walkable = isWalkable;

        nextNode._BatNode = isResrve;
        nextNode._movementCost = reserveCost;
    }

    bool isOtherReserved(Node nextNode)
    {
        if (nextNode._GolemNode == true ||
            nextNode._SlimeNode == true ||
            nextNode._SkeletonNode == true)
            return true;
        else return false;
    }

    IEnumerator MoveToNode(Node nextNode)
    {
        _isMoving = true;
        Vector3 origin = transform.position;
        Vector3 targetPos = nextNode._worldPosition;
        
        Vector3 dir = (nextNode._worldPosition - origin).normalized;

        SetTile(nextNode, true, false, 0);


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

    IEnumerator Attack(Node nextNode, float delay)
    {
        _isAttack = true;

        Node targetAttackNode = nextNode;

        yield return new WaitForSeconds(delay);

        Node playerNowNode = _tileManager.NodeFromWorldPos(_targetTF.position);

        if (playerNowNode == targetAttackNode)
        {
            StartCoroutine(AttackFrontBack(nextNode));
            //Debug.Log("공격 성공! 플레이어가 공격 경로로 들어옴");
            // TODO: 데미지 처리
            SoundManager._instance.PlaySFX(SFXName.Bat_attack);
            _playerController.OnHitting(_strength);
        }
        else if(isOtherReserved(nextNode) && nextNode._walkable)
        {
            StartCoroutine(AttackFrontBack(nextNode));
        }
        else
        {
            //Debug.Log("공격 실패 → 이동");
            StartCoroutine(MoveToNode(nextNode));
        }

        _isAttack = false;
    }

    IEnumerator AttackFrontBack(Node nextNode)
    {
        Vector3 origin = transform.position;

        Vector3 dir = (nextNode._worldPosition - origin).normalized;
        float t = 0;
        float moveTime = 1 / _moveSpeed;

        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;

            
            float move = Mathf.Sin(t * Mathf.PI);   
            Vector3 offset = dir * move * 0.5f;

            transform.position = origin + offset;

            yield return null;
        }

        
        transform.position = origin;
    }

    Node GetRandomNode()
    {
        Vector2Int[] arr = dir;
        Node rndNode;
        while (true)
        {
            int rnd = Random.Range(0, arr.Length);

            Vector3 rndPos = new Vector3(arr[rnd].x + transform.position.x, arr[rnd].y + transform.position.y, 0);

            rndNode = _tileManager.NodeFromWorldPos(rndPos);

            if (rndNode._walkable)
                break;
        }
       return rndNode;

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
