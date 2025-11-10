using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] float _speed = 120f;
    [SerializeField] Transform _spriteTf;

    Vector3 _stopPos;
    bool _isSpriteStop = false;

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
