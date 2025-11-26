using System.Collections;
using UnityEngine;
using DefineEnum;

public class GoldCoinObj : MonoBehaviour
{
    public Sprite _gold;
    public Sprite _goldMany;
    public Sprite _goldFiled;

    PlayerController _playerController;
    SpriteRenderer _sRenderer;

    int _goldCount;

    public void InitGold(int goldCount)
    {
        _goldCount = goldCount;

        _sRenderer = GetComponent<SpriteRenderer>();

        if (goldCount < 10)
            _sRenderer.sprite = _gold;
        else if (goldCount < 20)
            _sRenderer.sprite = _goldMany;
        else
            _sRenderer.sprite = _goldFiled;

        _playerController = GameObject.Find("PlayerCharacter").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager._instance.PlaySFX(SFXName.sfx_pickup_gold_03);

            _playerController.GetGold(_goldCount);

            Destroy(gameObject);
        }
    }
}
