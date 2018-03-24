using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Assets.Code;
using System.Linq;
using Utils;

public class DungeonGenerator : MonoBehaviour {

    public GameController GameData; //reference to needed gamedata object;

    public Dictionary<string, GameObject> DungeonPrefabs;
    GameObject StartRoom_Prefab;
    GameObject Wall_Prefab;
    // Use this for initialization

    public int Iterations = 140;
    public int Seed = 233;
    public float Pause = 0.01f;

    private List<GameObject> PlacedPrefabs;
    private List<GameObject> PlacedWalls;

    public Vector3 SpawnMarker { get; set; }
    public Vector3 ExitWorldPosition { get; set; }

    Vector3 DungeonWorldPos; //position of the dungeon in world space

    

    public bool IsReady { get; private set; }

    private void Awake()
    {
        GameData = GameController.GetSharedInstance();
        GameData.DungeonGenerator = this;
        //this.Seed = GameData.TriggeredDungeonSeed;
        this.loadPrefabs();
    }
    void Start () {
        
        DungeonWorldPos = this.gameObject.transform.position;
        //this.Gen(); //debugging only
    }
    


    public void Generate()
    {
        this.loadPrefabs();
        this.PlacedPrefabs = new List<GameObject>();
        this.PlacedWalls = new List<GameObject>();
        Gen(Seed);
    }

    private void Gen(int seed)
    {
        int instanceIterations = Iterations;
        //move
        Random.InitState(seed);

        //start room
        GameObject start = (Instantiate(StartRoom_Prefab, DungeonWorldPos, Quaternion.identity));
        start.transform.parent = this.transform;
        //Add the startroom exit marker to list
        List<Transform> availableExits = new List<Transform>
        {
            start.gameObject.transform.Find("ExitMarker")
        };
        
        //mark the spawnpoint
        this.SpawnMarker = start.gameObject.transform.Find("SpawnMarker").transform.position;
        //configure the exit so we can leave the dungeon
        var exitMarker = start.gameObject.transform.Find("DungeonExit").gameObject;
        SetExitTriggerPosition(exitMarker);

        this.IsReady = true;
        PlacedPrefabs.Add(start);
       
        //Iterations = 40;
        while (instanceIterations > 0)
        {
            //Debug.Log("Iteration: " + Iterations);
            //Stop if we have no exit markers left
            if(availableExits.Count == 0)
            {
                Debug.Log("No Exits!");
                break;
            }
            Debug.Log("Available Exits: " + availableExits.Count);

            Transform marker = availableExits.Pop();

            List<string> partsnames = new List<string>(this.DungeonPrefabs.Keys);
            partsnames.Shuffle();

            bool fits = false;
            while(!fits && partsnames.Count > 0)
            {
                string partname = partsnames.Pop();
                GameObject selectedPart = Instantiate(this.DungeonPrefabs[partname]);
                Debug.Log("Trying [" + selectedPart.name + "]");
                List<Transform> partExits = selectedPart.GetComponent<DungeonPrefabController>().GetAllExits();
                Transform usedExit = null;
                //check each available exit orientation on the new part to see if it collides with existing part
                foreach(Transform exit in partExits)
                {
                    GameObject mover = selectedPart.GetComponent<DungeonPrefabController>().OrientExit(exit);
                    mover.transform.position = marker.transform.position;
                    mover.transform.rotation = marker.transform.rotation;
                    mover.transform.Rotate(new Vector3(0, 180, 0));

                    //checking if this orientation causes a collision with an existing part
                    bool colliding = false;
                    foreach (var placedPart in PlacedPrefabs)
                    {
                        //check if the collision is with the part were placing against
                        DungeonPrefabController placed = placedPart.GetComponent<DungeonPrefabController>();
                        if(placed == null) { Debug.Log("DUNGEON PART SHOULD NOT BE NULL"); }
                        
                        if (selectedPart.GetComponent<DungeonPrefabController>().Intersects(placed.parentMesh.bounds))
                        {
                            //Debug.Log(selectedPart.name + " is colliding");
                            colliding = true;
                            
                            Destroy(mover);
                        }
                    }

                    if (!colliding)
                    {
                        //this part fits without collisions, mark its used exits
                        // and add the rest to the available
                        fits = true;
                        usedExit = exit;
                        selectedPart.transform.parent = this.transform;
                        Destroy(mover);
                        
                        break;
                    }
                    
                }
                if (fits)
                {
                    //handle coincidental connections
                    List<Transform> coincidentals = new List<Transform>();
                    foreach(Transform ext in partExits)
                    {
                        foreach(Transform existingExit in availableExits)
                        {
                            if(ext.transform.position == existingExit.transform.position)
                            {
                                //Debug.Log("Coincidental connection identified");
                                coincidentals.Add(existingExit);
                                coincidentals.Add(ext);
                            }
                        }
                    }
                    foreach(var t in coincidentals)
                    {
                        availableExits.Remove(t);
                    }
                    PlacedPrefabs.Add(selectedPart);
                    partExits.Remove(usedExit);
                    availableExits.AddRange(partExits);
                    Debug.Log("Added Part: " + selectedPart.name);
                }
                else
                {
                    //Debug.Log("Couldnt place [" + selectedPart.name + "]");
                    Destroy(selectedPart);
                    
                }
               
            }
            //no new part fit here, place wall instead
            if (!fits)
            {
                // do a check here to see if this exitmarker postion
                // is the same as any other exit marker postion in usedParts
                // if it is, we've made an accidental conneciton
                // otherwise, just place the wall
                //Debug.Log("No part fits, I would place wall now");
                GameObject wall = Instantiate(this.Wall_Prefab);
                wall.transform.position = marker.transform.position;
                wall.transform.rotation = marker.transform.rotation;
                wall.transform.parent = this.transform;
                PlacedWalls.Add(wall);
            }
            instanceIterations--;
            //yield return new WaitForSecondsRealtime(this.Pause);
        }

        //closing off unused exits at the end of generation
        if(availableExits.Count > 0)
        {
            foreach(var openexit in availableExits)
            {
               
                GameObject wallfiller = Instantiate(this.Wall_Prefab);
                wallfiller.transform.position = openexit.transform.position;
                wallfiller.transform.rotation = openexit.transform.rotation;
                wallfiller.transform.SetParent(openexit.transform.parent);
                //Debug.Log("Rotation wall in ["+ openexit.transform.parent.name + "]to " + openexit.transform.eulerAngles);
            }
        }

        RemovePlacementColliders();
        //Add and initalise the grammar engine to rewrite the room contents
        var grammarEngine = gameObject.AddComponent<GenerativeGrammar.GrammarEngine>();
        this.IsReady = true;

    }


