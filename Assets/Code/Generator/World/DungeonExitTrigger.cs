using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonExitTrigger : MonoBehaviour {

    public Vector3 ReturnToWorldPosition { get; set; }

    private void OnCollisionEnter(Collision other)
    {
        //if the player collides
        //set his position in GameData (And weapons, inv, etc should already be there)
        //Set Dungeon Seed in GameData based on ChunkCoords + some other fixed value
        //Load Dungeon Scene
        //Check is Ready:
        //if ready: set active.
        if (other.gameObject.tag == "Player")
        {
            GameController Gamedata = GameController.GetSharedInstance();
            Gamedata.PlayerInDungeon = false;
            Gamedata.DungeonGenerator.Cleanup();
            var torch = other.gameObject.transform.Find("Torch");
            if (torch != null)
            {
                Destroy(torch.gameObject);
            }
            other.transform.position = ReturnToWorldPosition;
        }
    }
}
