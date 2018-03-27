using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawner : MonoBehaviour {

    GameController GameController = GameController.GetSharedInstance();

    Color color1 = Color.green;
    Color color2  = Color.yellow;
    Light interiorLight;
    float duration = 3.0f;
    private void Awake()
    {
        //Prevent duplicate keys from spawning in the same dungeon twice
        if (GameController.CompletedDungeonList.Contains(GameController.TriggeredDungeonSeed)){
            Destroy(this);
        }
        interiorLight = GetComponentInChildren<Light>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () { 

        var t = Mathf.PingPong (Time.time, duration) / duration;
        interiorLight.color = Color.Lerp (color1, color2, t);
        interiorLight.intensity = UnityEngine.Random.Range(0.3f, 0.6f);
	}
}
