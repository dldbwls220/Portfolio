using DefineEnum;
using System.Collections;
using UnityEngine;

public class Bat : CharBase
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] Transform _characterPos;

    [SerializeField] DetectorParent _playerDetector;

    BoxCollider2D _collider;
    Animator _aniController;
    PlayerController _target;

    Vector3 _originPos;
    Vector3 _nextPos;
    LookDir _myDir;
    LookDir _nextDir;

    int _nowBeat;
    bool _isAttack;
    bool _isMove;

    private void Start()
    {
        InitSet(2);
        NoteManager._instance.OnBeat += OnBeat;
        _collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void InitSet(int enemyIndex)
    {
        TableBase table = GameTableManager._instance.Get(TableName.MonsterInfoList);
        string name = table.ToS(enemyIndex, "Name");
        float hp = table.ToF(enemyIndex, "HP");
        int str = table.ToI(enemyIndex, "Strength");
        int gold = table.ToI(enemyIndex, "Gold");
        int beat = table.ToI(enemyIndex, "Beat");

        InitBaseSet(name, str, hp, gold, beat);

        Debug.Log(name);
        Debug.Log(hp);

        _isAttack = false;
        _originPos = transform.position;
        _aniController = GetComponent<Animator>();
        _aniController.speed = ((1f / 60f) * 115f);
        _nowBeat = 0;
        _myDir = LookDir.Down;
    }

    void Move()
    {      
        if (_isAttack) return;

        if (_nowBeat == _beat)
        {
            if (_playerDetector.IsMonsterInDirection(_myDir.ToString()) && _myDir != LookDir.count)
            {
                StartCoroutine(FrontBack());
                Attack();
                _nowBeat = 0;
            }
            else
            {
                StartCoroutine(MonsterMove());
                _myDir = LookDir.count;
            }
        }      

    }

    void SetNextPosition()
    {     
        
        //LookDir dir = new LookDir();
        //do
        //{
        //    int rnd = Random.Range(0, (int)LookDir.count);
        //    dir = (LookDir)rnd;
        //    switch (rnd)
        //    {
        //        case (int)LookDir.Up:
        //            _nextPos = _originPos + Vector3.up;
        //            _myDir = LookDir.Up;
        //            break;
        //        case (int)LookDir.Down:
        //            _nextPos = _originPos + Vector3.down;
        //            _myDir = LookDir.Down;
        //            break;
        //        case (int)LookDir.Left:
        //            _nextPos = _originPos + Vector3.left;
        //            _myDir = LookDir.Left;
        //            break;
        //        case (int)LookDir.Right:
        //            _nextPos = _originPos + Vector3.right;
        //            _myDir = LookDir.Right;
        //            break;
        //    }           
        //}
        //while (_playerDetector.IsWallInDirection(dir.ToString()));

    }

    void Attack()
    {
        Debug.Log("¹ÚÁã°ø°Ý!");
    }

    void OnBeat()
    {
        _nowBeat += 1;
        if (_nowBeat > 2)
            _nowBeat = 1;

        _originPos = transform.position;

        if (_nowBeat == _beat - 1)
            SetNextPosition();

        _aniController.SetTrigger(_nowBeat + "Beat");
    }

    IEnumerator MonsterMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, _nextPos, 5 * Time.deltaTime);
        yield return null;
    }

    IEnumerator FrontBack()
    {
        float t = 0;
        float moveTime = 1 / _moveSpeed;
        float move = 0;

        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;

            if (_myDir == LookDir.Down)
                move = Mathf.Sin(t * Mathf.PI) * -0.5f;
            else if (_myDir == LookDir.Up)
                move = Mathf.Sin(t * Mathf.PI) * 0.5f;
            else if (_myDir == LookDir.Left)
                move = Mathf.Sin(t * Mathf.PI) * -0.5f;
            else if (_myDir == LookDir.Right)
                move = Mathf.Sin(t * Mathf.PI) * 0.5f;

            Vector3 charPos = transform.position;

            if (_myDir == LookDir.Left || _myDir == LookDir.Right)
                charPos.x = _originPos.x + move;
            else
                charPos.y = _originPos.y + move;

            transform.position = charPos;

            yield return null;
        }
    }

}
