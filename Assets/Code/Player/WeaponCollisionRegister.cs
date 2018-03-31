using Assets.Code.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollisionRegister : MonoBehaviour {

    GameController Controller;
    CapsuleCollider weaponcoll;
	void Start () {
        Controller = GameController.GetSharedInstance();
        weaponcoll = GetComponent<CapsuleCollider>();
        if(weaponcoll == null) { print("fup"); }
        weaponcoll.isTrigger = true;
        weaponcoll.enabled = false;
	}
	

    public void ActivateCollider(int active)
    {
        if (active == 0)
        {
            weaponcoll.enabled = false;
        }
        else
        {
            weaponcoll.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        //Debug.Log("Hit made with something!");
        if(c.gameObject.tag == "Enemy")
        {
            ActorController actor = c.gameObject.GetComponent<ActorController>();
            if (actor)
            {
                actor.TakeDamage(Controller.PlayerController.GetWeaponDamage());
            }
        }
    }
}
