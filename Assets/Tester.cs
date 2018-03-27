using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var donjon = GetComponent<DungeonGenerator>();
        donjon.Generate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