    private void SetExitTriggerPosition(GameObject entraceDoor)
    {
        var exit = entraceDoor.GetComponentInChildren<DungeonExitTrigger>();
        exit.ReturnToWorldPosition = GameData.TiggeredDungeonEnterance;
        GameData.PlayerInDungeon = false;
        GameData.DungeonGenerator.Cleanup();
    }
    //Turns off "Convex" on each placed object so the player can move inside them!
    private void RemovePlacementColliders()
    {
        foreach(var piece in PlacedPrefabs)
        {
            var colliders = piece.GetComponentsInChildren<MeshCollider>();
            for(int i = 0; i < colliders.Length; i++)
            {
                colliders[i].isTrigger = false;
                colliders[i].convex = false;
            }
        }
    }

    public void loadPrefabs()
    {
        //load the unique rooms (boss, start room, etc)

        this.StartRoom_Prefab = (GameObject)Resources.Load("donjon/prefabs/special/Entrance1", typeof(GameObject));
        this.Wall_Prefab = (GameObject)Resources.Load("donjon/prefabs/special/Wall", typeof(GameObject));
        this.DungeonPrefabs = new Dictionary<string, GameObject>();
        Object[] subListObjects = Resources.LoadAll("donjon/prefabs/parts", typeof(GameObject));
        //Object[] subListObjects = Resources.LoadAll("donjon/prefabs/parts", typeof(GameObject));
        foreach (GameObject subListObject in subListObjects)
        {
            GameObject lo = (GameObject)subListObject;
            this.DungeonPrefabs.Add(lo.name, lo);
            //Debug.Log(lo.name + " prefab loaded..");
        }


    }

    public void Cleanup()
    {
        //cleanup all dungeon components when the player leaves the dungeon
        while(this.PlacedPrefabs.Count > 0){
            var placed = this.PlacedPrefabs.Pop();
            Destroy(placed);
        }

        while (this.PlacedWalls.Count > 0)
        {
            var placed = this.PlacedWalls.Pop();
            Destroy(placed);
        }
    }



}
