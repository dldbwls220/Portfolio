using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] float _speed = 120f; // 240ÀÌ 60bpm
    [SerializeField] Transform _spriteTf;

    Vector3 _stopPos;
    bool _isSpriteStop = false;

    public bool _isStop { get { return _isSpriteStop; }}

    void OnEnable()
    {
        _isSpriteStop=false;
        _spriteTf.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (CompareTag("LeftNote"))
            transform.localPosition += Vector3.right * _speed * Time.deltaTime;
        else if (CompareTag("RightNote"))
            transform.localPosition += Vector3.left * _speed * Time.deltaTime;

        if (_isSpriteStop)
        {
            _spriteTf.position = _stopPos;
        }

    }

    public void StopSprite()
    {
        _stopPos = _spriteTf.position;
        _isSpriteStop = true;

    }

}
