using UnityEngine;
using DefineStructure;
using System.IO;


public class DataManger : TSingleton<DataManger>
{
    public TotalData _totalData = new TotalData();

    string _path;
    string _fileName = "save";

    private void Awake()
    {
        _path = Application.persistentDataPath + "/";
    }

    public void SaveData()
    {
        string data = JsonUtility.ToJson(_totalData);
        File.WriteAllText(_path + _fileName, data);

        Debug.Log(_path);
        Debug.Log("저장 데이터: " + JsonUtility.ToJson(_totalData));
    }

    public void LoadData()
    {
        if (!File.Exists(_path + _fileName))
        {
            SaveData();
            Debug.Log("파일생성");
        }

        string data = File.ReadAllText(_path + _fileName);
        _totalData = JsonUtility.FromJson<TotalData>(data);
        Debug.Log("파일불러오기");
    }

    
}
