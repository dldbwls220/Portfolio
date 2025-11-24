using UnityEngine;
using UnityEngine.UI;
using DefineEnum;


public class HeartManager : MonoBehaviour
{
    public Sprite _fullHeart;
    public Sprite _emptyHeart;

    Image _heartImage;

    void Awake()
    {
        _heartImage = GetComponent<Image>();
    }

    public void SetHeartImage(HeartSatus status)
    {
        switch (status)
        {
            case HeartSatus.Empty:
                _heartImage.sprite = _emptyHeart;
                break;
            case HeartSatus.Full:
                _heartImage.sprite= _fullHeart;
                break;
        }
    }
}
