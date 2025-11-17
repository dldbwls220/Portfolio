using DefineEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DummyMonster : MonoBehaviour
{
    public PathFinding pathfinder;
    public TileMapGridManager grid;
    public Transform target;
    public float moveSpeed = 5f;
    public Transform _characterPos;

    List<Node> path;
    Node startNode;
    Node targetNode;
    int _myBeat = 0;
    bool isMoving = false;

    Animator _anim;

    void Start()
    {
        NoteManager._instance.OnBeat += OnBeat;
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        targetNode = grid.NodeFromWorldPos(target.position);
        path = pathfinder.FindPath(startNode._worldPosition, targetNode._worldPosition);
    }

    void OnBeat()
    {
        if (isMoving) return;

        _myBeat += 1;
        if (_myBeat > 4)
            _myBeat = 1;

        // 현재 몬스터 위치 → 타일 노드
        startNode = grid.NodeFromWorldPos(transform.position);

        // 플레이어 위치 → 타일 노드
        

        // A*로 전체 경로 생성
        

        if (_myBeat == 1 || _myBeat == 3)
        {
            // 경로가 있고 1칸 이상이라면 다음 칸으로 이동

            if (path != null && path.Count == 2)
            {
                Debug.Log("공격!");
            }
            else if (path != null && path.Count > 1)
            {                
                StartCoroutine(MoveToNode(path[1]));  // path[0]은 startNode 이므로 path[1]이 다음 칸
            }
            
        }

        _anim.SetTrigger(_myBeat + "Beat");
      
    }

    IEnumerator MoveToNode(Node nextNode)
    {
        isMoving = true;
        StartCoroutine(MoveJump());
        Vector3 targetPos = nextNode._worldPosition;
        targetPos.z = transform.position.z;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;
    }

    //IEnumerator FrontBack()
    //{
    //    float t = 0;
    //    float moveTime = 1 / 5;
    //    float move = 0;

    //    while (t < 1f)
    //    {
    //        t += Time.deltaTime / moveTime;

    //        move = Mathf.Sin(t * Mathf.PI) * 0.5f;

    //        Vector3 charPos = transform.position;

    //        charPos.x = _originPos.x + move;
    //        charPos.y = _originPos.y + move;

    //        transform.position = charPos;

    //        yield return null;
    //    }
    //}

    IEnumerator MoveJump()
    {
        float t = 0;
        float moveTime = 1 / moveSpeed;

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
}
