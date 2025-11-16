using DefineEnum;
using System.Collections;
using UnityEngine;

public class Skeleton : CharBase
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpHeight = 0.5f;
    [SerializeField] Transform _characterPos;

    BoxCollider2D _collider;
    Animator _aniController;
    [SerializeField] GameObject _target; // юс╫ц

    Vector3 _originPos;

    int _nowBeat;
    float _baseY = 0;

    private void Start()
    {
        InitSet(3);
        NoteManager._instance.OnBeat += OnBeatSkeleton;
        _collider = GetComponent<BoxCollider2D>();
    }

    void OnEnable()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        if (_nowBeat == _beat)
        {
            transform.position = Vector3.MoveTowards(transform.position, (_originPos + Vector3.up), _moveSpeed * Time.deltaTime);
            StartCoroutine(MoveJump());
        }
        else if (_nowBeat == _beat + _beat)
        {
            transform.position = Vector3.MoveTowards(transform.position, (_originPos), 5 * Time.deltaTime);
            StartCoroutine(MoveJump());
        }
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

        _originPos = transform.position;
        _aniController = GetComponent<Animator>();
        _aniController.speed = ((1f / 60f) * 115f);
        _nowBeat = 0;
    }

    void CheckTargetLocation()
    {

    }

    void OnBeatSkeleton()
    {
        _nowBeat += 1;
        if (_nowBeat > 4)
            _nowBeat = 1;

        _aniController.SetTrigger(_nowBeat + "Beat");

        Debug.Log(_nowBeat);
    }

    IEnumerator MoveJump()
    {
        float t = 0;
        float moveTime = 1 / _moveSpeed;
        _collider.enabled = false;
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
        _collider.enabled = true;
    }
}
