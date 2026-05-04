using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] Text _isWinText;
    [SerializeField] Text _timeText;
    [SerializeField] Text _goldText;
    [SerializeField] Text _scoreText;
    [SerializeField] Text _bestScore;
    [SerializeField] Text _bestTime;
    [SerializeField] Text _newRecordK;
    [SerializeField] Text _newRecordT;
    [SerializeField] Text _reset;
    [SerializeField] Text _Extit;

    public void CloseWnd()
    {
        gameObject.SetActive(false);
    }

    public void SetResult(string result, float time, int gold, int score, bool isBestKill = false, bool isBestTime = false)
    {
        gameObject.SetActive (true);

        _isWinText.text = result;

        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);

        _timeText.text = $"{minutes:00}:{seconds:00}";

        _goldText.text = "GOLD : " + gold.ToString();

        _scoreText.text = "Kill : " + score.ToString();

        _bestScore.text = "BEST KILL : " + DataManger._instance._totalData._playerData._bestKillCount.ToString();

        int bestMinutes = (int)(DataManger._instance._totalData._playerData._bestSurviveTime / 60f);
        int bestSeconds = (int)(DataManger._instance._totalData._playerData._bestSurviveTime % 60f);

        _bestTime.text = $"Best Time : {bestMinutes:00}:{bestSeconds:00}";

        if(!isBestKill)
            _newRecordK.enabled = false;
        else
            _newRecordK.enabled = true;

        if(!isBestTime)
            _newRecordT.enabled = false;
        else
            _newRecordT.enabled = true;

        Debug.Log(isBestKill);
        Debug.Log(isBestTime);

        SoundManager._instance._shopkeeperDESC._volum = 0f;
    }

    public void ResetGame()
    {
        SoundManager._instance._loopDESC._stop();
        SoundManager._instance._shopkeeperDESC._stop();
        SoundManager._instance._sfxDESC._mute = true;
        DataManger._instance.SaveData();
        NetManager._instance.UpdateMonsterNDiamondData();

        SceneControlManager._instance.StartGame();
    }

    public void EndGame()
    {
        DataManger._instance.SaveData();
        NetManager._instance.UpdateMonsterNDiamondData();

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
