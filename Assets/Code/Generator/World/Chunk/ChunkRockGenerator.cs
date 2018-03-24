using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;
using UnityEngine;
using Utils;

namespace WorldGen
{

    public class ChunkRockGenerator : MonoBehaviour
    {
        #region Static Prefabloader + resource path
        private static List<GameObject> AssetPrefabs;
        private static string RESOURCE_PATH = "Rocks/Prefabs";
        #endregion

        private GameController GameController;

        public TerrainChunk ParentChunk { get; set; }
        public List<GameObject> PlacedAssets { get; set; }


        //random distributor
        PoissonDiscSampler sampler;

        #region static methods
        protected static void LoadPrefabs()
        {
            AssetPrefabs = new List<GameObject>();
            UnityEngine.Object[] loadedObj = Resources.LoadAll(ChunkRockGenerator.RESOURCE_PATH, typeof(GameObject));

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
            if (ChunkRockGenerator.AssetPrefabs == null)
            {
                ChunkRockGenerator.LoadPrefabs();
            }
            this.GameController = GameController.GetSharedInstance();
            this.PlacedAssets = new List<GameObject>();
            this.sampler = new PoissonDiscSampler();

        }

        public void Generate()
        {
            //totally random distribution as Poisson doesnt look great.
            var points = FakeDistribution();

            for (var t = 0; t < points.Count; t++)
            {
                if (points[t] != null)
                {
                    //position to place at.
                    Vector3 pos = (Vector3)points[t];
                    if (ParentChunk.Terrain.terrainData.GetHeight((int)pos.x, (int)pos.z) > TerrainChunkSettings.SeaLevelGlobal)
                    {
                        //adjust rock specific height based on steepness to hide edges better
                        float normalizedX = pos.x / ParentChunk.Terrain.terrainData.size.x;
                        float normalizedY = pos.z / ParentChunk.Terrain.terrainData.size.z;
                        float slope = ParentChunk.Terrain.terrainData.GetSteepness(normalizedX, normalizedY);
                        float slopeOffsetHeight = (slope > 25.0f ? 3.2f : 0.5f);

                        //choose random rock type
                        GameObject chosenRock = ChunkRockGenerator.ChooseRandomAsset();

                        //set the offset for placing in the chunks world position X, Z. uses terrain height for Y
                        Vector3 offsetPosition = new Vector3(
                            (ParentChunk.Position.X * ParentChunk.Terrain.terrainData.size.x),
                            ParentChunk.Terrain.terrainData.GetHeight((int)pos.x, (int)pos.z) - slopeOffsetHeight,
                            (ParentChunk.Position.Z * ParentChunk.Terrain.terrainData.size.z)
                            );

                        Vector3 spawnPosition = pos + offsetPosition;
                        //gives rock random rotation in the y-axis
                        Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
                        float scaleModifer = UnityEngine.Random.Range(0.1f, 0.5f);
                        Vector3 randomSize = new Vector3(scaleModifer, scaleModifer, scaleModifer);
                        chosenRock.transform.localScale = randomSize;

                        GameObject spawnedAsset = Instantiate(chosenRock, spawnPosition, randomRotation);
                        //parent it under this component and track it
                        spawnedAsset.transform.parent = this.transform;
                        PlacedAssets.Add(spawnedAsset);
                        RemoveOverlappingAssets(spawnedAsset);
                    }
                }
                
            }
        }

        //Removes trees that collide with rocks
        private void RemoveOverlappingAssets(GameObject rock)
        {
            var radius = rock.GetComponentInChildren<MeshRenderer>().bounds.extents.magnitude;
            var center = rock.transform.position;
            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            int i = 0;
            while (i < hitColliders.Length)
            {
                if(hitColliders[i].gameObject.tag != "Terrain")
                {
                    if(hitColliders[i].transform.parent != null)
                    {
                        hitColliders[i].transform.name = "BROKEN OBJECT REFERENCE";
                    }
                    else if (hitColliders[i].transform.parent.gameObject.tag == "Tree")
                    {
                       
                        ParentChunk.TreesComponent.DestroyAsset(hitColliders[i].transform.parent.gameObject);
                    }
                }
                i++;
            }
        }

        #region AssetRemoval
        //destroy a single asset
        public void DestroyAsset(GameObject asset)
        {
            if (PlacedAssets.Contains(asset))
            {
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

        #region fake distribution testing
        private ArrayList FakeDistribution()
        {
            var points = new ArrayList();
            var num = UnityEngine.Random.Range(0, 15);
            for (int i = 0; i < num; i++)
            {
                var x = UnityEngine.Random.Range(0, 128);
                var z = UnityEngine.Random.Range(0, 128);
                points.Add(new Vector3(x, 0, z));
            }
            return points;
        }

        #endregion
    }

}

