using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Weapons
{
    public class WeaponGenerator : MonoBehaviour
    {
        private WeaponDataGenerator _dataGenerator = WeaponDataGenerator.GetSharedInstance();

        public GameObject Generate(WeaponQuality quality, WeaponType type)
        {
            GameObject weaponObj = new GameObject();
            //generate weapon stats
            Weapon weaponComponent = weaponObj.AddComponent<Weapon>();
            weaponComponent.WeaponData = _dataGenerator.GenerateWeaponData(quality, type); ;
            //generate weapon model

            return weaponObj;
        }

    }
}
