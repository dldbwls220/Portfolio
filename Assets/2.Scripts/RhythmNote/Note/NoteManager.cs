using UnityEngine;

public class NoteManager : MonoBehaviour
{

    [SerializeField] int _bpm = 0;
    double _currentTime = 0;

    [SerializeField] Transform _tfNoteAppearLeft;
    [SerializeField] Transform _tfNoteAppearRight;
    [SerializeField] GameObject _goNoteLeft;
    [SerializeField] GameObject _goNoteRight;

    void Update()
    {
        _currentTime += Time.deltaTime;
        //60(1분) / _bpm을 하여 1beat를 계산
        if (_currentTime >= 60d / _bpm) 
        {
            GameObject goLeft = Instantiate(_goNoteLeft, _tfNoteAppearLeft.position, Quaternion.identity);
            goLeft.transform.SetParent(transform);
            TimingManager.Instance._boxNoteListL.Add(goLeft);
            GameObject goRight = Instantiate(_goNoteRight, _tfNoteAppearRight.position, Quaternion.identity);
            goRight.transform.SetParent(transform);
            TimingManager.Instance._boxNoteListR.Add(goRight);
            _currentTime -= 60d / _bpm;
        }
    }

    
}
