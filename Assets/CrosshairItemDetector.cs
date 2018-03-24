using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class CrosshairItemDetector : MonoBehaviour {

    Camera cam;

    public delegate void HitEvent(GameObject obj);
    public static event HitEvent ItemHit;
    public delegate void NoItemHitEvent();
    public static event NoItemHitEvent NoItemHit;
    

    void Start () {
        cam = this.GetComponentInChildren<Camera>();
	}
	
	
	void Update () {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10f))
        {
            if (hit.collider.gameObject.tag.Equals("Weapon") && hit.distance < 10f)
            {
                var weaponChild = hit.collider.gameObject.name;
                print(weaponChild);
                //BroadcastMessage("ShowInfoBox", hit.collider.gameObject);
                ItemHit(hit.collider.gameObject);
            }
        }
        else
        {
            NoItemHit();
            //BroadcastMessage("HideInfoBox");
        }
            
    }
}
