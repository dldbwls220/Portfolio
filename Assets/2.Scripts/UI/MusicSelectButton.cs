using UnityEngine;
using UnityEngine.UI;
using DefineEnum;
using System.Collections;

public class MusicSelectButton : MonoBehaviour
{
    [SerializeField]Text _musicName;
    [SerializeField]Text _BPM;

    [SerializeField] MusicSelectBox _musicSelectBox;

    string _name;
    int _bpm;
    int _index;

    public string _myName { get { return _name; } }
    public int _myBPM { get { return _bpm; } }

    public int _musicIndex { get { return _index; } }

    public void SetParentBox(MusicSelectBox box)
    {
        _musicSelectBox = box;
    }

    public void InitMusicSelect(string name, int bpm, int index)
    {
        _bpm = bpm;
        _musicName.text = _name = name;
        _BPM.text = _bpm.ToString() + " BPM";
        _index = index;
        SoundManager._instance._bgmDESC._mute = true;
        SoundManager._instance._bgmDESC._volum = 0.6f;
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

        SoundManager._instance._bgmDESC._stop() ;
    }

    IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(0.1f);

        SoundManager._instance.PlayBGM((BGMName)(_musicIndex - 1));
        SoundManager._instance._bgmDESC._mute = false;
    }
}
