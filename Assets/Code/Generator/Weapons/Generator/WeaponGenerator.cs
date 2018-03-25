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

        [SerializeField]
        public List<GameObject> SwordStarters;
        [SerializeField]
        public List<GameObject> AxeStarters;//    { get; set; }
        [SerializeField]
        public List<GameObject> HammerStarters;// { get; set; }


        private void Start()
        {
            for(int i = 0; i < 50; i++)
            {
                var wep = Generate(Music.Utils.ChooseEnum<WeaponQuality>(), Music.Utils.ChooseEnum<WeaponType>());
                //Debug.Log("=========<Weapon>=========");
                //Debug.Log("DMG: " + wep.TextDamage);
                //Debug.Log("Speed: " + wep.Speed);
                //Debug.Log("=========</Weapon>=========");
                wep.transform.position = this.transform.position;
                wep.name = "Wep";
            }
           
        }


        public GameObject Generate(WeaponQuality quality, WeaponType type)
        {

            //create weapon model
            var weaponObj = GenerateModel(quality, type);
            weaponObj.transform.position = this.transform.position;
            
            //generate weapon stats
            Weapon weaponComponent = weaponObj.AddComponent<Weapon>();
            weaponComponent.tag = "Weapon";
            var data = _dataGenerator.GenerateWeaponData(quality, type);
            weaponComponent.Data = data;
            weaponComponent.Name = GenerateName(quality, type);
            weaponObj.name = weaponComponent.name;
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
                    model = Instantiate(Music.Utils.ChooseList(SwordStarters));
                    break;
                case WeaponType.Axe:
                    model = Instantiate(Music.Utils.ChooseList(SwordStarters));
                    break;
                case WeaponType.Hammer:
                    model = Instantiate(Music.Utils.ChooseList(SwordStarters));
                    break;
                default:
                    model = Instantiate(Music.Utils.ChooseList(SwordStarters));
                    break;
            }
            
            GrammarEngine.enabled = true;
            GrammarEngine.RewriteSpecificAtom(model);
            return model;
        }

       
    }
}
