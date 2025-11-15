using UnityEngine;
using DefineEnum;

public class Slime : CharBase
{
    Animator _aniController;
    PlayerController _target;

    float _hp, _nowHp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitSet(int enemyIndex)
    {
        TableBase table = GameTableManager._instance.Get(TableName.MonsterInfoList);
        string name = table.ToS(enemyIndex, "Name");
        float hp = table.ToF(enemyIndex, "HP");
        int str = table.ToI(enemyIndex, "Strength");
        int gold = table.ToI(enemyIndex, "Gold");
        int beat = table.ToI(enemyIndex, "Beat");

        InitBaseSet(name, str, hp, gold, beat);

        
    }
}
