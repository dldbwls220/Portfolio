using System.Collections.Generic;
using UnityEngine;

public abstract class TableBase
{
    Dictionary<string, Dictionary<string, string>> _tableDatas;

    public int _recordCount{ get { return _tableDatas.Count; } }

    protected TableBase()
    {
        _tableDatas = new Dictionary<string, Dictionary<string, string>>();
    }

    protected void Add(string index, string column, string val)
    {
        if(!_tableDatas.ContainsKey(index))
            _tableDatas.Add(index, new Dictionary<string, string>());
        if (!_tableDatas[index].ContainsKey(column))
            _tableDatas[index].Add(column, val);
        else
            Debug.LogErrorFormat("Index[{0}], Column[{1}](이/가) 같은 자료가 있습니다.", index, column);
    }

    public abstract void Load(string txtData);

    public string ToS(string index, string column)
    {
        string findValue = string.Empty;
        if(_tableDatas.ContainsKey(index))
            _tableDatas[index].TryGetValue(column, out findValue);

        return findValue;
    }

    public string ToS(int index, string column)
    {
        return ToS(index.ToString(), column);
    }

    public int ToI(string index, string column)
    {
        string findValue = ToS(index, column);
        int val = 0;
        if (int.TryParse(findValue, out val))
            return val;
        else
            return int.MinValue;

    }

    public int ToI(int index, string column)
    {
        return ToI(index.ToString(), column);
    }

    public float ToF(string index, string column)
    {
        string findValue = ToS(index, column);
        float val = 0;
        if (float.TryParse(findValue, out val))
            return val;
        else
            return float.MinValue;
    }
    public float ToF(int index, string column)
    {
        return ToF(index.ToString(), column);
    }
}
