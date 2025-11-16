using System;
using UnityEngine;

public class NoteManager : MonoBehaviour
{

    static NoteManager _uniqueInstance;

    [SerializeField] int _bpm = 0;
    double _currentTime = 0;

    [SerializeField] Transform _tfNoteAppearLeft;
    [SerializeField] Transform _tfNoteAppearRight;

    public event Action OnBeat;

    public static NoteManager _instance
        { get { return _uniqueInstance; } }

    private void Awake()
    {
        _uniqueInstance = this;
    }

    void Update()
    {
        _currentTime += Time.deltaTime;
        //60(1분) / _bpm을 하여 1beat를 계산

        if (_bpm <= 0) return;

        if (_currentTime >= 60d / _bpm) 
        {
            GameObject goLeft = ObjectPool._instance._leftNoteQueue.Dequeue();
            goLeft.transform.position = _tfNoteAppearLeft.position;
            goLeft.SetActive(true);
           
            TimingManager.Instance._boxNoteListL.Add(goLeft);


            GameObject goRight = ObjectPool._instance._rightNoteQueue.Dequeue();
            goRight.transform.position = _tfNoteAppearRight.position;
            goRight.SetActive(true);
            
            TimingManager.Instance._boxNoteListR.Add(goRight);
            _currentTime -= 60d / _bpm;

            OnBeat?.Invoke();
            
        }
    }
}
