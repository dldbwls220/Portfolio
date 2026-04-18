using UnityEngine;
using DefineEnum;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneControlManager : TSingleton<SceneControlManager>
{
    GameScene _scene;

    bool _isMusicLoadEnd = false;
    public bool _isBeginning = true;

    GameObject _lodingUIPrefab;
    TitleUI _titleUI;

    public GameScene _nowScene { get { return _scene; } }

    public bool _musicLoadEnd { get { return _isMusicLoadEnd; } }

    private void Update()
    {
        if (_isMusicLoadEnd && _isBeginning)
        {
            if (Input.anyKey)
            {
                SoundManager._instance.PlayLobby(LoopName.Rhythmortis_lobby);
                _titleUI.CloseLoddingWnd();
                StartCoroutine(StartDelay());
            }
        }
    }

    public void StartInGame()
    {
        _lodingUIPrefab = Resources.Load("UI/TitleImage") as GameObject;
        StartLobby();
    }

    public void StartLobby()
    {
        _scene = GameScene.LobbyScene;
        StartCoroutine(LoddingScene(_scene));
    }

    public void StartGame()
    {
        _scene = GameScene.GamePlayScene;
        StartCoroutine(LoddingScene(_scene));
    }

    IEnumerator LoddingScene(GameScene scene)
    {
        _isMusicLoadEnd = false;
        AsyncOperation aOper;
        _scene = scene;
        if (_titleUI == null)
        {
            GameObject go = Instantiate(_lodingUIPrefab, transform);
            _titleUI = go.transform.GetChild(0).GetComponent<TitleUI>();
        }

        if (!_isBeginning)
        {
            _titleUI.OpenLoddingWnd();
        }

        aOper = SceneManager.LoadSceneAsync(scene.ToString());

        while (!aOper.isDone)
        {
            Debug.Log(aOper.progress);
            yield return null;
        }

        StartCoroutine(LoadingMusic());

    }

    IEnumerator LoadingMusic()
    {
        float progress = 0f;

        yield return SoundManager._instance.LoadAllSound(p =>
        {
            progress = p;
            Debug.Log(progress);
        });

        if (_isBeginning)
        {
            yield return new WaitForSeconds(2);

            _isMusicLoadEnd = true;

            _titleUI.CloseLodingAnim();
        }
        else
        {
            yield return new WaitForSeconds(2);
            Debug.Log("æ¿ ¿Ãµø");
            _titleUI.CloseLoddingWnd();
            SoundManager._instance._sfxDESC._mute = false;

            if (_scene == GameScene.LobbyScene)
                SoundManager._instance.PlayLobby(LoopName.Rhythmortis_lobby);
        }
            
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.1f);
        _isBeginning = false;
    }

}
