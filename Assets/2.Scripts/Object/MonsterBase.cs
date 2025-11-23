using UnityEngine;

public class MonsterBase : MonoBehaviour
{
    protected string _name;
    protected float _hp;
    protected float _strength;
    protected int _gold;
    protected int _beat;

    protected bool _dead;
    protected bool _detectPlayer;
    protected float _nowHp;

    public bool _isDead { get { return _dead; } }
    public float _currentHp { get { return _nowHp; } }


    protected void InitBaseSet(string name, int strength, float hp, int gold, int beat)
    {
        _dead = false;
        _name = name;
        _nowHp = _hp = hp;
        _strength = strength;
        _gold = gold;
        _beat = beat;
        _detectPlayer = false;
    }

    protected virtual void CheckPlayerinRange()
    {
        Collider2D collide = Physics2D.OverlapCircle(transform.position, 5, LayerMask.GetMask("Player"));
        if (collide != null)
            _detectPlayer = true;
        else
            _detectPlayer = false;
    }
}
