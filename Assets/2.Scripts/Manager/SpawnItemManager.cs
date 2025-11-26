using UnityEngine;

public class SpawnItemManager : MonoBehaviour
{
    [SerializeField] GameObject _itemPosObj;
    Transform[] _itemTf;

    void Start()
    {
        _itemTf = new Transform[_itemPosObj.transform.childCount];

        for (int i = 0; i < _itemPosObj.transform.childCount; i++)
        {
            _itemTf[i] = _itemPosObj.transform.GetChild(i).GetComponent<Transform>();
        }

        SetSellingItems();
    }

    public void SetSellingItems()
    {
        int rndW = Random.Range(0, ObjectPool._instance._weaponList.Count);
        GameObject w = ObjectPool._instance._weaponList[rndW];
        w.transform.position = _itemTf[0].position;
        w.SetActive(true);

        int rndF = Random.Range(0, ObjectPool._instance._foodList.Count);
        GameObject f = ObjectPool._instance._foodList[rndF];
        f.transform.position = _itemTf[1].position;
        f.SetActive(true);

        GameObject pu = ObjectPool._instance._powerUpList[0];
        pu.transform.position = _itemTf[2].position;
        pu.SetActive(true);

        pu = ObjectPool._instance._powerUpList[1];
        pu.transform.position = _itemTf[3].position;
        pu.SetActive(true);
    }
}
