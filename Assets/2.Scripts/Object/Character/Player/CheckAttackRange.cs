using UnityEngine;

public class CheckAttackRange : MonoBehaviour
{
    CharBase _owner;

    public void InitSetRange(CharBase o)
    {
        _owner = o;
    }

    public T GetOwner<T>() where T : CharBase
    {
        return (T)_owner;
    }
}
