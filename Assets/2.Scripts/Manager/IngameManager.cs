using UnityEngine;
using DefineEnum;
using System.Collections;

public class IngameManager : MonoBehaviour
{
    static IngameManager _uniqueInstance;

    int _myBeat;
    bool _isPlayingBGM;

    public static IngameManager _instance { get { return _instance; } }

    private void Awake()
    {
        _uniqueInstance = this;
        NoteManager._instance.OnBeat += OnBeat;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _myBeat = 0;
        _isPlayingBGM = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnBeat()
    {
        _myBeat++;
        Debug.Log(_myBeat);
        if (_myBeat == 4 && !_isPlayingBGM)
        {
            StartCoroutine(DelayMusic());
            Debug.Log("노래시작");
        }
        else if (_myBeat > 4)
        {
            _myBeat = 1;
        }
    }

    IEnumerator DelayMusic()
    {
        yield return new WaitForSeconds(0.08f);
        SoundManager._instance.PlayBGM(BGMName.Disco_Descent);
        SoundManager._instance.PlayBanshee();
        _isPlayingBGM = true;
    }
}
