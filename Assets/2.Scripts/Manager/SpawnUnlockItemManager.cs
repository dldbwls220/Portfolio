using UnityEngine;

public class SpawnUnlockItemManager : MonoBehaviour
{
    static SpawnUnlockItemManager _uniqueinstance;

    [SerializeField] GameObject _unlockItemPosObj;
    Transform[] _unlockTf;

    public static SpawnUnlockItemManager _instance { get { return _uniqueinstance; } }

    void Awake()
    {
        _uniqueinstance = this;
    }

    public void InitSpawn()
    {
        _unlockTf = new Transform[_unlockItemPosObj.transform.childCount];

        for (int i = 0; i < _unlockTf.Length; i++)
        {
            _unlockTf[i] = _unlockItemPosObj.transform.GetChild(i).GetComponent<Transform>();
        }

        SetUnlockItem();
    }

    public void SetUnlockItem()
    {
        Debug.Log("ÇØ±Ý");

        int index = 0;
        int limit = 0;
        while (limit < 3)
        {
            ItemBase itemB = LobbyObjectPool._instance._unlockList[index].GetComponent<ItemBase>();

            GameObject gb = LobbyObjectPool._instance._unlockList[index++];

            if (itemB._isUnlocked)
            {
                continue;
            }

            gb.transform.position = _unlockTf[limit++].position;
            gb.SetActive(true);
        }
    }
}
