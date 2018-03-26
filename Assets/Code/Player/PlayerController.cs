using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Weapons;

namespace Assets.Code.Player
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        PlayerData Data;
        public Transform WeaponPosition; //place an equipped weapon is shown
        public GameObject EquippedWeapon;
        public Transform ItemAtPoint;

        public Animator WeaponAnimator;

        void Awake()
        {
            Data = PlayerData.GetSharedInstance();
            //subscribe to the crosshair hit event so we know if we can collect this item
            CrosshairItemDetector.ItemHit += SetItemAtPoint;
            CrosshairItemDetector.NoItemHit += ClearItemAtPoint;

            WeaponAnimator = GetComponentInChildren<Animator>();
            WeaponAnimator.SetBool("SwordEquipped", false);
            WeaponAnimator.SetBool("AxeHamEquipped", false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                WeaponAnimator.SetTrigger("AttackActive");
            
            //DoAttackAnimation
            if (Input.GetKey(KeyCode.E))
            {
                if(ItemAtPoint != null)
                {
                    MoveWeaponToHand(ItemAtPoint.gameObject);
                }
            }

            if (Input.GetKey(KeyCode.Q))
            {
                if(EquippedWeapon != null)
                {
                    MoveWeaponFromHand();
                }
            }
        }

        public void TakeDamage(int damage)
        {
            this.Data.Health -= damage;
            if(this.Data.Health <= 0)
            {
                //fire event tigger gameover
            }
        }

        public void AddGold(int amount)
        {
            this.Data.Gold += amount;
        }

        public void SetItemAtPoint(GameObject obj)
        {
            print("This is an object I can use");
            ItemAtPoint = obj.transform;
        }

        public void ClearItemAtPoint()
        {
            ItemAtPoint = null;
        }

        public void MoveWeaponToHand(GameObject weapon)
        {
            if(EquippedWeapon != null)
            {
                MoveWeaponFromHand();
            }
            var weaponComponent = weapon.GetComponent<Weapon>();
            if(weaponComponent != null)
            {
                //weaponComponent.SetChildRigidBodiesActive(false);
                weaponComponent.DisableRigidBody();
                switch (weaponComponent.Data.Type)
                {
                    case WeaponType.Sword:
                        WeaponAnimator.SetBool("SwordEquipped", true);
                        WeaponAnimator.SetBool("AxeHamEquipped", false);
                        
                        break;
                    case WeaponType.Axe:
                    case WeaponType.Hammer:
                        WeaponAnimator.SetBool("AxeHamEquipped", true);
                        WeaponAnimator.SetBool("SwordEquipped", false);
                        break;
                }
                //attach weapons speed multiplier to the animator
                WeaponAnimator.SetFloat("WeaponSpeed", weaponComponent.Data.Speed);
            }
            weapon.transform.SetParent(WeaponPosition.transform); //attach to hand for animators
            weapon.transform.position = WeaponPosition.position;
            weapon.transform.rotation = WeaponPosition.rotation;
            EquippedWeapon = weapon;
        }

        public void MoveWeaponFromHand()
        {
            var weapon = EquippedWeapon.GetComponentInChildren<Weapon>();
            weapon.EnableRigidBody();
            EquippedWeapon.transform.position = this.transform.position + Vector3.one;
            weapon.transform.SetParent(null);
            EquippedWeapon = null;
            //Disable attack animations (no punching!)
            WeaponAnimator.SetBool("SwordEquipped", false);
            WeaponAnimator.SetBool("AxeHamEquipped", false);
            WeaponAnimator.SetFloat("WeaponSpeed", 1.0f);
        }

       
    }
}
