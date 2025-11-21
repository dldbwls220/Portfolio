using DefineEnum;
using DefineStructure;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SoundManager : TSingleton<SoundManager>
{
    Dictionary<BGMName, AudioClip> _introClipDoc;
    Dictionary<BGMName, AudioClip> _bgmClipDoc;
    Dictionary<BGMName, AudioClip> _shopkeeperClipDoc;
    Dictionary<SFXName, AudioClip> _sfxClipDoc;

    AudioPlayerDESC _introDESC;
    AudioSource _introPlayer;
    public AudioPlayerDESC _bgmDESC;
    AudioSource _bgmPlayer;
    public AudioPlayerDESC _shopkeeperDESC;
    AudioSource _shopkeeperPlayer;
    public AudioPlayerDESC _bansheeDESC;
    AudioSource _bansheePlayer;
    AudioPlayerDESC _sfxDESC;
    AudioSource _sfxPlayer;

    public void LoadAllSound()
    {
        _introClipDoc = new Dictionary<BGMName, AudioClip>();
        _bgmClipDoc = new Dictionary<BGMName, AudioClip>();
        _shopkeeperClipDoc = new Dictionary<BGMName, AudioClip>();
        _sfxClipDoc = new Dictionary<SFXName, AudioClip>();

        _introPlayer = gameObject.AddComponent<AudioSource>();
        _bgmPlayer = gameObject.AddComponent<AudioSource>();
        _shopkeeperPlayer = gameObject.AddComponent<AudioSource>();
        _bansheePlayer = gameObject.AddComponent<AudioSource>();
        _sfxPlayer = gameObject.AddComponent<AudioSource>();

        _introDESC = new AudioPlayerDESC(_introPlayer, 1, false, false);
        _bgmDESC = new AudioPlayerDESC(_bgmPlayer, 1, false);
        _shopkeeperDESC = new AudioPlayerDESC(_shopkeeperPlayer, 1, false);
        _bansheeDESC = new AudioPlayerDESC(_bansheePlayer, 0, false);
        _sfxDESC = new AudioPlayerDESC(_sfxPlayer, 1, false, false);

        string path = "Sound/";
        int count = (int)BGMName.Count;
        //for (int i = 0; i < count; i++)
        //{
        //    BGMName name = (BGMName)i;
        //    AudioClip clip = Resources.Load<AudioClip>(path + "BGM/" + name + "_intro");
        //    _introClipDoc.Add(name, clip);
        //}
        for (int i = 0; i < count; i++)
        {
            BGMName name = (BGMName)i;
            AudioClip clip = Resources.Load<AudioClip>(path + "BGM/" + name);
            _bgmClipDoc.Add(name, clip);
        }
        //for (int i = 0; i < count; i++)
        //{
        //    BGMName name = (BGMName)i;
        //    AudioClip clip = Resources.Load<AudioClip>(path + "BGM/" + name + "_shopkeeper");
        //    _shopkeeperClipDoc.Add(name, clip);
        //}

        count = (int)SFXName.Count;
        for (int i = 0; i < count; i++)
        {
            SFXName name = (SFXName)i;
            AudioClip clip = Resources.Load<AudioClip>(path + "SFX/" + name);
            _sfxClipDoc.Add(name, clip);
        }
    }

    public void PlayBGM(BGMName name)
    {
        if (!_bgmClipDoc.ContainsKey(name))
        {
            Debug.LogFormat("{0} AudioClip은 없습니다", name);
            return;
        }
        _bgmPlayer.clip = _bgmClipDoc[name];
        _bgmPlayer.Play();
    }

    public void PlaySFX(SFXName name)
    {
        //동시에 여러 사운드가 나와야 한다
        if (!_sfxClipDoc.ContainsKey(name))
        {
            Debug.LogFormat("{0} AudioClip은 없습니다", name);
            return;
        }
        _sfxPlayer.PlayOneShot(_sfxClipDoc[name]);
    }

    public void PlayBanshee()
    {
        _bgmPlayer.clip = Resources.Load<AudioClip>("Sound/" + "BGM/" + "Banshee_loop");
        _bgmPlayer.Play();        
    }

}
