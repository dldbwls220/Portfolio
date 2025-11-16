using UnityEngine;

public class IngameManager : MonoBehaviour
{
    private void Awake()
    {
        GameTableManager._instance.AllLoadTable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
