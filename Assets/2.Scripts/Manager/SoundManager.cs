using DefineEnum;
using DefineStructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : TSingleton<SoundManager>
{
    Dictionary<LoopName, AudioClip> _loopClipDoc;
    Dictionary<BGMName, AudioClip> _bgmClipDoc;
    Dictionary<ShopkeeperName, AudioClip> _shopkeeperClipDoc;
    Dictionary<SFXName, AudioClip> _sfxClipDoc;

    public AudioPlayerDESC _loopDESC;
    AudioSource _loopPlayer;
    public AudioPlayerDESC _bgmDESC;
    AudioSource _bgmPlayer;
    public AudioPlayerDESC _shopkeeperDESC;
    AudioSource _shopkeeperPlayer;
    public AudioPlayerDESC _bansheeDESC;
    AudioSource _bansheePlayer;
    public AudioPlayerDESC _sfxDESC;
    AudioSource _sfxPlayer;

    public IEnumerator LoadAllSound(System.Action<float> onProgress = null)
    {
        _loopClipDoc = new Dictionary<LoopName, AudioClip>();
        _bgmClipDoc = new Dictionary<BGMName, AudioClip>();
        _shopkeeperClipDoc = new Dictionary<ShopkeeperName, AudioClip>();
        _sfxClipDoc = new Dictionary<SFXName, AudioClip>();

        _loopPlayer = gameObject.AddComponent<AudioSource>();
        _bgmPlayer = gameObject.AddComponent<AudioSource>();
        _shopkeeperPlayer = gameObject.AddComponent<AudioSource>();
        _bansheePlayer = gameObject.AddComponent<AudioSource>();
        _sfxPlayer = gameObject.AddComponent<AudioSource>();

        _loopDESC = new AudioPlayerDESC(_loopPlayer, 1, false);
        _bgmDESC = new AudioPlayerDESC(_bgmPlayer, 0.6f, false);
        _shopkeeperDESC = new AudioPlayerDESC(_shopkeeperPlayer, 0, false);
        _bansheeDESC = new AudioPlayerDESC(_bansheePlayer, 1, true);
        _sfxDESC = new AudioPlayerDESC(_sfxPlayer, 1, false, false);

        string path = "Sound/";
        int bgmCount = (int)BGMName.Count;
        int sfxCount = (int)SFXName.Count;

        int totalLoad = bgmCount * 3 + sfxCount; // Loop, BGM, Shopkeeper + SFX
        int currentLoad = 0;

        for (int i = 0; i < bgmCount; i++)
        {
            LoopName name = (LoopName)i;

            ResourceRequest request = Resources.LoadAsync<AudioClip>(path + "BGM/" + name);
            yield return request;
            
            _loopClipDoc.Add(name, request.asset as AudioClip);

            currentLoad++;
            onProgress?.Invoke((float)currentLoad / totalLoad);
        }
        for (int i = 0; i < bgmCount; i++)
        {
            BGMName name = (BGMName)i;
            
            ResourceRequest request = Resources.LoadAsync<AudioClip>(path + "BGM/" + name);
            yield return request;
            
            _bgmClipDoc.Add(name, request.asset as AudioClip);

            currentLoad++;
            onProgress?.Invoke((float)currentLoad / totalLoad);
        }
        for (int i = 0; i < bgmCount; i++)
        {
            ShopkeeperName name = (ShopkeeperName)i;

            ResourceRequest request = Resources.LoadAsync<AudioClip>(path + "BGM/" + name);
            yield return request;

            _shopkeeperClipDoc.Add(name, request.asset as AudioClip);

            currentLoad++;
            onProgress?.Invoke((float)currentLoad / totalLoad);
        }

        for (int i = 0; i < sfxCount; i++)
        {
            SFXName name = (SFXName)i;

            ResourceRequest request = Resources.LoadAsync<AudioClip>(path + "SFX/" + name);
            yield return request;

            _sfxClipDoc.Add(name, request.asset as AudioClip);

            currentLoad++;
            onProgress?.Invoke((float)currentLoad / totalLoad);
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

    public void PlayLoop(LoopName name, double dspStartTime)
    {
        if (!_loopClipDoc.ContainsKey(name))
        {
            Debug.LogFormat("{0} AudioClip은 없습니다", name);
            return;
        }
        _loopPlayer.clip = _loopClipDoc[name];
        _loopPlayer.PlayScheduled(dspStartTime);
    }

    public void PlayShop(ShopkeeperName name, double dspStartTime)
    {
        if (!_shopkeeperClipDoc.ContainsKey(name))
        {
            Debug.LogFormat("{0} AudioClip은 없습니다", name);
            return;
        }
        _shopkeeperPlayer.clip = _shopkeeperClipDoc[name];
        _shopkeeperPlayer.PlayScheduled(dspStartTime);
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
        _bansheePlayer.clip = Resources.Load<AudioClip>("Sound/" + "BGM/" + "Banshee_loop");
        _bansheePlayer.Play();
    }
}
