using LibNoise.Generator;
using System.Collections;
using System.Collections.Generic;
using TerrainGenerator;
using UnityEngine;

public class ChunkTreeGenerator : MonoBehaviour {

    public int ChunkSize = 129;

    public Vector2i ChunkPosition;

    private PoissonDiscSampler sampler;
    public Terrain ChunkTerrain;
    public float[,] ChunkHeightmap;

    private Perlin sparseMask;
    private float SeaLevel = 0.4f;

    float[,] treePoints;

    List<GameObject> TreesList;

    // Use this for initialization
    private void Awake()
    {
        this.sampler = new PoissonDiscSampler();

        this.sparseMask = new Perlin()
        {
            OctaveCount = 8,
            Frequency = 1.7,
            Persistence = 0.6
        };

        TreesList = new List<GameObject>();
    }


    public void GenerateTrees()
    {
        //Debug.Log("CHUNK SIZE: " + ChunkTerrain.terrainData.size.x);
        var points = sampler.GeneratePoints((int)ChunkTerrain.terrainData.size.x);
        for(var t = 0; t < points.Count; t++)
        {
            if(points[t] != null)
            {
                Vector3 pos = (Vector3)points[t];
                var chunkheight = ChunkHeightmap[(int)pos.x, (int)pos.z];
                if ( (sparseMask.GetValue(pos.x, 0, pos.z) < 0.4) && ( chunkheight > SeaLevel))
                {
                    //set the offset
                    var offsetPosition = new Vector3(
                        (ChunkPosition.X * ChunkTerrain.terrainData.size.x),
                        ChunkTerrain.SampleHeight(pos),
                        (ChunkPosition.Z * ChunkTerrain.terrainData.size.z)
                        );

                    //random horizontal rotation
                    var randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
                    //set random scale adjustments
                    var scalemod = UnityEngine.Random.Range(-0.3f, 0.3f);
                    Vector3 randomSize = new Vector3(scalemod, scalemod, scalemod);

                    //TODO: Get a tree from Generator now
                    

                    GameObject tree = (GameObject)Resources.Load("BTree");
                    var spawnposition = pos + offsetPosition;
                    var spawned = Instantiate(tree, offsetPosition, randomRotation);
                    spawned.transform.position = spawnposition;
                    spawned.transform.localScale = spawned.transform.localScale + randomSize;
                    spawned.transform.parent = this.transform;


                    var modifiedPos = spawned.transform.position;
                    if (IsValidPosition(ref modifiedPos))
                    {
                        spawned.transform.position = modifiedPos;
                        TreesList.Add(spawned);
                    }
                    else
                    {
                        Destroy(spawned);
                    }

                }
            }
        }

        //Debug.Log("This chunk: " + ChunkPosition.X + ", " + ChunkPosition.Z + "has " + TreesList.Count + " trees");
    }

    private bool IsValidPosition(ref Vector3 position)
    {
        bool placeable = true;

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(position, Vector3.down, out hit, 200))
        {

            if (hit.collider.gameObject.name == "Terrain")
            {
                //Debug.DrawLine(position, hit.point, Color.red, 500f);
                if (hit.point.y < 10.5f) //below global sealevel
                {
                    
                    placeable = false;
                }
                else
                {
                    position = new Vector3(position.x, hit.point.y - 0.08f, position.z);
                }

            }
            //Debug.DrawLine(position, hit.point, Color.blue, 500f);
        }
        else if (Physics.Raycast(position, Vector3.up, out hit, 200))
        {
            //Debug.DrawLine(position, hit.point, Color.green, 500f);

            if (hit.collider.gameObject.name == "Terrain")
            {
                if (hit.point.y < 10.5f) //below global sealevel
                {
                    //Debug.Log("I am below sealevel");
                    placeable = false;
                }
                else
                {
                    position = new Vector3(position.x, hit.point.y - 0.08f, position.z);
                }
                
            }
            //Debug.DrawLine(position, hit.point, Color.yellow, 500f);
        }
        else
        {
            placeable = false;
        }

        return placeable;
    }


    public void DestroyAllTrees()
    {
        foreach(var tree in TreesList)
        {
            GameObject.Destroy(tree);
        }
    }
}
