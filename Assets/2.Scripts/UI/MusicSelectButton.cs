using UnityEngine;
using UnityEngine.UI;
using DefineEnum;
using System.Collections;

public class MusicSelectButton : MonoBehaviour
{
    [SerializeField]Text _musicName;
    [SerializeField]Text _BPM;

    MusicSelectBox _musicSelectBox;

    string _name;
    int _bpm;
    int _index;

    public string _myName { get { return _name; } }
    public int _myBPM { get { return _bpm; } }

    public int _musicIndex { get { return _index; } }

    private void Awake()
    {
        _musicSelectBox = GameObject.Find("SelectSongUI").GetComponent<MusicSelectBox>();
    }

    public void InitMusicSelect(string name, int bpm, int index)
    {
        _bpm = bpm;
        _musicName.text = _name = name;
        _BPM.text = _bpm.ToString() + " BPM";
        _index = index;
        SoundManager._instance._bgmDESC._mute = true;
    }

    public void selectMusic()
    {
        _musicSelectBox.SetMusic(_myName, _myBPM, _musicIndex);
        SoundManager._instance.PlaySFX(SFXName.sfx_ui_select_up);
    }

    public void PointerOn()
    {
        _musicName.color = Color.cyan;
        _BPM.color = Color.cyan;

        StartCoroutine(StartMusic());
        
    }

    public void PointerOut()
    {
        _musicName.color= Color.white;
        _BPM.color= Color.white;

        SoundManager._instance._bgmDESC._mute = true;
    }

    IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(0.3f);

        SoundManager._instance.PlayBGM((BGMName)(_musicIndex - 1));
        SoundManager._instance._bgmDESC._mute = false;
    }
}
