using System;
using UnityEngine;

public class NoteManager : MonoBehaviour
{

    static NoteManager _uniqueInstance;

    int _bpm = 0;
    public double startDspTime;      // DSP 시작 시간
    double beatInterval;      // 한 박자 시간
    int lastBeat;

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
        beatInterval = 60.0 / _bpm;

        // 음악이 시작되는 DSP 시간
        startDspTime = IngameManager._instance._dpsTime;

        lastBeat = 0;
    }

    void Update()
    {
        if (!IngameManager._instance._isStartMusic || IngameManager._instance._isPaused)
            return;

        // 오디오 기반 정확한 시간 계산
        double songTime = AudioSettings.dspTime - startDspTime;

        int currentBeat = (int)(songTime / beatInterval);

        if (!_skipFirstBeat)
        {
            lastBeat = currentBeat;
            _skipFirstBeat = true;
            return;
        }

        if (currentBeat != lastBeat)
        {
            lastBeat = currentBeat;

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

