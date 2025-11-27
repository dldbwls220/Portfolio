using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    private void Awake()
    {
        GameTableManager._instance.AllLoadTable();
        SoundManager._instance.LoadAllSound();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadSceneAsync("GamePlayScene");
        }
    }
}
