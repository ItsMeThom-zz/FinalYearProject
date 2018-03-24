using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class Weapon : MonoBehaviour
    {
        public WeaponData WeaponData;

        public List<Material> WeaponMaterials;


    }
}
