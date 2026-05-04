using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    private void Awake()
    {
        GameTableManager._instance.AllLoadTable();
        SoundManager._instance.LoadAllSound();
        //DataManger._instance.LoadData();
    }

    private void Start()
    {
        SceneControlManager._instance.StartInGame();
    }
}
