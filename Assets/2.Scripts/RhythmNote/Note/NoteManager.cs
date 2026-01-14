using System;
using UnityEngine;

public class NoteManager : MonoBehaviour
{

    static NoteManager _uniqueInstance;

    int _bpm = 0;
    public double _startDspTime;      // DSP 시작 시간
    double _beatInterval;      // 한 박자 시간
    int _lastBeat;

    bool _skipFirstBeat;

    [SerializeField] Transform _tfNoteAppearLeft;
    [SerializeField] Transform _tfNoteAppearRight;

    public event Action OnBeat;

    public static NoteManager _instance
    { get { return _uniqueInstance; } }

    private void Awake()
    {
        _uniqueInstance = this;
    }

    public void InitNote(int bpm)
    {
        _skipFirstBeat = false;

        _bpm = bpm;
        _beatInterval = 60.0 / _bpm;

        // 음악이 시작되는 DSP 시간
        _startDspTime = IngameManager._instance._dpsTime;

        _lastBeat = 0;
    }

    void Update()
    {
        if (!IngameManager._instance._isStartMusic || IngameManager._instance._isPaused)
            return;

        // 오디오 기반 정확한 시간 계산
        double songTime = (AudioSettings.dspTime - _startDspTime) - IngameManager._instance._totalPaused;

        int currentBeat = (int)(songTime / _beatInterval);

        if (!_skipFirstBeat)
        {
<<<<<<< HEAD
            _lastBeat = currentBeat;
            _skipFirstBeat = true;
            return;
=======
            GameObject goLeft = ObjectPool._instance._leftNoteQueue.Dequeue();
            goLeft.transform.position = _tfNoteAppearLeft.position;
            goLeft.SetActive(true);
            //GameObject goLeft = Instantiate(_goNoteLeft, _tfNoteAppearLeft.position, Quaternion.identity);
            //goLeft.transform.SetParent(transform);
            TimingManager.Instance._boxNoteListL.Add(goLeft);


            GameObject goRight = ObjectPool._instance._rightNoteQueue.Dequeue();
            goRight.transform.position = _tfNoteAppearRight.position;
            goRight.SetActive(true);
            //GameObject goRight = Instantiate(_goNoteRight, _tfNoteAppearRight.position, Quaternion.identity);
            //goRight.transform.SetParent(transform);
            TimingManager.Instance._boxNoteListR.Add(goRight);
            _currentTime -= 60d / _bpm;
>>>>>>> parent of 664fc50 (Update NoteManager.cs)
        }

        if (currentBeat != _lastBeat)
        {
            _lastBeat = currentBeat;

            SpawnBeatNotes();
        }      
    }

    void SpawnBeatNotes()
    {
        GameObject goLeft = ObjectPool._instance._leftNoteQueue.Dequeue();
        goLeft.transform.position = _tfNoteAppearLeft.position;
        goLeft.SetActive(true);
        TimingManager.Instance._boxNoteListL.Add(goLeft);

        GameObject goRight = ObjectPool._instance._rightNoteQueue.Dequeue();
        goRight.transform.position = _tfNoteAppearRight.position;
        goRight.SetActive(true);
        TimingManager.Instance._boxNoteListR.Add(goRight);

        OnBeat?.Invoke();
    }
}

