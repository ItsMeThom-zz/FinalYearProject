using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollisionRegister : MonoBehaviour {

    CapsuleCollider weaponcoll;
	void Start () {
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
        Debug.Log("Hit made with something!");
    }
}
