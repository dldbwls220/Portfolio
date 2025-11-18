using UnityEngine;
using DefineEnum;
using Unity.VisualScripting;
using System.Collections;

public class Slime : CharBase
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpHeight = 0.5f;
    [SerializeField] Transform _characterPos;

    [SerializeField] DetectorParent _playerDetector;

    BoxCollider2D _collider;
    Animator _aniController;
    PlayerController _target;

    Vector3 _originPos;
    LookDir _myDir;

    int _nowBeat;
    float _baseY = 0;
    bool _canAttack;

    public bool _damageable { get { return _canAttack; } }

    private void Start()
    {
        InitSet(1);
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

        _canAttack = true;
        _originPos = transform.position;
        _aniController = GetComponent<Animator>();
        _aniController.speed = ((1f / 60f) * 115f); 
        _nowBeat = 0;
        _myDir = LookDir.Down;
    }

    void Move()
    {

        if (_nowBeat == _beat)
        {
            if (_playerDetector.IsMonsterInDirection("Up")  && _myDir == LookDir.Down)
            {
                Debug.Log("위 공격");
                StartCoroutine(FrontBack());
                StartCoroutine(MoveJump());
                _nowBeat = 4;
            }
            else
            {
                Debug.Log("위 이동");
                transform.position = Vector3.MoveTowards(transform.position, (_originPos + Vector3.up), _moveSpeed * Time.deltaTime);
                StartCoroutine(MoveJump());
                _myDir = LookDir.Up;
            }
           
        }
        else if (_nowBeat == _beat + _beat)
        {
            if (_playerDetector.IsMonsterInDirection("Down")  && _myDir == LookDir.Up)
            {
                Debug.Log("아래 공격");
                StartCoroutine(FrontBack());
                StartCoroutine(MoveJump());
                _nowBeat = 2;
            }
            else
            {
                Debug.Log("아래 이동");
                transform.position = Vector3.MoveTowards(transform.position, (_originPos), 5 * Time.deltaTime);
                StartCoroutine(MoveJump());
                
                _myDir = LookDir.Down;
            }
        }
                   
    }

    void OnBeat()
    {
        _nowBeat += 1;
        if (_nowBeat > 4)
            _nowBeat = 1;

        _aniController.SetTrigger(_nowBeat + "Beat");              
    }

    IEnumerator MoveJump()
    {
        float t = 0;
        float moveTime = 1 / _moveSpeed;

        while (t < 1f)
        {           
            t += Time.deltaTime / moveTime;
            float height = Mathf.Sin(t * Mathf.PI) * _jumpHeight;
            Vector3 charPos = _characterPos.localPosition;
            charPos.y = _baseY + height;
            _characterPos.localPosition = charPos;
            yield return null;
        }
        _characterPos.localPosition = new Vector3(_characterPos.localPosition.x, _baseY, _characterPos.localPosition.z);
        
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
            {
                move = Mathf.Sin(t * Mathf.PI) * 0.5f;
                Vector3 charPos = transform.position;
                charPos.y = _originPos.y + move;
                transform.position = charPos;
            }
            else
            {
                move = Mathf.Sin(t * Mathf.PI) * (-1f);
                Vector3 charPos = transform.position;
                charPos.y = (_originPos.y + 1) + move;
                transform.position = charPos;
            }
            
           
            yield return null;
        }
        _characterPos.localPosition = new Vector3(_characterPos.localPosition.x, _baseY, _characterPos.localPosition.z);
        
    }


}
