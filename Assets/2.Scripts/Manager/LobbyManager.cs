using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    static LobbyManager _uniqueinstance;

    [SerializeField] LobbyUI _lobbyUI;
    [SerializeField] LobbyPauseUI _lobbyPauseUI;

    public bool _isPaused;
    public bool _isLookingScore;
    public static LobbyManager _instance { get { return _uniqueinstance; } }

    private void Awake()
    {
        _uniqueinstance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                if (_isLookingScore)
                {
                    _lobbyPauseUI.CloseScoreWnd();
                }
                else
                    _lobbyPauseUI.UnpauseThisGame();
            }
            else
            {
                _lobbyPauseUI.PauseThisGame();
                _isPaused = true;
            }
        }
    }

    public void InitLobby()
    {
        _isPaused = false;
        _isLookingScore = false;
        _lobbyUI.UpdateDiamondCountUI();
        _lobbyPauseUI.CloseScoreWnd();
        _lobbyPauseUI.CloseWnd();
    }
}
