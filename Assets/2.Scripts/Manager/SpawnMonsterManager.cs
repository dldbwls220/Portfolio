using UnityEngine;

public class SpawnMonsterManager : MonoBehaviour
{
    [SerializeField] TileMapGridManager _grid;
    [SerializeField] GameObject _enemySpawnObj;
    Transform[] _spawnPositions;

    int _myBeat;

    private void Start()
    {
        NoteManager._instance.OnBeat += OnBeat;
        _spawnPositions = new Transform[_enemySpawnObj.transform.childCount];

        for (int i = 0; i < _enemySpawnObj.transform.childCount; i++)
        {
            _spawnPositions[i] = _enemySpawnObj.transform.GetChild(i).GetComponent<Transform>();
        }

        _myBeat = 0;
    }

    void OnBeat()
    {
        if (!IngameManager._instance._isStartMusic) return;

        _myBeat += 1;
        
        if (_myBeat > 8)
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
            case 8:
                if (ObjectPool._instance._golemQueue.Count > 0)
                {
                    GameObject golem = ObjectPool._instance._golemQueue.Dequeue();
                    golem.transform.position = SetPostion();
                    golem.SetActive(true);
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
