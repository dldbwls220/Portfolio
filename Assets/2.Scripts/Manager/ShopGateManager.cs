using UnityEngine;

public class ShopGateManager : MonoBehaviour
{
    static ShopGateManager _uniqueinstance;

    [SerializeField] GameObject _posObj;
    Transform[] _gatePos;

    public static ShopGateManager _instance {  get { return _uniqueinstance; } }

    private void Awake()
    {
        _uniqueinstance = this;
    }

    public void InitShopGate()
    {
        _gatePos = new Transform[_posObj.transform.childCount];
        for (int i = 0; i < _gatePos.Length; i++)
        {
            _gatePos[i] = _posObj.transform.GetChild(i).GetComponent<Transform>();
        }

        SpawnGate();
    }

    public void SpawnGate()
    {
        int rnd = Random.Range(0, _gatePos.Length);

        GameObject g = ObjectPool._instance._shopList[0];
        g.transform.position = _gatePos[rnd].position;
        g.SetActive(true);
    }
}
