using Assets.Code.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class CrosshairItemDetector : MonoBehaviour {

    private Camera cam; //reference to player camera to use centerpoint for raycasts

    public PlayerController Controller;

    public delegate void WeaponHitEvent(GameObject obj);
    public static event WeaponHitEvent WeaponHit;

    public delegate void ItemHitEvent(GameObject obj);
    public static event ItemHitEvent ItemHit;

    public delegate void NoItemHitEvent();
    public static event NoItemHitEvent NothingHit;
    

    void Start () {
        cam = this.GetComponentInChildren<Camera>();
	}
	
	
	void Update () {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10f))
        {
            if (hit.collider.gameObject.tag.Equals("Weapon"))
            {
                print("Weapon detected");
                WeaponHit(hit.collider.gameObject);
            }else if (hit.collider.gameObject.tag.Equals("Interactable"))
            {
                print("Interactable detected");
                ItemHit(hit.collider.gameObject);
            }
            else
            {
                NothingHit();
            }

        }
        else
        {
            NothingHit();
        }
            
    }
}
