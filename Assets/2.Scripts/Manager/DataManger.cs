using UnityEngine;
using DefineStructure;
using System.IO;


public class DataManger : TSingleton<DataManger>
{
    public PlayerData nowPlayer = new PlayerData();

    string _path;
    string _fileName = "save";

    private void Awake()
    {
        _path = Application.persistentDataPath + "/";
    }

    public void SaveData()
    {
        string data = JsonUtility.ToJson(nowPlayer);
        File.WriteAllText(_path + _fileName, data);

        Debug.Log(_path);
    }

    public void LoadData()
    {
        if (!File.Exists(_path + _fileName))
        {
            SaveData();
            Debug.Log("파일생성");
        }

        string data = File.ReadAllText(_path + _fileName);
        nowPlayer = JsonUtility.FromJson<PlayerData>(data);
        Debug.Log("파일불러오기");
    }
}
