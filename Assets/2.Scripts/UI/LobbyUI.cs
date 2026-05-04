using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    static LobbyUI _uniqueinstance;

    [SerializeField]Text _diamondTxt;

    public static LobbyUI _instance { get { return _uniqueinstance; } }

    private void Awake()
    {
        _uniqueinstance = this;
    }

    public void UpdateDiamondCountUI()
    {
        int dia = DataManger._instance._totalData._currentDiamond._totalDiamond;

        _diamondTxt.text = " X " + dia.ToString("D3");
    }
}
