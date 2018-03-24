using GenerativeGrammar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Weapons
{
    public class WeaponGenerator : MonoBehaviour
    {
        private WeaponDataGenerator _dataGenerator = WeaponDataGenerator.GetSharedInstance();

        public GrammarEngine GrammarEngine;

        public List<GameObject> SwordStarters  { get; set; }
        public List<GameObject> AxeStarters    { get; set; }
        public List<GameObject> HammerStarters { get; set; }


        private void Start()
        {
            for(int i = 0; i < 1; i++)
            {
                //var wep = Generate(Music.Utils.ChooseEnum<WeaponQuality>(), Music.Utils.ChooseEnum<WeaponType>());
                //wep.name = "Weapon Object";
                //Debug.Log("=========<Weapon>=========");
                //Debug.Log("DMG: " + wep.TextDamage);
                //Debug.Log("Speed: " + wep.Speed);
                //Debug.Log("=========</Weapon>=========");
                //GameObject obj = Instantiate(wep, this.gameObject.transform);
            }
           
        }


        public GameObject Generate(WeaponQuality quality, WeaponType type)
        {

            //create weapon model
            GameObject weaponObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            weaponObj.transform.position = this.transform.position;
            weaponObj.tag = "Weapon";
            //generate weapon stats
            Weapon weaponComponent = weaponObj.AddComponent<Weapon>();
            weaponComponent.Data = _dataGenerator.GenerateWeaponData(quality, type); ;
            weaponComponent.Name = GenerateName(quality, type);
            return weaponObj;
        }

        public string GenerateName(WeaponQuality quality, WeaponType type)
        {
            var sb = new StringBuilder();

            sb.Append(quality.ToString());
            sb.Append(" ");
            sb.Append(type.ToString());
            sb.Append(" ");
            sb.Append("of Testing");
            return sb.ToString();

        }


        public GameObject GenerateModel(WeaponQuality quality, WeaponType type)
        {
            GameObject model;
            switch (type)
            {
                case WeaponType.Sword:
                    model = Music.Utils.ChooseList(SwordStarters);
                    break;
                case WeaponType.Axe:
                    model = Music.Utils.ChooseList(SwordStarters);
                    break;
                case WeaponType.Hammer:
                    model = Music.Utils.ChooseList(SwordStarters);
                    break;
                default:
                    model = Music.Utils.ChooseList(SwordStarters);
                    break;
            }
            GameObject weaponModel = GrammarEngine.RewriteSpecificAtom(model);
            return weaponModel;

        }

       
    }
}
