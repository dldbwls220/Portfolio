using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] TitleUI _titleUI;

    bool _isLoadEnd = false;

    private void Awake()
    {
        GameTableManager._instance.AllLoadTable();
        SoundManager._instance.LoadAllSound();
        DataManger._instance.LoadData();
        StartCoroutine(Loading());
    }

    private void Update()
    {
        if (_isLoadEnd)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadSceneAsync("GamePlayScene");
            }
        }  
    }

    IEnumerator Loading()
    {
        float progress = 0f;

        yield return SoundManager._instance.LoadAllSound(p =>
        {
            progress = p;
            Debug.Log(progress);
        });

        yield return new WaitForSeconds(2);
        _titleUI.CloseLodingAnim();
        _isLoadEnd=true;
    }
}
