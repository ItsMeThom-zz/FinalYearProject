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

        
        private int[,] QualityMaxStep; //maximum range step between chosen vals
        private float[,] TypeSpeedVariant;

        private Vector2i[,] DMGRange;

        private WeaponDataGenerator()
        {
            //sword
            //axe    (fast)
            //hammer (slow)

            DMGRange = new Vector2i[,]
            {
                {new Vector2i(1,5), new Vector2i(4,10), new Vector2i(7,20)},
                {new Vector2i(1,5), new Vector2i(4,10), new Vector2i(7,20)},
                {new Vector2i(1,5), new Vector2i(4,10), new Vector2i(7,20)},
            };

            QualityMaxStep = new int[,]
             {
                {2, 3, 4},
                {3, 3, 4},
                {3, 3, 4}
            };

            TypeSpeedVariant = new float[,]
            {
                {0.3f, 0.2f, 0.4f},
                {0.2f, 0.4f, 0.8f },
                {-0.5f, 0.2f, -0.1f}
            };


        }


        public WeaponData GenerateWeaponData(WeaponQuality quality, WeaponType type)
        {
            //Generate the range of damage this weapon type can do
            Vector2i damageRange = DMGRange[(int)type, (int)quality];
            int dmgLow = UnityEngine.Random.Range((int)damageRange.X, (int)damageRange.Z);
            int dmgHi = dmgLow;
            if(dmgHi != damageRange.Z)
            {
                dmgHi = UnityEngine.Random.Range(dmgLow, dmgLow + 1 + QualityMaxStep[(int)type, (int)quality]);
            }

            //detmine the speed of the weapon (capped to 0.05 steps)
            float speedVariantMax = TypeSpeedVariant[(int)type, (int)quality];
            float speedVariantSelected = UnityEngine.Random.Range(0, speedVariantMax);
            float stepSize = 0.05f;
            int numSteps = (int)Math.Floor(speedVariantSelected / stepSize);
            float speedVariant = numSteps * stepSize;
            WeaponData newWeaponData = new WeaponData();
            newWeaponData.Quality = quality;
            newWeaponData.Type = type;
            newWeaponData.DamageRange = new Utils.Vector2i(dmgLow, dmgHi);
            newWeaponData.Speed = 1.0f + speedVariant;

            return newWeaponData;
        }


    }
}
