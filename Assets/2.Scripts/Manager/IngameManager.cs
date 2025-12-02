using UnityEngine;
using DefineEnum;
using System.Collections;

public class IngameManager : MonoBehaviour
{
    static IngameManager _uniqueInstance;

    [SerializeField] MusicSelectBox _musicSelectBox;
    [SerializeField] SpawnMonsterManager _spawnMonsterManager;
    [SerializeField] NumberUI _numberUI;
    [SerializeField] PauseGame _pause;
    [SerializeField] ResultUI _resultUI;
    [SerializeField] PlayerController _playerController;
    [SerializeField] TimeUI _timeUI;
    [SerializeField] int _totalBossToKill = 3;
    [SerializeField] float _spawnBossTime;

    int _musicIndex;
    int _myBeat;
    int _comboCount;
    int _killCount;
    int _bossKillCount;
    int _bpm;
    float _gameTime;
    double _startDSPTime;
    bool _isPlayingBGM;
    bool _isSelected;
    bool _isGameEnd;
    
    [SerializeField] bool _isComboBonus;


    public bool _isCombo { get { return _isComboBonus; } }

    public bool _isPaused { get; set; }
    public bool _bansheeSound { get; set; }

    public bool _isStartMusic { get { return _isSelected; } }

    public bool _gameEnd { get { return _isGameEnd; } }

    public int _comboNum {  get { return _comboCount; } }

    public int _monsterKillCount { get { return _killCount; } }
    public int _myBPM { get { return _bpm; } }
    public int _myMusicIndex { get { return _musicIndex; } }
    public double _dpsTime { get { return _startDSPTime; } }

    public static IngameManager _instance { get { return _uniqueInstance; } }

    private void Awake()
    {
        _uniqueInstance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NoteManager._instance.OnBeat += OnBeat;
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isSelected) return;

        if (!_isGameEnd)
            _gameTime += Time.deltaTime;

        _timeUI.SetTime(_gameTime);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                _pause.UnpauseThisGame();
            }
            else
            {
                _pause.PauseThisGame();
                _isPaused = true;
            }
        }

        if (_bossKillCount == _totalBossToKill)
        {
            _isGameEnd = true;
            SoundManager._instance._bansheeDESC._mute = true;
            StartCoroutine(OpenResult());
        }
        else if(_playerController._isDead)
        {
            _isGameEnd = true;
            SoundManager._instance._bansheeDESC._mute = true;
            StartCoroutine(OpenResult());
        }

    }

    public void InitGame()
    {
        _myBeat = 0;
        _comboCount = 0;
        _killCount = 0;
        _bossKillCount = 0;
        _gameTime = 0;
        _bpm = 0;
        _isPlayingBGM = false;
        _isSelected = false;
        _isComboBonus = false;
        _isGameEnd = false;
        _isPaused = false;
        _bansheeSound = false;
        _musicSelectBox.InitWnd();
        _startDSPTime = AudioSettings.dspTime + 0.1f;

        _pause.CloseWnd();
        _resultUI.CloseWnd();

    }

    public void SetMusic(int index, int bpm)
    {
        _musicIndex = index;
        _isSelected = true;
        _bpm = bpm; 
        _playerController.SetWaitAttackTime(_bpm);
        NoteManager._instance.InitNote(bpm);
        ObjectPool._instance.InitPool();
        ShopGateManager._instance.InitShopGate();
        SpawnItemManager._instance.initSpawn();
        SpawnMonsterManager._instance.InitSpawn();
    }

    public void UpgradeMonster()
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
        ++_comboCount;
        if (_comboCount == 2)
        {
            SoundManager._instance.PlaySFX(SFXName.sfx_chain_groove_ST);
            _isComboBonus = true;
        }
        if( _comboCount > 3)
            _comboCount = 3;
    }

    public void ResetCombo()
    {
        if(_gameEnd) return;

        if (_comboCount > 1)
            SoundManager._instance.PlaySFX(SFXName.sfx_chain_break_ST);

        _comboCount = 0;
        _isComboBonus = false;       
    }

    public void KillCount()
    {
        _killCount++;
        ComboCountUp();
        _numberUI.KillCountUI(_killCount);
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
            StartCoroutine(SpawnBoss());
        }
        else if (_myBeat > 4)
        {
            _myBeat = 1;
        }
    }

    IEnumerator DelayMusic()
    {
        yield return null;
        
        SoundManager._instance.PlayLoop((LoopName)(_musicIndex - 1), _startDSPTime);
        SoundManager._instance.PlayShop((ShopkeeperName)(_musicIndex - 1), _startDSPTime);
        SoundManager._instance._loopDESC._volum = 0.5f;
        SoundManager._instance._shopkeeperDESC._volum = 0.5f;
        SoundManager._instance.PlayBanshee();
        _isPlayingBGM = true;
    }

    IEnumerator SpawnBoss()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnBossTime);
            _spawnMonsterManager.SpawnBoss();
        }
    }

    IEnumerator OpenResult()
    {
        yield return new WaitForSeconds(0.5f);
        if (!_playerController._isDead)
            _resultUI.SetResult("Clear!!!", _gameTime, _playerController._goldContain, _monsterKillCount);
        else
            _resultUI.SetResult("Loose...", _gameTime, _playerController._goldContain, _monsterKillCount);

    }
}
