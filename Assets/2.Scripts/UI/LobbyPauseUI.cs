using DefineEnum;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPauseUI : MonoBehaviour
{
    [SerializeField] Text _score;
    [SerializeField] Text _extit;
    [SerializeField] Text _resume;
    [SerializeField] ScoreUI _scoreUI;
    public void CloseWnd()
    {
        gameObject.SetActive(false);
    }

    public void PauseThisGame()
    {
        gameObject.SetActive(true);
        ResumeBtnOut();
        Time.timeScale = 0;
        SoundManager._instance._lobbyDESC._volum = 0.2f;
    }

    public void UnpauseThisGame()
    {
        Time.timeScale = 1;
        SoundManager._instance._lobbyDESC._volum = 1f;
        LobbyManager._instance._isPaused = false;
        CloseWnd();
    }

    public void ResetBTNOn()
    {
        SoundManager._instance.PlaySFX(SFXName.sfx_ui_select_up);
        _score.color = Color.cyan;
        _score.fontSize = 100;
    }

    public void ExitBtnOn()
    {
        SoundManager._instance.PlaySFX(SFXName.sfx_ui_select_up);
        _extit.color = Color.cyan;
        _extit.fontSize = 100;
    }

    public void ResetBTNOut()
    {
        _score.color = Color.white;
        _score.fontSize = 80;
    }

    public void ExitBtnOut()
    {
        _extit.color = Color.white;
        _extit.fontSize = 80;
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

    public void OpenScoreWnd()
    {
        _scoreUI.OpenWnd();
        LobbyManager._instance._isLookingScore = true;
    }

    public void CloseScoreWnd()
    {
        _scoreUI.CloseWnd();
        LobbyManager._instance._isLookingScore = false;
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
