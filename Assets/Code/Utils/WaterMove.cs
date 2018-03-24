using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMove : MonoBehaviour {

    public float WaterLevel = 10.5f;

    private Transform playerPosition;

	// Use this for initialization
	void Start () {
        
       
	}
	
	// Update is called once per frame
	void Update () {
        if(GameObject.FindGameObjectsWithTag("Player").Length > 0)
        {
            playerPosition = GameObject.FindGameObjectsWithTag("Player")[0].transform;
            var pX = playerPosition.transform.position.x;
            var pZ = playerPosition.transform.position.z;
            this.transform.position = new Vector3(pX, WaterLevel, pZ);
        };

        
	}
}
