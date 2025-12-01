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
        if (collide != null && !IngameManager._instance._bansheeSound)
        {
            SoundManager._instance._shopkeeperDESC._volum = 0.5f;
            SoundManager._instance._loopDESC._volum = 0;
        }
        else if(!IngameManager._instance._bansheeSound)
        {
            SoundManager._instance._shopkeeperDESC._volum = 0;
            SoundManager._instance._loopDESC._volum = 0.5f;
        }
            
    }
}
