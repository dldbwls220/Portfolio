using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [SerializeField] Text _time;

    public void SetTime(float time)
    {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);

        _time.text = $"{minutes:00}:{seconds:00}";
      
    }
}
