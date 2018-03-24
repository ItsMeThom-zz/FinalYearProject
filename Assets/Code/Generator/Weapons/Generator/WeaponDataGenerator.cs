using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils;

namespace Weapons
{
    public class WeaponDataGenerator
    {
        /// <summary>
        /// Singleton pattern for data generator
        /// </summary>
        public static WeaponDataGenerator _instance;
        public static WeaponDataGenerator GetSharedInstance()
        {
            if(_instance == null)
            {
                _instance = new WeaponDataGenerator();
            }
            return _instance;
        }

        private Dictionary<WeaponType, Dictionary<WeaponQuality, Vector2i>> QualityDMGRange;  //range in which damage for this quality lies
        private Dictionary<WeaponQuality, int> QualityMaxStep;                                //maximum range step between chosen vals
        private Dictionary<WeaponType, float> TypeSpeedVariant;

        private WeaponDataGenerator()
        {
            QualityDMGRange = new Dictionary<WeaponType, Dictionary<WeaponQuality, Vector2i>>() {
                {WeaponType.Sword, new Dictionary<WeaponQuality, Vector2i>(){
                        { WeaponQuality.Common,    new Vector2i(1,5)  },
                        { WeaponQuality.Rare,      new Vector2i(4,10) },
                        { WeaponQuality.Legendary, new Vector2i(7,20) }
                    }
                },
                {WeaponType.Axe, new Dictionary<WeaponQuality, Vector2i>(){
                        { WeaponQuality.Common,    new Vector2i(1,5)  },
                        { WeaponQuality.Rare,      new Vector2i(4,10) },
                        { WeaponQuality.Legendary, new Vector2i(7,20) }
                    }
                },
                {WeaponType.Hammer, new Dictionary<WeaponQuality, Vector2i>(){
                        { WeaponQuality.Common,    new Vector2i(1,5)  },
                        { WeaponQuality.Rare,      new Vector2i(4,10) },
                        { WeaponQuality.Legendary, new Vector2i(7,20) }
                    }
                },
            };

            QualityMaxStep = new Dictionary<WeaponQuality, int>()
            {
                { WeaponQuality.Common,    3 },
                { WeaponQuality.Rare,      4 },
                { WeaponQuality.Legendary, 5 }
            };

            TypeSpeedVariant = new Dictionary<WeaponType, float>()
            {
                {WeaponType.Axe, 0.5f },
                {WeaponType.Sword, 0.3f },
                {WeaponType.Hammer, -0.5f },
            };


        }


        public WeaponData GenerateWeaponData(WeaponQuality quality, WeaponType type)
        {
            //Generate the range of damage this weapon type can do
            Vector2i damageRange = QualityDMGRange[type][quality];
            int dmgLow = UnityEngine.Random.Range((int)damageRange.X, (int)damageRange.Z+1);
            int dmgHi = dmgLow;
            if(dmgHi != damageRange.Z)
            {
                dmgHi = UnityEngine.Random.Range(dmgLow, dmgLow + 1 + QualityMaxStep[quality]);
            }

            float speedVariantMax = TypeSpeedVariant[type];
            float speedVariant = UnityEngine.Random.Range(0, speedVariantMax);

            WeaponData newWeaponData = new WeaponData();
            newWeaponData.Quality = quality;
            newWeaponData.Type = type;
            newWeaponData.DamageRange = new Utils.Vector2i(dmgLow, dmgHi);
            newWeaponData.Speed = 1.0f + speedVariant;

            return newWeaponData;
        }


    }
}
