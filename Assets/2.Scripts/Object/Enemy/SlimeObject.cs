using DefineEnum;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlimeObject : CharBase
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
    Node _upDownNode;
    LookDir _myDir;
    int _myBeat;
    bool _isMoving;
    bool _isAttack;

    Vector2Int[] dir = new Vector2Int[]
    {
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

        _targetTF = _playerObj.transform;
        _playerController = _targetTF.GetComponent<PlayerController>();


        _animController = GetComponent<Animator>();
        _animController.speed = (115f / 60f);
        _myBeat = 0;
        _isAttack = false;
        _isMoving = false;
        _myDir = LookDir.Down;
    }

    void OnBeat()
    {
        if (_isMoving) return;

        _myBeat += 1;
        if (_myBeat > 4) _myBeat = 1;

        _startNode = _tileManager.NodeFromWorldPos(transform.position);
        _startNode._walkable = false;
        _startNode._movementCost = 5;

        if (_myBeat == 1 || _myBeat == 3)
            _upDownNode = GetUpDownNode();

        _path = _pFinder.FindPath(_startNode._worldPosition, _upDownNode._worldPosition);

        _tileManager.SetDebugPath(gameObject.name, _path);

        if (_path == null || _path.Count <= 1)
            return;
        Node nextNode = _path[1];

        int distance = Mathf.Abs(_startNode._grideX - _targetNode._grideX) + Mathf.Abs(_startNode._grideY - _targetNode._grideY);

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
            else
            {
                StartCoroutine(MoveToNode(_path[1]));
            }
        }


        _animController.SetTrigger(_myBeat + "Beat");
    }

    IEnumerator MoveToNode(Node nextNode)
    {
        _isMoving = true;

        Vector3 targetPos = nextNode._worldPosition;
        targetPos.z = transform.position.z;

        _startNode._walkable = true;


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
            if (_myDir == LookDir.Up)
            {
                _myDir = LookDir.Down;
                _myBeat = 2;
            }
            else if (_myDir == LookDir.Down)
            {
                _myDir = LookDir.Up;
                _myBeat = 0;
            }
                Debug.Log("공격 성공! 플레이어가 공격 경로로 들어옴");
            // TODO: 데미지 처리
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
}
