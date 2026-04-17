using DefineEnum;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Unlock
{
    public GameObject _objPrefab;
    public Transform _tfPoolParent;
}

public class LobbyObjectPool : MonoBehaviour
{
    [SerializeField] Unlock[] _unlockInfo;

    public List<GameObject> _unlockList;

    static LobbyObjectPool _uniqueInstance;

    public static LobbyObjectPool _instance { get { return _uniqueInstance; } }

    void Awake()
    {
        _uniqueInstance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitPool();

        SpawnUnlockItemManager._instance.InitSpawn();
    }

    public void InitPool()
    {
        _unlockList = new List<GameObject>();

        for (int i = 0; i < (int)UnlockItems.Count; i++)
        {
            InsertUnlockList(_unlockInfo[i]);
        }

    }

    void InsertUnlockList(Unlock objInfo)
    {
        GameObject weapon = Instantiate(objInfo._objPrefab, transform.position, Quaternion.identity);
        weapon.SetActive(false);
        weapon.transform.SetParent(objInfo._tfPoolParent);

        _unlockList.Add(weapon);
    }
    
}

