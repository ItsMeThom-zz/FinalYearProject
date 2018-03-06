using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPrefabController : MonoBehaviour {


    public List<Transform> Exits;
    public List<Transform> UsedExits;

    public bool Colliding = false;
    public MeshCollider parentMesh;

    private void Awake()
    {
        //basicall a ctor
        this.Exits = GetAllExits();
        this.UsedExits = new List<Transform>();
     
        this.generateStaticCollisionMesh();
    }
    // Use this for initialization
    void Start () {

        

    }

    /// <summary>
    /// Returns the transforms of all exit markers in this prefab object
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetAllExits() {
        List<Transform> exits = new List<Transform>();

        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.name.Contains("ExitMarker"))
            {
               
                exits.Add(child);
            }
        }
        return exits;
    }


    /// <summary>
    /// Creates a temporary parent object at the position of the selectedChild
    /// so that transforms applied to the temporary parent move the whole object
    /// </summary>
    /// <param name="selectedChild"></param>
    /// <returns></returns>
    public GameObject OrientExit(Transform selectedChild)
    {
      
        GameObject obj = new GameObject();
        obj.transform.position = selectedChild.transform.position;
        obj.transform.rotation = selectedChild.transform.rotation;

        //obj.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
        if(gameObject.transform.parent != null)
        {
            GameObject oldparent = gameObject.transform.parent.gameObject;
            gameObject.transform.parent = null;
            Destroy(oldparent);
        }
        gameObject.transform.SetParent(obj.gameObject.transform);
        
        return obj;
    }



    public void generateStaticCollisionMesh()
    {
        
        MeshCollider collider = this.transform.Find("main").gameObject.GetComponentInChildren<MeshCollider>();
        collider.convex = true;
     
        collider.isTrigger = true;

        parentMesh = null;
        parentMesh = gameObject.AddComponent<MeshCollider>();

        parentMesh.convex = true;
      
        parentMesh.isTrigger = true;

        parentMesh.sharedMesh = collider.sharedMesh;
    }



    

    public bool Intersects(Bounds bounds)
    {
        if(parentMesh == null) { Debug.Log("Im being interesected but I have no mesh"); this.generateStaticCollisionMesh(); };
        return parentMesh.bounds.Intersects(bounds);
    }




}
