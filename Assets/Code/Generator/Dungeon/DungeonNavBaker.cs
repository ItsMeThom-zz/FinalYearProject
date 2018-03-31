using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonNavBaker : MonoBehaviour {

	//Recalculates nav point neighbours post-room creation.
    // Allows nav points to be connected across room
	void Start () {
        NavPoint[] AllNavPoints = GetComponentsInChildren<NavPoint>();
        foreach(var np in AllNavPoints)
        {
            np.FindNeighbours();
            np.Baked = true;
        }


	}
	

}
