using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefineEnum;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] Dictionary<string, Sprite> _weaponDictionary = new Dictionary<string, Sprite>();

    [SerializeField]Image _weaponImage;
    [SerializeField]Text _weaponInfoText;

    public void InitWeaponUI()
    {
        Sprite[] imgs = Resources.LoadAll<Sprite>("Sprite/Item/Weapon/Dagger/Daggers");
        for(int i = 0; i < imgs.Length; i++)
            _weaponDictionary.Add(imgs[i].name, imgs[i]);

        imgs = Resources.LoadAll<Sprite>("Sprite/Item/Weapon/Sword/Swords");
        for (int i = 0; i < imgs.Length; i++)
            _weaponDictionary.Add(imgs[i].name, imgs[i]);
    }

    public void ChangeWeapon(WeaponName weapon)
    {
        Sprite img = _weaponDictionary[weapon.ToString()];

        _weaponImage.sprite = img;
    }

    public void SetInfoText(WeaponName weapon, int num = 0)
    {
        switch (weapon)
        {
            case WeaponName.DaggerN:
                _weaponInfoText.text = "";
                break;
            case WeaponName.DaggerB:
                _weaponInfoText.text = num + "Kills";
                break;
            case WeaponName.DaggerT:
                _weaponInfoText.text = num + "+ DMG";
                break;
            case WeaponName.DaggerO1:
                _weaponInfoText.text = num + " Combo";
                break;
            case WeaponName.DaggerO2:
                _weaponInfoText.text = num + " Combo";
                break;
            case WeaponName.DaggerO3:
                _weaponInfoText.text = num + " Combo";
                break;
            case WeaponName.SwordN:
                _weaponInfoText.text = "";
                break;
            case WeaponName.SwordB:
                _weaponInfoText.text = num + "Kills";
                break;
            case WeaponName.SwordT:
                _weaponInfoText.text = num + "+ DMG";
                break;
            case WeaponName.SwordO1:
                _weaponInfoText.text = num + " Combo";
                break;
            case WeaponName.SwordO2:
                _weaponInfoText.text = num + " Combo";
                break;
            case WeaponName.SwordO3:
                _weaponInfoText.text = num + " Combo";
                break;
        }
    }
}
