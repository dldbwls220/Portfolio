using UnityEngine;
using DefineStructure;
using System.IO;
using DefineEnum;

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

    public void AddKillCount(Monsters state)
    {
        //if (_totalData._score._monsterKillCountList.Count == 0)
        //{
        //    for (int i = 0; i < (int)Monsters.Count; i++)
        //    {
        //        Monsters s = (Monsters)i;

        //        MonsterKillData monster = new MonsterKillData(s.ToString(), 0);
        //        _totalData._score._monsterKillCountList.Add(monster);
        //    }
        //}

        //MonsterKillData targetMonster = _totalData._score._monsterKillCountList.Find(x => x.monsterName == state.ToString());


        //targetMonster.count += 1;

        _totalData._score._monsterKillCountDic[state.ToString()] += 1;
    }

    public int GetKillCount(Monsters state)
    {        
        if(state == Monsters.Count)
        {
            int total = 0;

            for (int i = 0; i < (int)Monsters.Count; i++)
            {
                Monsters s = (Monsters)(i);

                total += _totalData._score._monsterKillCountDic[s.ToString()];
            }
            return total;
        }

        return _totalData._score._monsterKillCountDic[state.ToString()];
    }

    

}
