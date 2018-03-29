using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitDetector : MonoBehaviour {

    private GameController GameController;
	// Use this for initialization
	void Start () {
        GameController = GameController.GetSharedInstance();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            var player = GameController.PlayerController;
            int dmg = UnityEngine.Random.Range(1, 4);
            player.TakeDamage(dmg);
        }
    }
}
