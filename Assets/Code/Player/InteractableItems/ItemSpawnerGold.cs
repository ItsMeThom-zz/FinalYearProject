using Player.InteractableItems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
/// <summary>
/// Used by the Grammar Engine & Atoms to spawn interactable pickups of Gold with a random value;
/// </summary>
public class ItemSpawnerGold : MonoBehaviour {

    public List<GameObject> GoldModels;
    
	// Use this for initialization
	void Start () {
        var model = GoldModels.Choose();
        GameObject gold = GameObject.Instantiate(model, this.transform.parent);
        gold.tag = "Interactable";
        var goldComponent = gold.AddComponent<TreasurePickup>();
        goldComponent.Value = UnityEngine.Random.Range(10, 100);
        gold.transform.localPosition = this.transform.localPosition;
        Destroy(this.gameObject);
	}
	
}
