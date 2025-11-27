using UnityEngine;
using DefineEnum;
using System.Collections;

public class IngameManager : MonoBehaviour
{
    static IngameManager _uniqueInstance;

    [SerializeField] MusicSelectBox _musicSelectBox;
    [SerializeField] SpawnMonsterManager _spawnMonsterManager;
    [SerializeField] int _totalBossToKill = 3;
    [SerializeField] float _upgradeMonsterTime;

    int _musicIndex;
    int _myBeat;
    int _comboCount;
    int _killCount;
    int _bossKillCount;
    bool _isPlayingBGM;
    bool _isSelected;
    bool _isGameEnd;
    [SerializeField] bool _isComboBonus;


    public bool _isCombo { get { return _isComboBonus; } }

    public bool _isStartMusic { get { return _isSelected; } }

    public bool _gameEnd { get { return _isGameEnd; } }

    public int _comboNum {  get { return _comboCount; } }

    public int _monsterKillCount { get { return _killCount; } }

    public static IngameManager _instance { get { return _uniqueInstance; } }

    private void Awake()
    {
        _uniqueInstance = this;

        NoteManager._instance.OnBeat += OnBeat;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitGame()
    {
        _myBeat = 0;
        _comboCount = 0;
        _killCount = 0;
        _bossKillCount = 0;
        _isPlayingBGM = false;
        _isSelected = false;
        _isComboBonus = false;
        _isGameEnd = false;
        _musicSelectBox.InitWnd();
    }

    public void SetMusic(int index, int bpm)
    {
        NoteManager._instance.InitNote(bpm);
        _musicIndex = index;
        _isSelected = true;
    }

    void UpgradeMonster()
    {
        MonsterBase[] monsters =
         FindObjectsByType<MonsterBase>(FindObjectsSortMode.None);

        foreach (var m in monsters)
        {
            if (m.gameObject)
                m.UpgradeMonster();
        }
    }

    public void ComboCountUp()
    {
        _comboCount++;
        if( _comboCount == 2)
            _isComboBonus = true;
        if( _comboCount > 3)
            _comboCount = 3;
    }

    public void ResetCombo()
    {
        _comboCount = 0;
        _isComboBonus = false;
    }

    public void KillCount()
    {
        _killCount++;
        ComboCountUp();
    }

    public void BossCount()
    {
        _bossKillCount++;
    }

    void OnBeat()
    {
        if (!_isSelected) return;

        _myBeat++;
        
        if (_myBeat == 4 && !_isPlayingBGM)
        {            
            StartCoroutine(DelayMusic());
            StartCoroutine(UpgradeRoutine());
        }
        else if (_myBeat > 4)
        {
            _myBeat = 1;
        }
    }

    IEnumerator DelayMusic()
    {
        yield return new WaitForSeconds(0.08f);
        
        SoundManager._instance.PlayLoop((LoopName)(_musicIndex - 1));
        SoundManager._instance.PlayShop((ShopkeeperName)(_musicIndex - 1));
        SoundManager._instance.PlayBanshee();
        _isPlayingBGM = true;
    }

    IEnumerator UpgradeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_upgradeMonsterTime);
            _spawnMonsterManager.SpawnBoss();
            UpgradeMonster();
        }
    }
}
