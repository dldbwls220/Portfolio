using DefineEnum;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectInfo
{
    public GameObject _objPrefab;
    public int _count;
    public Transform _tfPoolParent;
}

[System.Serializable]
public class Monsterinfo
{
    public GameObject _objPrefab;
    public int _count;
    public Monsters MonsterType;
    public Transform _tfPoolParent;
}

[System.Serializable]
public class ItemInfo
{
    public GameObject _objPrefab;
    public Transform _tfPoolParent;
}

public class ObjectPool : MonoBehaviour
{
    [SerializeField] ObjectInfo[] _objInfo;
    [SerializeField] Monsterinfo[] _monsterInfo;

    public Queue<GameObject> _leftNoteQueue;
    public Queue<GameObject> _rightNoteQueue;

    public Queue<GameObject> _slimeQueue;
    public Queue<GameObject> _batQueue;
    public Queue<GameObject> _skeletonQueue;
    public Queue<GameObject> _golemQueue;
    public Queue<GameObject> _redDragonQueue;
    public Queue<GameObject> _bansheeQueue;
    public Queue<GameObject> _direBatQueue;

   static ObjectPool _uniqueInstance;

    public static ObjectPool _instance { get { return _uniqueInstance; } }

    void Awake()
    {
        _uniqueInstance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitPool();
    }

    public void InitPool()
    {
        _leftNoteQueue = new Queue<GameObject>();
        _rightNoteQueue = new Queue<GameObject>();

        _slimeQueue = new Queue<GameObject>();
        _batQueue = new Queue<GameObject>();
        _skeletonQueue = new Queue<GameObject>();
        _golemQueue = new Queue<GameObject>();
        _redDragonQueue = new Queue<GameObject>();
        _bansheeQueue = new Queue<GameObject>();
        _direBatQueue = new Queue<GameObject>();

        _leftNoteQueue = InsertQueue(_objInfo[0]);
        _rightNoteQueue = InsertQueue(_objInfo[1]);

        _slimeQueue = InsertMonsterQueue(_monsterInfo[0]);
        _batQueue = InsertMonsterQueue(_monsterInfo[1]);
        _skeletonQueue = InsertMonsterQueue(_monsterInfo[2]);
        _golemQueue = InsertMonsterQueue(_monsterInfo[3]);
        _redDragonQueue = InsertMonsterQueue(_monsterInfo[4]);
        _bansheeQueue = InsertMonsterQueue(_monsterInfo[5]);
        _direBatQueue = InsertMonsterQueue(_monsterInfo[6]);
    }

    Queue<GameObject> InsertQueue(ObjectInfo objInfo)
    {
        Queue<GameObject> queue = new Queue<GameObject>();
        for (int i = 0; i < objInfo._count; i++)
        {
            GameObject note = Instantiate(objInfo._objPrefab, transform.position, Quaternion.identity);
            note.SetActive(false);
            if (objInfo._tfPoolParent != null)
                note.transform.SetParent(objInfo._tfPoolParent);
            else
                note.transform.SetParent(this.transform);

            queue.Enqueue(note);
        }

        return queue;
    }

    Queue<GameObject> InsertMonsterQueue(Monsterinfo objInfo)
    {
        Queue<GameObject> queue = new Queue<GameObject>();

        for (int i = 0; i < objInfo._count; i++)
        {
            GameObject monster = Instantiate(objInfo._objPrefab, transform.position, Quaternion.identity);
            
            switch(objInfo.MonsterType)
            {
                case Monsters.Slime:
                    SlimeObject slime = monster.GetComponent<SlimeObject>();
                    slime.InitMonster((int)objInfo.MonsterType + 1);
                    break;
                case Monsters.Bat:
                    BatObject bat = monster.GetComponent<BatObject>();
                    bat.InitMonster((int)objInfo.MonsterType + 1);
                    break;
                case Monsters.Skeleton:
                    SkeletonObject skeleton = monster.GetComponent<SkeletonObject>();
                    skeleton.InitMonster((int)objInfo.MonsterType + 1);
                    break;
                case Monsters.Golem:
                    GolemObject golem = monster.GetComponent<GolemObject>();
                    golem.InitMonster((int)objInfo.MonsterType + 1);
                    break;
                case Monsters.RedDragon:
                    RedDragonObject redDragon = monster.GetComponent<RedDragonObject>();
                    redDragon.InitMonster((int)objInfo.MonsterType + 1);
                    break;
                case Monsters.Banshee:
                    BansheeObject banshee = monster.GetComponent<BansheeObject>();
                    banshee.InitMonster((int)objInfo.MonsterType + 1);
                    break;
                case Monsters.DireBat:
                    BatObject direbat = monster.GetComponent<BatObject>();
                    direbat.InitMonster((int)objInfo.MonsterType + 1);
                    break;
            }

            monster.SetActive(false);
            if (objInfo._tfPoolParent != null)
                monster.transform.SetParent(objInfo._tfPoolParent);
            else
                monster.transform.SetParent(this.transform);

            queue.Enqueue(monster);
        }

        return queue;
    }
}
