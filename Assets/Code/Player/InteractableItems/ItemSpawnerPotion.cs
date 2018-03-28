using Player.InteractableItems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnerPotion : MonoBehaviour {

    public GameObject PotionModel;
	// Use this for initialization
	void Start () {
        GameObject obj = GameObject.Instantiate(PotionModel, this.transform.parent);
        obj.tag = "Interactable";
        var healthPickup = obj.AddComponent<HealthPickup>();
        healthPickup.Value = 10;
        obj.transform.localPosition = this.transform.localPosition;
        Destroy(this.gameObject);
    }
	
}
