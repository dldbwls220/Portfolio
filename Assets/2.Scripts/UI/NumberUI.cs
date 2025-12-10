using UnityEngine;
using UnityEngine.UI;

public class NumberUI : MonoBehaviour
{
    [SerializeField] Text _gold;
    [SerializeField] Text _kill;
    [SerializeField] Text _boss;

    public void GoldCountUI(int gold)
    {
        _gold.text = " X " + gold.ToString("D3");
    }

    public void KillCountUI(int kill)
    {
        _kill.text =" X " + kill.ToString("D3");
    }

    public void BossCountUI(int bossKilled, int bossTotal)
    {
        _boss.text = " X " + bossKilled.ToString() + "/" + bossTotal.ToString();
    }
}
