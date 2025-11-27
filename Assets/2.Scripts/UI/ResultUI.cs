using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] Text _isWinText;
    [SerializeField] Text _timeText;
    [SerializeField] Text _goldText;
    [SerializeField] Text _scoreText;
    [SerializeField] Text _reset;
    [SerializeField] Text _Extit;

    public void CloseWnd()
    {
        gameObject.SetActive(false);
    }

    public void SetResult(string result, float time, int gold, int score)
    {
        gameObject.SetActive (true);

        _isWinText.text = result;

        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);

        _timeText.text = $"{minutes:00}:{seconds:00}";

        _goldText.text = "GOLD : " + gold.ToString();

        _scoreText.text = "Kill : " + score.ToString();

        SoundManager._instance._loopDESC._volum = 0.3f;
        SoundManager._instance._shopkeeperDESC._volum = 0f;
    }

    public void ResetGame()
    {
        SoundManager._instance._loopDESC._stop();
        SoundManager._instance._shopkeeperDESC._stop();

        SceneManager.LoadScene("StartScene");
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void ResetBTNOn()
    {
        _reset.color = Color.cyan;
        _reset.fontSize = 100;
    }

    public void ExitBtnOn()
    {
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
}
