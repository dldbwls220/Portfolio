using DefineEnum;
using UnityEngine;

public class SpawnMonsterManager : MonoBehaviour
{
    static SpawnMonsterManager _uniqueinstance;

    PlayerController _playerController;
    [SerializeField] TileMapGridManager _grid;
    [SerializeField] GameObject _enemySpawnObj;
    Transform[] _spawnPositions;

    int _myBeat;

    public static SpawnMonsterManager _instance { get { return _uniqueinstance; } }

    private void Awake()
    {
        _uniqueinstance = this;
    }

    public void InitSpawn()
    {
        NoteManager._instance.OnBeat += OnBeat;
        _spawnPositions = new Transform[_enemySpawnObj.transform.childCount];

        for (int i = 0; i < _enemySpawnObj.transform.childCount; i++)
        {
            _spawnPositions[i] = _enemySpawnObj.transform.GetChild(i).GetComponent<Transform>();
        }

        _playerController = GameObject.Find("PlayerCharacter").GetComponent<PlayerController>();

        _myBeat = 0;
    }

    void OnBeat()
    {
        if (!IngameManager._instance._isStartMusic || _playerController._isInShop) return;

        _myBeat += 1;
        
        if (_myBeat > 4)
            _myBeat = 1;      

        switch (_myBeat)
        {
            case 2:
                if (ObjectPool._instance._slimeQueue.Count > 0)
                {
                    GameObject slime = ObjectPool._instance._slimeQueue.Dequeue();
                    slime.transform.position = SetPostion();
                    slime.SetActive(true);
                }
                if (ObjectPool._instance._skeletonQueue.Count > 0)
                {
                    GameObject skeleton = ObjectPool._instance._skeletonQueue.Dequeue();
                    skeleton.transform.position = SetPostion();
                    skeleton.SetActive(true);
                }
                break;
            case 3:
                if (ObjectPool._instance._batQueue.Count > 0)
                {
                    GameObject bat = ObjectPool._instance._batQueue.Dequeue();
                    bat.transform.position = SetPostion();
                    bat.SetActive(true);
                }
                break;
            case 4:
                if (ObjectPool._instance._golemQueue.Count > 0)
                {
                    GameObject golem = ObjectPool._instance._golemQueue.Dequeue();
                    golem.transform.position = SetPostion();
                    golem.SetActive(true);
                }
                break;
 
        }

    }

    public void SpawnBoss()
    {
        int rndBoss = Random.Range(0, 3);

        switch (rndBoss)
        {
            case 0:
                if (ObjectPool._instance._redDragonQueue.Count > 0)
                {
                    GameObject RDragon = ObjectPool._instance._redDragonQueue.Dequeue();
                    RDragon.transform.position = SetPostion();
                    SoundManager._instance.PlaySFX(SFXName.Dragon_cry);
                    RDragon.SetActive(true);
                }
                break;
            case 1:
                if (ObjectPool._instance._bansheeQueue.Count > 0)
                {
                    GameObject Banshee = ObjectPool._instance._bansheeQueue.Dequeue();
                    Banshee.transform.position = SetPostion();
                    SoundManager._instance.PlaySFX(SFXName.Banshee_cry);
                    Banshee.SetActive(true);
                }
                break;
            case 2:
                if (ObjectPool._instance._direBatQueue.Count > 0)
                {
                    GameObject DireBat = ObjectPool._instance._direBatQueue.Dequeue();
                    DireBat.transform.position = SetPostion();
                    SoundManager._instance.PlaySFX(SFXName.Direbat_cry);
                    DireBat.SetActive(true);
                }
                break;
        }
    }

    Vector3 SetPostion()
    {
        Vector3 pos = Vector3.zero;

        while (true)
        {
            int rnd = Random.Range(0, _spawnPositions.Length);

            pos = _spawnPositions[rnd].position;

            if (_grid.NodeFromWorldPos(pos)._walkable)
            {
                break;
            }
        }

        return pos;
    }
}
