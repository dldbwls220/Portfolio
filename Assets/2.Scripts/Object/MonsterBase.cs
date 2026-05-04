using DefineEnum;
using UnityEngine;

public class MonsterBase : CharBase
{
    protected float _bpm;
    public MonsterPriority _monsterP;

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
        Debug.Log($"SpawnGold() »£√‚µ , comboNum = {IngameManager._instance._comboNum}");

        Vector3 pos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));

        GameObject goldcoin = Resources.Load<GameObject>("Prefabs/Item/GoldCoin");
        GameObject go = Instantiate(goldcoin, pos, Quaternion.identity);
        GoldCoinObj co = go.GetComponent<GoldCoinObj>();

        if (IngameManager._instance._comboNum < 2)
            co.InitGold(gold);
        else if (IngameManager._instance._comboNum == 2)
            co.InitGold(gold * 2);
        else if (IngameManager._instance._comboNum == 3)
            co.InitGold(gold * 3); 
    }

    protected void SpawnDiamond()
    {
        Vector3 pos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));

        GameObject diamond = Resources.Load<GameObject>("Prefabs/Item/Diamond");
        GameObject go = Instantiate(diamond, pos, Quaternion.identity);

    }

    protected void GetGoldAndDiamond(int gold)
    {
        if (Random.Range(1, 101) <= 5)
        {
            SpawnDiamond();
        }
        else
            SpawnGold(gold);
    }

    public void UpgradeMonster()
    {
        _hp += 1;
    }

    public bool CanReserve(Node node)
    {
        if(node == null) return false;

        if(!node._walkable) return false;

        if(node._reservedBy == null) return true;

        if(node._reservedBy == this) return true;

        if ((int)this._monsterP > (int)node._reservedBy._monsterP)
        {
            node._reservedBy = this;
            return true;
        }

        return false;
    }

    public void ReleaseReservation(Node node)
    {
        if (node == null) return;
        if (node._reservedBy == this)
            node._reservedBy = null;
    }
}
