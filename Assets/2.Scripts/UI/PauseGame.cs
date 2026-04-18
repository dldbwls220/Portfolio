using UnityEngine;
using UnityEngine.UI;
using DefineEnum;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    [SerializeField] Text _reset;
    [SerializeField] Text _Extit;
    [SerializeField] Text _resume;

    public void CloseWnd()
    {
        gameObject.SetActive(false);
    }

    public void PauseThisGame()
    {
        gameObject.SetActive(true);
        ResumeBtnOut();
        Time.timeScale = 0;
        SoundManager._instance._loopDESC._pause();
        SoundManager._instance._shopkeeperDESC._pause();
    }

    public void UnpauseThisGame()
    {
        Time.timeScale = 1;
        IngameManager._instance.UnpausedspTime();
        SoundManager._instance._loopDESC._unpause();
        SoundManager._instance._shopkeeperDESC._unpause();
        IngameManager._instance._isPaused = false;
        CloseWnd();
    }

    public void ResetBTNOn()
    {
        SoundManager._instance.PlaySFX(SFXName.sfx_ui_select_up);
        _reset.color = Color.cyan;
        _reset.fontSize = 100;
    }

    public void ExitBtnOn()
    {
        SoundManager._instance.PlaySFX(SFXName.sfx_ui_select_up);
        _Extit.color = Color.cyan;
        _Extit.fontSize = 100;
    }

    public void ResetBTNOut()
    {
        _reset.color = Color.white;
        _reset.fontSize = 80;
    }

    public void ExitBtnOut()
    {
        _Extit.color = Color.white;
        _Extit.fontSize = 80;
    }

    public void ResumeBtnOn()
    {
        SoundManager._instance.PlaySFX(SFXName.sfx_ui_select_up);
        _resume.color = Color.cyan;
        _resume.fontSize = 100;
    }

    public void ResumeBtnOut()
    {
        _resume.color = Color.white;
        _resume.fontSize = 80;
    }

    public void ResetGame()
    {
        Time.timeScale = 1;
        SoundManager._instance._loopDESC._unpause();
        SoundManager._instance._shopkeeperDESC._unpause();
        SoundManager._instance._loopDESC._stop();
        SoundManager._instance._shopkeeperDESC._stop();
        SoundManager._instance._sfxDESC._mute = true;
        //SceneControlManager._instance.StartGame();
        SceneControlManager._instance.StartLobby();
    }

    public void BackToLobby()
    {

    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
