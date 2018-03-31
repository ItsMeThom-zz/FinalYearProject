using LibNoise.Generator;
using System.Collections;
using System.Collections.Generic;
using TerrainGenerator;
using UnityEngine;
using Utils;

public class ChunkTreeGenerator : MonoBehaviour
{
    #region Static Prefabloader + resource path
    private static List<GameObject> AssetPrefabs;
    private static string RESOURCE_PATH = "Trees/Prefabs";
    #endregion

    private GameController GameController;

    public TerrainChunk ParentChunk      { get; set; }
    public List<GameObject> PlacedAssets { get; set; }


    //random distributor
    PoissonDiscSampler sampler;


    //additional component-specific features
    Perlin sparseMask;

    #region static methods
    protected static void LoadPrefabs()
    {
        AssetPrefabs = new List<GameObject>();
        UnityEngine.Object[] loadedObj = Resources.LoadAll(ChunkTreeGenerator.RESOURCE_PATH, typeof(GameObject));

        for (int i = 0; i < loadedObj.GetLength(0); i++)
        {
            AssetPrefabs.Add(loadedObj[i] as GameObject);
        }
    }

    public static GameObject ChooseRandomAsset()
    {
        int i = UnityEngine.Random.Range(0, AssetPrefabs.Count);
        return AssetPrefabs[i];
    }

    #endregion

    private void Awake()
    {
        if(ChunkTreeGenerator.AssetPrefabs == null)
        {
            ChunkTreeGenerator.LoadPrefabs();
        }
        this.GameController = GameController.GetSharedInstance();
        this.PlacedAssets   = new List<GameObject>();
        this.sampler        = new PoissonDiscSampler();
        this.sparseMask     = new Perlin()
        {
            OctaveCount = 8,
            Frequency = 1.7,
            Persistence = 0.6
        };
    }

    public void Generate()
    {
        sampler.Radius = Random.Range(8, 81);
        var points = sampler.GeneratePoints((int)ParentChunk.Terrain.terrainData.size.x);
        for (var t = 0; t < points.Count; t++)
        {
            if (points[t] != null)
            {
                Vector3 pos = (Vector3)points[t];
               
                //sinking into terrain to hide seams
                float newY = ParentChunk.Terrain.terrainData.GetHeight((int)pos.x, (int)pos.z) - 0.08f; 
                if (newY > TerrainChunkSettings.SeaLevelGlobal && sparseMask.GetValue(pos.x, 0, pos.z) < 0.15f)
                {

                    //set the offset
                    Vector3 offsetPosition = new Vector3(
                        (ParentChunk.Position.X * ParentChunk.Terrain.terrainData.size.x),
                        newY, 
                        (ParentChunk.Position.Z * ParentChunk.Terrain.terrainData.size.z)
                        );

                    //random horizontal rotation
                    Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
                    
                    //set random scale/size adjustments
                    float scaleModifier = UnityEngine.Random.Range(-0.3f, 0.3f);
                    Vector3 randomScale = new Vector3(scaleModifier, scaleModifier, scaleModifier);
                    var spawnPosition = pos + offsetPosition;

                    GameObject asset = ChooseRandomAsset();
                    GameObject spawnedAsset = Instantiate(asset, spawnPosition, randomRotation);
                    spawnedAsset.name = "Tree (Valid)";
                    spawnedAsset.transform.localScale = spawnedAsset.transform.localScale + randomScale;
                    spawnedAsset.transform.parent = this.transform;
                    PlacedAssets.Add(spawnedAsset);
                    RemoveOverlappingAssets(spawnedAsset);
                }
            }
            
        }
    }



    #region AssetRemoval
    private void RemoveOverlappingAssets(GameObject tree)
    {
        

    }


    //destroy a single asset
    public void DestroyAsset(GameObject asset)
    {
        if (PlacedAssets.Contains(asset))
        {
            asset.name = "DESTROYED TREE";
            PlacedAssets.Remove(asset);
            Destroy(asset);
           
        }
    }

    //Desttroy a range of assets
    public void DestroyAssetRange(List<GameObject> assetList)
    {
        foreach (var asset in assetList)
        {
            DestroyAsset(asset);
        }
    }

    //destroy all trees
    public void DestroyAllAssets()
    {
        foreach (var asset in PlacedAssets)
        {
            GameObject.Destroy(asset);
        }
    }
    #endregion
}

#region old Raycasting code. kept for reference
//private bool IsValidPosition(ref Vector3 position)
//{
//    #region Summary of Code (For report)
//    /*
//     Simple Summary of raycast steps for tree placement
//     Raycast down:
//        If hit:
//            if terrain:
//                if > sealevel:
//                    valid
//                    down = true
//                else:
//                    not valid
//     if not down:
//        raycast up:
//            if hit:
//                if terrain:
//                    if > sealevel
//                        valid
//                    else: not valid
//                else:
//                    not valid
//     */
//    #endregion
//    bool placeable = true;
//    bool downHandled = false;
//    RaycastHit hit = new RaycastHit();
//    if (Physics.Raycast(position, Vector3.down, out hit, 1000))
//    {

//        if (hit.collider.gameObject.tag == "Terrain")
//        {
//            Debug.DrawLine(position, hit.point, Color.red, 500f);
//            if (hit.point.y < 10.5f) //below global sealevel
//            {
//                //Debug.DrawLine(position, hit.point, Color.blue, 500f);
//                placeable = false;
//            }
//            else
//            {
//                //Debug.DrawLine(position, hit.point, Color.red, 500f);
//                position = new Vector3(position.x, hit.point.y - 0.08f, position.z);
//            }
//            downHandled = true;
//        }
//        //Debug.DrawLine(position, hit.point, Color.yellow, 500f);
//    }
//    if (!downHandled)
//    {
//        if (Physics.Raycast(position, Vector3.up, out hit, 1000))
//        {
//            //Debug.DrawLine(position, hit.point, Color.green, 500f);

//            if (hit.collider.gameObject.tag == "Terrain")
//            {

//                if (hit.point.y < 10.5f) //below global sealevel
//                {
//                    //Debug.Log("I am below sealevel");
//                    //Debug.DrawLine(position, hit.point, Color.blue, 500f);
//                    placeable = false;
//                }
//                else
//                {
//                    //Debug.DrawLine(position, hit.point, Color.green, 500f);
//                    position = new Vector3(position.x, hit.point.y - 0.08f, position.z);
//                }

//            }
//            //Debug.DrawLine(position, hit.point, Color.white, 500f);
//        }
//        else
//        {
//            //Debug.Log("Hit nothing! FFS");
//            placeable = false;
//        }
//    }


//    return placeable;
//}
#endregion