using UnityEngine;

public class CharBase : MonoBehaviour
{
    protected string _name;
    protected float _hp;
    protected float _strength;
    protected int _gold;
    protected int _beat;

    protected float _currentStrength;
    protected float _dmgUp;

    protected bool _dead;
    protected bool _detectPlayer;
    protected bool _isMonster;
    protected float _nowHp;

    public bool _isDead { get {  return _dead; } }
    public float _currentHp { get{return _nowHp;} }
    public float _maxHP { get { return _hp; } }

    protected void InitBaseSet(string name, int strength, float hp, int gold, int beat)
    {
        _dead = false;
        _name = name;
        _nowHp = _hp = hp;
        _strength = _currentStrength = strength;
        _gold = gold;
        _beat = beat;
        _detectPlayer = false;
    }

   public void SetCurrentDmg(float dmg)
    {
        _dmgUp = dmg;
    }

    public void InitDamage()
    {
        _currentStrength = _strength + _dmgUp;

        Debug.Log(_currentStrength + "현재 데미지");
    }
}
