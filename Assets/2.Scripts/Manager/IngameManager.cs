using UnityEngine;
using DefineEnum;
using System.Collections;

public class IngameManager : MonoBehaviour
{
    static IngameManager _uniqueInstance;

    [SerializeField]MusicSelectBox _musicSelectBox;

    int _musicIndex;
    int _myBeat;
    bool _isPlayingBGM;
    bool _isSelected;

    public bool _isStartMusic { get { return _isSelected; } }

    public static IngameManager _instance { get { return _uniqueInstance; } }

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
        _isSelected = false;
        _musicSelectBox.InitWnd();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMusic(int index, int bpm)
    {
        NoteManager._instance.InitNote(bpm);
        _musicIndex = index;
        _isSelected = true;
    }

    void OnBeat()
    {
        if (!_isSelected) return;

        _myBeat++;
        
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
        
        SoundManager._instance.PlayLoop((LoopName)(_musicIndex - 1));
        SoundManager._instance.PlayShop((ShopkeeperName)(_musicIndex - 1));
        SoundManager._instance.PlayBanshee();
        _isPlayingBGM = true;
    }
}
