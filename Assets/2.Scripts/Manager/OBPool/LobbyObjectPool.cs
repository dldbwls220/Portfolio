using DefineEnum;
using DefineStructure;
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
    UnlockedItem _unlockI;

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
        UnlockedItem uis = DataManger._instance._totalData._unlockDate;

        Debug.Log(uis);

        for (int i = 0; i < (int)UnlockItems.Count; i++)
        {
            ItemBase ib = _unlockInfo[i]._objPrefab.transform.GetComponent<ItemBase>();

            ib._isUnlocked = false;

            if (uis._unlockedItem.Count > 0)
                for (int n = 0; n < uis._unlockedItem.Count; n++)
                {
                    if (uis._unlockedItem[n] == ib._thisItemID)
                    {                      
                        ib._isUnlocked = true;
                    }
                }
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

