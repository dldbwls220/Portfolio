using DefineEnum;
using DefineStructure;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] Text _exitBtn;

    [Header("Monster")]
    [SerializeField] Text _total;
    [SerializeField] Text _sekl;
    [SerializeField] Text _slime;
    [SerializeField] Text _bat;
    [SerializeField] Text _golem;
    [SerializeField] Text _dire;
    [SerializeField] Text _red;
    [SerializeField] Text _banshee;

    public void CloseWnd()
    {
        _exitBtn.color = Color.white;
        _exitBtn.fontSize = 80;
        gameObject.SetActive(false);
    }

    public void OpenWnd()
    {
        gameObject.SetActive(true);
        InitScore();
    }

    public void InitScore()
    {
        TotalData _data = DataManger._instance._totalData;

        Debug.Log(DataManger._instance.GetKillCount(Monsters.Count));

        _total.text = " X " + DataManger._instance.GetKillCount(Monsters.Count).ToString("D3");
        _sekl.text = " X " + DataManger._instance.GetKillCount(Monsters.Skeleton).ToString("D3");
        _slime.text = " X " + DataManger._instance.GetKillCount(Monsters.Slime).ToString("D3");
        _bat.text = " X " + DataManger._instance.GetKillCount(Monsters.Bat).ToString("D3");
        _golem.text = " X " + DataManger._instance.GetKillCount(Monsters.Golem).ToString("D3");
        _dire.text = " X " + DataManger._instance.GetKillCount(Monsters.DireBat).ToString("D3");
        _red.text = " X " + DataManger._instance.GetKillCount(Monsters.RedDragon).ToString("D3");
        _banshee.text = " X " + DataManger._instance.GetKillCount(Monsters.Banshee).ToString("D3");
    }

    public void ExitBtnOn()
    {
        SoundManager._instance.PlaySFX(SFXName.sfx_ui_select_up);
        _exitBtn.color = Color.cyan;
        _exitBtn.fontSize = 100;
    }

    public void ExitBtnOut()
    {
        _exitBtn.color = Color.white;
        _exitBtn.fontSize = 80;
    }

    public void ExitBtnClick()
    {
        CloseWnd();
        LobbyManager._instance._isLookingScore = false;
    }
}
