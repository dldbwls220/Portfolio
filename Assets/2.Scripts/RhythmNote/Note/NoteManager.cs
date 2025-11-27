using System;
using UnityEngine;

public class NoteManager : MonoBehaviour
{

    static NoteManager _uniqueInstance;

    int _bpm = 0;
    double _currentTime = 0;
    double startTime;
    double beatInterval;
    int lastBeat = -1;

    [SerializeField] Transform _tfNoteAppearLeft;
    [SerializeField] Transform _tfNoteAppearRight;

    public event Action OnBeat;

    public static NoteManager _instance
        { get { return _uniqueInstance; } }

    private void Awake()
    {
        _uniqueInstance = this;
    }

    void Start()
    {
        
       
    }

    public void InitNote(int bpm)
    {
        _bpm = bpm;
        startTime = Time.timeAsDouble;
        beatInterval = 60.0 / _bpm;
    }

    void Update()
    {
        if (!IngameManager._instance._isStartMusic)
            return;

        double elapsed = Time.timeAsDouble - startTime;

        int currentBeat = (int)(elapsed / beatInterval);

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
