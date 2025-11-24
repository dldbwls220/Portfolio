using DefineEnum;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectBox : MonoBehaviour
{
    [SerializeField] ScrollRect _musicScroll;
    [SerializeField] Text _startBtn;
    [SerializeField] Text _demoBtn;
    RectTransform _rect;

    string _selectedName;
    int _selectedIndex;
    int _selectedBPM;

    bool _isSelected;

    public void CloseWnd()
    {
        gameObject.SetActive(false);
    }

    public void InitWnd()
    {
        _isSelected = false;
        _rect = _musicScroll.content;

        TableBase musicTable = GameTableManager._instance.Get(TableName.MusicList);

        for (int i = 1; i <= (int)BGMName.Count; i++)
        {
            string name = musicTable.ToS(i, "Name");
            int bpm = musicTable.ToI(i, "BPM");

            GameObject musicButton = Resources.Load<GameObject>("UI/MusicSelection");
            GameObject go = Instantiate(musicButton, _rect);
            MusicSelectButton msb = go.GetComponent<MusicSelectButton>();
            msb.InitMusicSelect(name, bpm, i);
        }
    }

    public void SetMusic(string name, int bpm, int index)
    {
        _selectedBPM = bpm;
        _selectedIndex = index;
        _selectedName = name;

        _isSelected = true;

        Debug.Log(_selectedBPM);
        Debug.Log(_selectedName);
    }

    public void DemoPointerOn()
    {
        _demoBtn.color = Color.cyan;

    }

    public void StartPointerOn()
    {
        _startBtn.color = Color.cyan;
    }

    public void DemoPointerOut()
    {
        _demoBtn.color = Color.white;
    }

    public void StartPointerOut()
    {
        _startBtn.color = Color.white;
    }

    public void StartDemo()
    {
        Debug.Log("¿Ωæ«Ω√¿€");
        BGMName name = (BGMName)(_selectedIndex-1);
        SoundManager._instance.PlayBGM(name);
    }
}
