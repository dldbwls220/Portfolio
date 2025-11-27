using UnityEngine;

public class MonsterBase : CharBase
{
    protected virtual void CheckPlayerinRange()
    {
        Collider2D collide = Physics2D.OverlapCircle(transform.position, 7, LayerMask.GetMask("Player"));
        if (collide != null)
            _detectPlayer = true;
        else
            _detectPlayer = false;
    }

    protected void SpawnGold(int gold)
    {
        Vector3 pos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));

        GameObject goldcoin = Resources.Load<GameObject>("Prefabs/Item/GoldCoin");
        GameObject go = Instantiate(goldcoin, pos, Quaternion.identity);
        GoldCoinObj co = go.GetComponent<GoldCoinObj>();

        if (IngameManager._instance._comboNum == 0)
            co.InitGold(gold);
        else if (IngameManager._instance._comboNum == 2)
            co.InitGold(gold * 2);
        else if (IngameManager._instance._comboNum == 3)
            co.InitGold(gold * 3);
    }

    public void UpgradeMonster()
    {
        _hp += 1;
    }
}
