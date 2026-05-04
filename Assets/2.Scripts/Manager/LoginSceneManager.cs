using UnityEngine;

public class LoginSceneManager : MonoBehaviour
{
    [SerializeField] LoginUI _ui;
   
    void Start()
    {        
        _ui.CloseSignUpWnd();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
