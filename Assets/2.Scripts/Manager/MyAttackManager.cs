using UnityEngine;

public class MyAttackManager : MonoBehaviour
{
    [SerializeField] PlayerController _pController;

    void OnAttackCollider(int i)
    {
        _pController.EnableWeaponCollider(i);
    }

    void OffAttackColider()
    {
        _pController.DisableWeaponCollider();
    }
}
