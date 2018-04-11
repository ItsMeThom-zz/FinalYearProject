using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using Weapons;

namespace Assets.Code.Player
{
    /// <summary>
    /// PlayerController handles player interaction. It subscribed to events
    /// from the CrosshairDetector component in order to register items that are
    /// interactable such as weapons or consumables.
    /// </summary>
    public class PlayerController : MonoBehaviour, IDamageable
    {
        private PlayerData Data;
        public  Transform  WeaponPosition; //place an equipped weapon is shown
        public  GameObject EquippedWeapon;
        public  Transform  ItemAtPoint;
        public  Animator   WeaponAnimator;

        public bool GodMode = false;
        #region UI Events
        public delegate void GoldChangedEvent();
        public static event GoldChangedEvent GoldChanged;
        public delegate void HealthChangeEvent();
        public static event HealthChangeEvent HealthChanged;
        #endregion

        void Awake()
        {
            GameController.GetSharedInstance().PlayerController = this;
            Data = PlayerData.GetSharedInstance();
            //subscribe to the crosshair hit event so we know if we can collect this item
            CrosshairItemDetector.WeaponHit += SetItemAtPoint;
            CrosshairItemDetector.ItemHit += SetItemAtPoint;
            CrosshairItemDetector.NothingHit += ClearItemAtPoint;

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
                    if(ItemAtPoint.tag.Equals("Weapon"))
                    {
                        MoveWeaponToHand(ItemAtPoint.gameObject);
                    }else if(ItemAtPoint.tag.Equals("Interactable")){
                        
                        ConsumeInteractableItem(ItemAtPoint.gameObject);
                    }
                }
            }
            //Drop weapon
            if (Input.GetKey(KeyCode.Q))
            {
                if(EquippedWeapon != null)
                {
                    MoveWeaponFromHand();
                }
            }

            if (Input.GetKey(KeyCode.T))
            {
                
                Debug.Log("GOD MODE ON");
                gameObject.GetComponent<FlyCamera>().enabled = true;
                gameObject.GetComponent<RigidbodyFirstPersonController>().enabled = false;
                gameObject.GetComponent<Rigidbody>().useGravity = false;
                
            }
            if (Input.GetKey(KeyCode.Y))
            {
                Debug.Log("GOD MODE OFF");
                gameObject.GetComponent<FlyCamera>().enabled = false;
                gameObject.GetComponent<RigidbodyFirstPersonController>().enabled = true;
                gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
        }

        private void ConsumeInteractableItem(GameObject gameObject)
        {
            IInteractable item = gameObject.GetComponent<IInteractable>();
            if(item != null)
            {
                item.Consume(gameObject);
            }
        }

        public void TakeDamage(int damage)
        {
            this.Data.Health -= damage;
            HealthChanged();
            if (this.Data.Health <= 0)
            {
                //fire event tigger gameover
            }
        }

        public void AddGold(int amount)
        {
            this.Data.Gold += amount;
            GoldChanged();
        }

        public int GetGoldValue()
        {
            return this.Data.Gold;
        }

        public void AddHealth(int amount)
        {
            this.Data.Health += amount;
            HealthChanged();
        }

        public int GetHealthValue()
        {
            return this.Data.Health;
        }

        public void SetItemAtPoint(GameObject obj)
        {
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

        public int GetWeaponDamage()
        {
            if(EquippedWeapon == null)
            {
                return 1;
            }
            if(EquippedWeapon.GetComponent<Weapon>() != null)
            {
                Weapon wep = EquippedWeapon.GetComponent<Weapon>();
                var dmg = wep.DamageRoll;
                print("DMG: " + dmg);
                return dmg;
            }
            return 1;
        }
       
    }
}
