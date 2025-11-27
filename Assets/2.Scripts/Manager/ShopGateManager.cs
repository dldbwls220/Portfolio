using UnityEngine;

public class ShopGateManager : MonoBehaviour
{
    [SerializeField] GameObject _posObj;
    Transform[] _gatePos;

    void Start()
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
