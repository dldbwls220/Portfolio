using UnityEngine;
using SimpleJSON;

public class MonsterTable : TableBase
{
    public enum Index
    {
        Index,
        Name,
        HP,
        Strngth,
        Gold,
        Beat,

        count
    }

    public override void Load(string txtData)
    {
        JSONNode node = JSON.Parse(txtData);
        string index = Index.Index.ToString();
        for (int n = 0; n < (int)Index.count; n++)
        {
            Index subKey = (Index)n;
            for (int m = 0; m < node[0].AsArray.Count; m++)
                Add(node[0][m][index], subKey.ToString(), node[0][m][subKey.ToString()].Value);
        }
    }
}
