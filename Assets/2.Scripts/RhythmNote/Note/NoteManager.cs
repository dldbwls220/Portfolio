using DefineEnum;
using System;
using UnityEngine;

public class NoteManager : MonoBehaviour
{

    static NoteManager _uniqueInstance;

    int _bpm = 0;
    public double startDspTime;      // DSP 시작 시간
    double beatInterval;      // 한 박자 시간
    int lastBeat;

    bool _isMusicStart;

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
        _isMusicStart = false;

        _bpm = bpm;
        beatInterval = 60.0 / _bpm;

        // 음악이 시작되는 DSP 시간
        startDspTime = AudioSettings.dspTime + 0.1f;

        lastBeat = -1;
    }

    void Update()
    {
        if (!IngameManager._instance._isStartMusic)
            return;

        // 오디오 기반 정확한 시간 계산
        double songTime = AudioSettings.dspTime - startDspTime;

        int currentBeat = (int)(songTime / beatInterval);

        if (currentBeat != lastBeat)
        {
            lastBeat = currentBeat;

            // 4박자 이전 → 노트만 스폰
            if (currentBeat < 4)
            {
                SpawnBeatNotes();
                return;
            }

            // 딱 4번째 박자 도달 → DSP 음악 재생
            if (currentBeat >= 4)
            {
                double dspMusicStart = startDspTime + beatInterval * 4;

                if (!_isMusicStart)
                {
                    SoundManager._instance.PlayLoop((LoopName)(IngameManager._instance._myMusicIndex - 1), dspMusicStart);
                    _isMusicStart=true;
                }
               

                // 이제부터 노트 스폰 (음악과 정확히 맞아감)
                SpawnBeatNotes();
                return;
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
    }
}
