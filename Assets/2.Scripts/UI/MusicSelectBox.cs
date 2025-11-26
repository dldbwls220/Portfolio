using DefineEnum;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectBox : MonoBehaviour
{
    [SerializeField] ScrollRect _musicScroll;
    [SerializeField] Text _startBtn;
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
        _rect = _musicScroll.content;
        _isSelected = false;

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

    public void StartPointerOn()
    {
        _startBtn.color = Color.cyan;
    }

    public void StartPointerOut()
    {
        _startBtn.color = Color.white;
    }

    public void StartGame()
    {
        if (!_isSelected) return;

        IngameManager._instance.SetMusic(_selectedIndex, _selectedBPM);
        SoundManager._instance._bgmDESC._mute = true;
        CloseWnd();
    }
}
