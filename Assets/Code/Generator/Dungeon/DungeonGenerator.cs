using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Assets.Code;
using System.Linq;

public class DungeonGenerator : MonoBehaviour {

    public Dictionary<string, GameObject> DungeonPrefabs;
    GameObject StartRoom_Prefab;
    GameObject Wall_Prefab;
    // Use this for initialization

    public int Iterations = 20;
    public int Seed = 123456;
    public float Pause = 1.0f;

	void Start () {


        this.loadPrefabs();
        StartCoroutine(Generate(Seed));
        

    }

    private IEnumerator Generate(int seed)
    {
        //move
        Random.InitState(seed);

        //start room
        GameObject start = (Instantiate(StartRoom_Prefab, new Vector3(0, 0, 0), Quaternion.identity));

        //Add the startroom exit marker to list
        List<Transform> availableExits = new List<Transform>();
        availableExits.Add(start.gameObject.transform.Find("ExitMarker"));
        List<GameObject> usedParts = new List<GameObject>();
        usedParts.Add(start);

        while (Iterations > 0)
        {
            //Stop if we have no exit markers left
            if(availableExits.Count == 0)
            {
                break;
            }

            Transform marker = availableExits.Pop();

            List<string> partsnames = new List<string>(this.DungeonPrefabs.Keys);
            partsnames.Shuffle();

            bool fits = false;
            while(!fits && partsnames.Count > 0)
            {
                string partname = partsnames.Pop();
                GameObject selectedPart = Instantiate(this.DungeonPrefabs[partname]);
                //Debug.Log("Trying [" + selectedPart.name + "]");
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
                    foreach (var placedPart in usedParts)
                    {
                        //TODO: check if the collision is with the part were placing against
                        DungeonPrefabController placed = placedPart.GetComponent<DungeonPrefabController>();
                        //TODO; The bug is here, parentmesh is the seperated collider, not the one on the object for some reason
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
                        selectedPart.transform.parent = null;
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
                                Debug.Log("Coincidental connection identified");
                                coincidentals.Add(existingExit);
                                coincidentals.Add(ext);
                            }
                        }
                    }
                    foreach(var t in coincidentals)
                    {
                        availableExits.Remove(t);
                    }
                    usedParts.Add(selectedPart);
                    partExits.Remove(usedExit);
                    availableExits.AddRange(partExits);
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
            }
            Iterations--;
            yield return new WaitForSecondsRealtime(this.Pause);
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

        

    }

    

    private void loadPrefabs()
    {
        //load the unique rooms (boss, start room, etc)

        this.StartRoom_Prefab = (GameObject)Resources.Load("dungeon/prefabs/special/startroom", typeof(GameObject));
        this.Wall_Prefab = (GameObject)Resources.Load("dungeon/prefabs/special/wall", typeof(GameObject));
        this.DungeonPrefabs = new Dictionary<string, GameObject>();
        Object[] subListObjects = Resources.LoadAll("dungeon/prefabs/parts", typeof(GameObject));
        foreach (GameObject subListObject in subListObjects)
        {
            GameObject lo = (GameObject)subListObject;
            this.DungeonPrefabs.Add(lo.name, lo);
           // Debug.Log(lo.name + " prefab loaded..");
        }


    }



}
