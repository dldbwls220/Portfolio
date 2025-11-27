using UnityEngine;

public class ShopkeeperObj : MonoBehaviour
{
    private void Update()
    {
        CheckPlayerinRange();
    }

    void CheckPlayerinRange()
    {
        Collider2D collide = Physics2D.OverlapCircle(transform.position, 10, LayerMask.GetMask("Player"));
        if (collide != null)
        {
            SoundManager._instance._shopkeeperDESC._volum = 1;
            SoundManager._instance._loopDESC._volum = 0;
        }
        else
        {
            SoundManager._instance._shopkeeperDESC._volum = 0;
            SoundManager._instance._loopDESC._volum = 1;
        }
            
    }
}
