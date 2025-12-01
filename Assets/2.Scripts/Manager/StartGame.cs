using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    bool _isLoadEnd = false;

    private void Awake()
    {
        GameTableManager._instance.AllLoadTable();
        SoundManager._instance.LoadAllSound();
        //StartCoroutine(Loading());
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadSceneAsync("GamePlayScene");
        }
       
    }

    IEnumerator Loading()
    {
        float progress = 0f;

        yield return SoundLoadAsync._instance.LoadAllSound(p =>
        {
            progress = p;
            Debug.Log(progress);
        });
    }
}
