using DefineEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemObject : CharBase
{
    [SerializeField] TileMapGridManager _tileManager;
    [SerializeField] PathFinding _pFinder;
    [SerializeField] Transform _targetTF;
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] Transform _characterPos;

    List<Node> _path;
    Node _startNode;
    Node _targetNode;
    int _myBeat = 0;
    bool _isMoving = false;

    Animator _anim;

    bool _isAttack;

    void Start()
    {
        NoteManager._instance.OnBeat += OnBeat;
        InitMonster(4);
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


        _anim = GetComponent<Animator>();
        _anim.speed = (115f / 60f);
        _isAttack = false;
    }


    void OnBeat()
    {
        if (_isMoving) return;

        _myBeat += 1;
        if (_myBeat > 4)
            _myBeat = 1;

        _startNode = _tileManager.NodeFromWorldPos(transform.position);
        _startNode._walkable = false;
        _startNode._movementCost = 3;
        SetMovementCost(5, true);

        _targetNode = _tileManager.NodeFromWorldPos(_targetTF.position);
        _path = _pFinder.FindPath(_startNode._worldPosition, _targetNode._worldPosition);

        _tileManager.SetDebugPath(gameObject.name, _path);


        if (_myBeat == 4)
        {
            if (_path != null && _path.Count == 3)
            {
                if (!_isAttack)
                    StartCoroutine(Attack(_path[1], 0.1f));
            }
            else if (_path != null && _path.Count == 2)       //바로 앞에 타겟이 있으면 공격
            {
                if (!_isAttack)
                    StartCoroutine(Attack(_path[1], 0.15f));
            }
            else if (_path != null && _path.Count > 1)   // 경로가 있고 1칸 이상이라면 다음 칸으로 이동
            {
                StartCoroutine(MoveToNode(_path[1]));   // path[0]은 startNode 이므로 path[1]이 다음 칸
            }
        }


        _anim.SetTrigger(_myBeat + "Beat");

    }

    IEnumerator MoveToNode(Node nextNode)
    {
        _isMoving = true;
        StartCoroutine(MoveJump());
        Vector3 targetPos = nextNode._worldPosition;
        targetPos.z = transform.position.z;

        _startNode._walkable = true;
        _startNode._movementCost = 0;
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
        Node upright = _tileManager.NodeFromWorldPos(_startNode._worldPosition + Vector3.up + Vector3.right);
        Node upleft = _tileManager.NodeFromWorldPos(_startNode._worldPosition + Vector3.up + Vector3.left);
        Node down = _tileManager.NodeFromWorldPos(_startNode._worldPosition + Vector3.down);
        Node downright = _tileManager.NodeFromWorldPos(_startNode._worldPosition + Vector3.down + Vector3.right);
        Node downleft = _tileManager.NodeFromWorldPos(_startNode._worldPosition + Vector3.down + Vector3.left);
        Node left = _tileManager.NodeFromWorldPos(_startNode._worldPosition + Vector3.left);
        Node right = _tileManager.NodeFromWorldPos(_startNode._worldPosition + Vector3.right);

        if (isSet)
        {
            up._movementCost = cost;
            down._movementCost = cost;
            left._movementCost = cost;
            right._movementCost = cost;
            upleft._movementCost = cost;
            upright._movementCost = cost;
            downleft._movementCost = cost;
            downright._movementCost = cost;
        }
        else
        {
            up._movementCost = 0;
            down._movementCost = 0;
            left._movementCost = 0;
            right._movementCost = 0;
            upleft._movementCost = 0;
            upright._movementCost = 0;
            downleft._movementCost = 0;
            downright._movementCost = 0;
        }

    }
}
