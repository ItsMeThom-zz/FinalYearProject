using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;
using Utils;

namespace Weapons
{
    public enum WeaponQuality { Common, Rare, Legendary }
    public enum WeaponType { Sword, Axe, Hammer}

 


    public class WeaponData
    {
        public Vector2i DamageRange {get;set;}
        public float Speed { get; set; }
        public WeaponQuality Quality { get; set; }
        public WeaponType Type { get; set; }
        public int RollDamage()
        {
            return UnityEngine.Random.Range(DamageRange.X, DamageRange.Z);
        }

        public string DamageText()
        {
            var builder = new StringBuilder();
            builder.Append(DamageRange.X);
            if(DamageRange.Z > DamageRange.X)
            {
                builder.Append("-");
                builder.Append(DamageRange.Z);
            }
            return builder.ToString();
        }
        public string QualityText()
        {
            switch (Quality)
            {
                case WeaponQuality.Common:
                    return "Common";

                case WeaponQuality.Rare:
                    return "Rare";

                case WeaponQuality.Legendary:
                    return "Legendary";
                default:
                    return "Unknown";
            }
        }
    }
}
