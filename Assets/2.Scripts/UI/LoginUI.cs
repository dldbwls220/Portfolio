using DefineEnum;
using Protocols;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] Text _loginBtn;
    [SerializeField] Text _signupBtn;
    [SerializeField] Text _signupWndBtn;
    [SerializeField] Text _exitBtn;

    [Header("ÂüÁ¶ components")]
    [SerializeField] TMP_InputField _inputLoginID;
    [SerializeField] TMP_InputField _inputLoginPW;
   
    [SerializeField] TMP_InputField _inputSignUpID;
    [SerializeField] TMP_InputField _inputSignUpPW;
    [SerializeField] TMP_InputField _inputSignUpName;

    [SerializeField] TMP_Text _checkID;
    [SerializeField] TMP_Text _checkPW;
    [SerializeField] TMP_Text _checkName;

    [SerializeField] Text _errorTxt;

    [SerializeField] GameObject _SignUpWnd;
    [SerializeField] GameObject _LoginWnd;
    [SerializeField] GameObject _errorWnd;

    public void InitWindow()
    {
        _checkID.text = string.Empty;
        _checkPW.text = string.Empty;
        _checkName.text = string.Empty;
    }

    public void ClickLogin()
    {
        Packet_Login pack;
        pack._id = _inputLoginID.text;
        pack._password = _inputLoginPW.text;
        byte[] data = PacketConverter.StructureToByteArray(pack);
        NetManager._instance.SendQueueIn(PacketConverter.CreatePacket(NetManager._instance._uuid, (uint)CLProtocol.Send.Client_Login, data.Length, data));

        StartCoroutine(MoveToLobbyScene());
    }

    public void OpenLoginWnd()
    {
        _LoginWnd.SetActive(true);
    }

    public void CloseLoginWnd()
    {
        _inputLoginID.text = null;
        _inputLoginPW.text = null;
        _LoginWnd.SetActive(false);
    }

    public void LoginBtnOn()
    {
        SoundManager._instance.PlaySFX(SFXName.sfx_ui_select_up);
        _loginBtn.color = Color.cyan;
        _loginBtn.fontSize = 75;
    }

    public void LoginBtnOut()
    {
        _loginBtn.color = Color.white;
        _loginBtn.fontSize = 60;
    }

    public void CloseSignUpWnd()
    {
        _inputSignUpID.text = null;
        _inputSignUpPW.text = null;

        _SignUpWnd.SetActive(false);
        OpenLoginWnd();
    }

    public void OpenSignUpWnd()
    {
        _SignUpWnd.SetActive(true);
        CloseLoginWnd();
    }

    public void SignupBtnOn()
    {
        SoundManager._instance.PlaySFX(SFXName.sfx_ui_select_up);
        _signupBtn.color = Color.cyan;
        _signupBtn.fontSize = 75;
    }

    public void SignupBtnOut()
    {
        _signupBtn.color = Color.white;
        _signupBtn.fontSize = 60;
    }

    public void ClickSignUp()
    {
        Packet_Join pack;
        pack._id = _inputSignUpID.text;
        pack._password = _inputSignUpPW.text;
        pack._userName = _inputSignUpName.text;
        byte[] data = PacketConverter.StructureToByteArray(pack);
        NetManager._instance.SendQueueIn(PacketConverter.CreatePacket(NetManager._instance._uuid, (uint)CLProtocol.Send.Client_Join, data.Length, data));

        CloseSignUpWnd();
    }

    public void SignupWndBtnOn()
    {
        SoundManager._instance.PlaySFX(SFXName.sfx_ui_select_up);
        _signupWndBtn.color = Color.cyan;
        _signupWndBtn.fontSize = 75;
    }

    public void SignupWndBtnOut()
    {
        _signupWndBtn.color = Color.white;
        _signupWndBtn.fontSize = 60;
    }

    public void ExitdBtnOn()
    {
        SoundManager._instance.PlaySFX(SFXName.sfx_ui_select_up);
        _exitBtn.color = Color.cyan;
        _exitBtn.fontSize = 75;
    }

    public void ExitBtnOut()
    {
        _exitBtn.color = Color.white;
        _exitBtn.fontSize = 60;
    }

    public void OpenErrorWnd(string message)
    {
        _errorWnd.SetActive(true);
        _errorTxt.text = message;

        StartCoroutine(CloseErrorWnd());
    }

    IEnumerator CloseErrorWnd()
    {
        yield return new WaitForSeconds(3);

        _errorWnd.SetActive(false);
    }

    IEnumerator MoveToLobbyScene()
    {
        yield return new WaitForSeconds(0.5f);

        SceneControlManager._instance.StartLobby();
    }
}
