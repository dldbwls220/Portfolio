using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] GameObject _lodingAnim;
    [SerializeField] Text _startTxt;
    [SerializeField] Image _titleImage;

    public void CloseLodingAnim()
    {
        _lodingAnim.SetActive(false);
        _startTxt.enabled = true;
    }

    public void OpenLoddingWnd()
    {
        gameObject.SetActive(true);
        _lodingAnim.SetActive(true);
        _titleImage.color = Color.black;
        _startTxt.enabled = false;
    }

    public void CloseLoddingWnd()
    {
        gameObject.SetActive(false);
    }
}
