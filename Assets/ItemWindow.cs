using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Weapons;

public class ItemWindow : MonoBehaviour {

    private bool IsEnabled = false;
    public Canvas InfoCanvas;

    public GameObject WeaponNameField;
    public GameObject WeaponDamageField;
    public GameObject WeaponQualityField;
    public GameObject WeaponSpeedField;

    private TextMeshProUGUI _weaponName;
    private TextMeshProUGUI _weaponQuality;
    private TextMeshProUGUI _weaponDamage;
    private TextMeshProUGUI _weaponSpeed;
    // Use this for initialization
    void Start () {
        _weaponName = WeaponNameField.GetComponent<TextMeshProUGUI>();
        _weaponQuality = WeaponQualityField.GetComponent<TextMeshProUGUI>();
        _weaponDamage = WeaponDamageField.GetComponent<TextMeshProUGUI>();
        _weaponSpeed = WeaponSpeedField.GetComponent<TextMeshProUGUI>();

        CrosshairItemDetector.ItemHit += ShowInfoBox;
        CrosshairItemDetector.NoItemHit += HideInfoBox;
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    public void ShowInfoBox(GameObject item)
    {
        
        if (!InfoCanvas.enabled)
        {
            //print("Showing Weapon Info Box");
            InfoCanvas.enabled = true;
            IsEnabled = true;
        }

        Weapon weaponComponent = item.GetComponent<Weapon>();
        if(weaponComponent != null)
        {
            DisplayWeaponData(weaponComponent);
        }
    }

    public void HideInfoBox()
    {
        
        if (IsEnabled)
        {
            InfoCanvas.enabled = false;
            //print("Hiding Canvas this time");
            InfoCanvas.enabled = false;
            IsEnabled = false;
        }
        
    }

    public void DisplayWeaponData(Weapon weapon)
    {
        if(weapon.Data != null)
        {
            Color32 qualityColor = new Color32(220, 220, 220, 255);
            switch (weapon.Data.Quality)
            {
                case WeaponQuality.Common:
                    break;
                case WeaponQuality.Rare:
                    qualityColor = new Color32(54, 236, 189, 255);
                    break;
                case WeaponQuality.Legendary:
                    qualityColor = new Color32(209, 23, 171, 255);
                    break;
            }
            _weaponName.text = weapon.Name;
            _weaponName.color = qualityColor;
            _weaponQuality.text = weapon.QualityText;
            _weaponQuality.color = qualityColor;

            _weaponDamage.text = weapon.DamageText + " DMG ";
            _weaponSpeed.text = weapon.Speed.ToString() + " SPD";
        }
        else
        {
            print("The weapondata is null for this object!");
        }
    }
}
