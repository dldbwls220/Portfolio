using DefineEnum;
using UnityEngine;

public class DiamondObj : MonoBehaviour
{
    public Sprite _diamnondSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager._instance.PlaySFX(SFXName.sfx_pickup_gold_03);

            collision.transform.parent.GetComponent<PlayerController>().GetDiamond();

            Destroy(gameObject);
        }
    }
}
