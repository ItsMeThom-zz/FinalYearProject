using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Assets.Code;
using System.Linq;
using Utils;

public class DungeonGenerator : MonoBehaviour {

    public GameController GameData; //reference to needed gamedata object;

    public Dictionary<string, GameObject> DungeonPrefabs;
    GameObject StartRoom_Prefab;
    GameObject BossRoom_Prefab;
    GameObject Wall_Prefab;
    // Use this for initialization

    public int Iterations = 40;
    public int Seed = 100;
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
        
        this.LoadPrefabs();
    }
    void Start () {
        
        DungeonWorldPos = this.gameObject.transform.position;
        //this.Gen(); //debugging only
    }
    


    public void Generate()
    {
        this.Seed = GameData.TriggeredDungeonSeed;
        this.LoadPrefabs();
        this.PlacedPrefabs = new List<GameObject>();
        this.PlacedWalls = new List<GameObject>();
        BeginInstanceGeneration(Seed);
    }

    private void BeginInstanceGeneration(int seed)
    {
        int instanceIterations = Iterations;
        instanceIterations = 100;
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
        ConfigureDungeonExit(start);
        PlacedPrefabs.Add(start);
       
        while (instanceIterations > 0)
        {
            //break if we run out of exits before we finish the iterations.
            // note: this should *never* happen.
            // becaue it means we cannot place a boss room.
            if(availableExits.Count == 0)
            {
                break;
            }
            Transform marker = availableExits.Pop();
            List<string> partsnames = new List<string>(this.DungeonPrefabs.Keys);
            partsnames.Shuffle();

            bool partFits = false;
            while(!partFits && partsnames.Count > 0)
            {
                string partName = partsnames.Pop();
                GameObject selectedPart = Instantiate(this.DungeonPrefabs[partName]);
                List<Transform> partExits = selectedPart.GetComponent<DungeonPrefabController>().GetAllExits();
                Transform usedExit = null;
                //check each available exit orientation on the new part to see if it collides with existing part
                partFits = TryPlacingPart(selectedPart, marker, out usedExit);
                if (partFits)
                {
                    print("Part Fits! Used Exit: " + usedExit.position);
                    //handle coincidental connections
                    HandleCoincedentalConnections(partExits, availableExits);
                    PlacedPrefabs.Add(selectedPart);
                    partExits.Remove(usedExit);
                    availableExits.AddRange(partExits);
                }
                else
                {
                    Destroy(selectedPart);
                }
               
            }
            //no new part fit here, place wall instead
            if (!partFits)
            {
                PlaceWallAtExit(marker);
            }
            instanceIterations--;
        }

        //closing off unused exits at the end of generation
        if(availableExits.Count > 0)
        {
            Transform usedExit = null;
            var BossRoom = Instantiate(BossRoom_Prefab);
            int count = availableExits.Count - 1;
            bool bossPlaced = false;
            while(count >= 0)
            {
                if (!bossPlaced)
                {
                    List<Transform> bossExit = BossRoom.GetComponent<DungeonPrefabController>().GetAllExits();
                    var fits = TryPlacingPart(BossRoom, availableExits[count], out usedExit);
                    if (fits)
                    {
                        //print("BOSS ROOM FITS!");
                        PlacedPrefabs.Add(BossRoom);
                        bossPlaced = true;
                        //break;
                    }
                    else
                    {
                        //print("BOSS NO FIT. PLACE WALL");
                        PlaceWallAtExit(availableExits[count]);
                    }
                }
                else
                {
                    //print("JUST PLACING WALLS NOW");
                    PlaceWallAtExit(availableExits[count]);
                }
                count--;
            }
        }
        RemovePlacementColliders();
        //Add and initalise the grammar engine to rewrite the room contents
        //var grammarEngine = gameObject.AddComponent<GenerativeGrammar.GrammarEngine>();
        this.IsReady = true;

    }

    private bool TryPlacingPart(GameObject selectedPart, Transform marker, out Transform usedExit)
    {
        bool fits = false;
        usedExit = null;
        List<Transform> partExits = selectedPart.GetComponent<DungeonPrefabController>().GetAllExits();
        partExits.Shuffle();
        //check each available exit orientation on the new part to see if it collides with existing part
        foreach (Transform exit in partExits)
        {
            GameObject mover = selectedPart.GetComponent<DungeonPrefabController>().OrientExit(exit);
            mover.transform.position = marker.transform.position;
            mover.transform.rotation = marker.transform.rotation;
            mover.transform.Rotate(new Vector3(0, 180, 0)); //match exit position and rotate
            //checking if this orientation causes a collision with an existing part
            if (!CollidesWithExistingPart(selectedPart))
            {
                //this part fits without collisions, mark its used exits
                //print("NO COLLIDE");
                fits = true;
                usedExit = exit;
                selectedPart.transform.parent = this.transform;
                Destroy(mover);
                break;
            }
        }
        return fits;
    }

    private bool CollidesWithExistingPart(GameObject selectedPart)
    {
        bool someCollision = false;
        foreach (var placedPart in PlacedPrefabs)
        {
            //check if the collision is with the part were placing against
            DungeonPrefabController placed = placedPart.GetComponent<DungeonPrefabController>();
            if (placed == null) { Debug.Log("DUNGEON PART SHOULD NOT BE NULL"); }

            if (selectedPart.GetComponent<DungeonPrefabController>().Intersects(placed.parentMesh.bounds))
            {
                //Debug.Log(selectedPart.name + " is colliding");
                //selectedPart.name += " colliding";
                someCollision = true;
                
            }
        }
        return someCollision;
    }

    private void HandleCoincedentalConnections(List<Transform> partExits, List<Transform> availableExits)
    {
        List<Transform> coincidentals = new List<Transform>();
        foreach (Transform ext in partExits)
        {
            foreach (Transform existingExit in availableExits)
            {
                if (ext.transform.position == existingExit.transform.position)
                {
                    //Debug.Log("Coincidental connection identified");
                    coincidentals.Add(existingExit);
                    coincidentals.Add(ext);
                }
            }
        }
        foreach (var t in coincidentals)
        {
            availableExits.Remove(t);
        }
    }

    private void PlaceWallAtExit(Transform marker)
    {
        GameObject wall = Instantiate(this.Wall_Prefab);
        wall.transform.position = marker.transform.position;
        wall.transform.rotation = marker.transform.rotation;
        wall.transform.parent = this.transform;
        PlacedWalls.Add(wall);
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

    private void ConfigureDungeonExit(GameObject start)
    {
        //mark the spawnpoint
        this.SpawnMarker = start.gameObject.transform.Find("SpawnMarker").transform.position;
        //configure the exit so we can leave the dungeon
        var exitMarker = start.gameObject.transform.Find("DungeonExit").gameObject;
        SetExitTriggerPosition(exitMarker);
        this.IsReady = true;
    }

    public void LoadPrefabs()
    {
        //load the unique rooms (boss, start room, etc)

        this.StartRoom_Prefab = (GameObject)Resources.Load("donjon/prefabs/special/Entrance1", typeof(GameObject));
        this.BossRoom_Prefab = (GameObject)Resources.Load("donjon/prefabs/special/bossroom", typeof(GameObject));
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
