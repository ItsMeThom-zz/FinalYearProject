using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class TreeCanopySpawner : MonoBehaviour {

    // Use this for initialization
    GameObject canopy;
    GameObject foliageObj;
    List<GameObject> FoliageList;
    
	void Start () {
        
        FoliageList = new List<GameObject>();
        foliageObj = (GameObject)Resources.Load("Trees/Leaves-sprite");
        GameObject tree = (GameObject)Resources.Load("Trees/tree_canopy_basic");
        canopy = Instantiate(tree, Vector3.zero, Quaternion.identity);
        //var scalemod = UnityEngine.Random.Range(-0.4f, 0.5f);
        //Vector3 randomSize = new Vector3(scalemod, scalemod, scalemod);
        //canopy.transform.localScale = canopy.transform.localScale + randomSize;

        var canopymesh = canopy.GetComponentInChildren<MeshFilter>().mesh;
        
        for (int i = 0; i < 120; i++)
        {
            var point = RandomPointOnMesh.GetRandomPointOnMesh(canopymesh);
            //cast back to mesh from point and get normal!
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(point, Vector3.down, out hit, 5))
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.Log(hit.normal);
            }

                //var normal = hit.normal;

                //var direction = point = hit.point;

                //point += randomSize;
                var randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            var spawnedmesh = Instantiate(foliageObj, point, randomRotation);
            spawnedmesh.transform.parent = canopy.transform;
            var leafscale = UnityEngine.Random.Range(-0.1f, 0.1f);
            Vector3 leaveSize = new Vector3(leafscale, leafscale, leafscale);
            spawnedmesh.transform.localScale = spawnedmesh.transform.localScale + leaveSize;
            //var meshes = spawnedmesh.GetComponentsInChildren<MeshFilter>();
            //foreach(var m in meshes)
            //{
            //    var newNormals = new Vector3[m.mesh.normals.Length];
            //    newNormals.Populate(normal);
            //    m.mesh.normals = newNormals;
            //}
            var leafMeshes = spawnedmesh.GetComponentsInChildren<MeshFilter>();
            foreach(var mesh in leafMeshes)
            {
                mesh.mesh.RecalculateNormals();
            }
        }
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
