using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When the main scene loads, control is attached to this object
/// to show a loading screen.
/// When the WorldController is ready to place the player it disables this component
/// </summary>
public class WorldLoaderController : MonoBehaviour {


    public WorldController GenerationController;

    void Start () {
        GenerationController.StartAll();
	}
	
}
