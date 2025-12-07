using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] GameObject _lodingAnim;
    [SerializeField] Text _startTxt;

    public void CloseLodingAnim()
    {
        _lodingAnim.SetActive(false);
        _startTxt.enabled = true;
    }
}
