using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectInfo
{
    public GameObject _objPrefab;
    public int _count;
    public Transform _tfPoolParent;
}

public class ObjectPool : MonoBehaviour
{
    [SerializeField] ObjectInfo[] _objInfo;

    public Queue<GameObject> _leftNoteQueue;
    public Queue<GameObject> _rightNoteQueue;

    public static ObjectPool _instance;

    void Awake()
    {
        _instance = this;
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

        _leftNoteQueue = InsertQueue(_objInfo[0]);
        _rightNoteQueue = InsertQueue(_objInfo[1]);
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
}
