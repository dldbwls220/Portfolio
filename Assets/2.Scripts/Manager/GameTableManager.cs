using DefineEnum;
using System.Collections.Generic;
using UnityEngine;

public class GameTableManager :TSingleton<GameTableManager>
{
    Dictionary<TableName, TableBase> _tableDoc;

    protected override void Init()
    {
        base.Init();
        _tableDoc = new Dictionary<TableName, TableBase>();
    }

    TableBase Load<T>(TableName name) where T : TableBase, new()
    {
        if (_tableDoc.ContainsKey(name))
        {
            TableBase tBase = _tableDoc[name];
            return tBase;
        }

        TextAsset tAsset = Resources.Load("Tables/" + name.ToString()) as TextAsset;


        if (tAsset != null)
        {
            T t = new T();
            t.Load(tAsset.text);
            _tableDoc.Add(name, t);
        }
        else
        {
            Debug.LogFormat("{0}이 Resources/Tables 에 없습니다.", name);
        }
        return _tableDoc[name];
    }

    public void AllLoadTable()
    {
        Load<MonsterTable>(TableName.MonsterInfoList);
        Load<MusicTable>(TableName.MusicList);
    }

    public TableBase Get(TableName name)
    {
        if (_tableDoc.ContainsKey(name))
            return _tableDoc[name];

        return null;
    }
}
