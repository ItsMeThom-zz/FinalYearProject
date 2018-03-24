using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonEntranceTrigger : MonoBehaviour {

    public int Seed { get; set; }
    private GameController GameController;
	// Use this for initialization
	void Start () {
        GameController = GameController.GetSharedInstance();
        this.Seed = (int)this.gameObject.transform.position.x + (int)this.gameObject.transform.position.y + GameController.BaseSeed;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision other)
    {
        //if the player collides
        //set his position in GameData (And weapons, inv, etc should already be there)
        //Set Dungeon Seed in GameData based on ChunkCoords + some other fixed value
        //Load Dungeon Scene
        //Check is Ready:
        //if ready: set active.
        if(other.gameObject.tag == "Player")
        {
            GameController controller = GameController.GetSharedInstance();
            controller.TiggeredDungeonEnterance = this.transform.Find("ExitPosition").transform.position;
            controller.TriggeredDungeonSeed = Seed;
            //DungeonGenerator donjon = GameObject.FindObjectOfType<DungeonGenerator>();
            controller.DungeonGenerator.Generate();
            controller.PlayerInDungeon = true;
            Vector3 spawn = controller.DungeonGenerator.SpawnMarker;
            GameObject torch = Instantiate((GameObject)Resources.Load("Torch"));
            torch.transform.position = other.transform.position;
            torch.transform.rotation = other.transform.rotation;
            torch.transform.parent = other.transform;
            torch.name = "Torch";
            other.transform.position = spawn;
            other.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

}
