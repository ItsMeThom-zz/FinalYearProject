using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;
using UnityEngine;
using Utils;

namespace WorldGen
{
    /// <summary>
    /// Awake: Gets a reference to the terraindata its attached to
    /// SetFeature: Sets the feature type for this chunk, plus any seed info
    /// 
    /// </summary>

    public class ChunkFeatureComponent_old : MonoBehaviour
    {
        List<GameObject> FeatureAssets;
        ChunkFeatureGenerator Generator;
        Terrain ChunkTerrain;
        public TerrainChunk ParentChunk;
        public Vector2i ChunkCoords { get; set; }
        private int Seed;

        
        private void Awake()
        {
            FeatureAssets = new List<GameObject>();
            ChunkTerrain = this.transform.parent.GetComponent<Terrain>();
            
            if(ParentChunk == null) { Debug.Log("FUUUUCK"); }
        }


        public void SetFeature(FeatureType feature, int seed)
        {
            this.Seed = seed;
            switch (feature)
            {
                case FeatureType.DungeonEntrance:
                    Generator = new DungeonFeature(seed, ChunkCoords);
                    break;
                case FeatureType.Town:
                    //no towns in this version!
                    break;
            }

        }


        public void Generate()
        {
            UnityEngine.Random.InitState(Seed);

            //pick a random position for the feature(s)
            var position = this.Generator.GetRandomPointInChunk();
            float sampleheight = ParentChunk.Terrain.terrainData.GetHeight(position.X, position.Z);
            Vector3 worldPosition = new Vector3(
                (ChunkCoords.X * TerrainChunkSettings.ChunkSize) + position.X,
                sampleheight - 0.5f,
                (ChunkCoords.Z * TerrainChunkSettings.ChunkSize) + position.Z
                );
            var randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            GameObject dungeontower = (GameObject)Resources.Load("DungeonTower");
            var spawntower = Instantiate(dungeontower, worldPosition, randomRotation);
            Generator.FeatureAssets.Add(
                spawntower
                );

            
            //setting the seed for the dungeon in the tower we just placed
            DungeonEntranceTrigger enterancetrigger = spawntower.gameObject.transform.GetComponentInChildren<DungeonEntranceTrigger>();
            enterancetrigger.Seed = ChunkCoords.X * ChunkCoords.Z + 123456;

            //raycasting the tower to the ground level
            float distanceToGround = TerrainUtils.RaycastToTerrain(spawntower.transform.position);
            var pos = spawntower.transform.position;
            pos = new Vector3(pos.x, (pos.y + distanceToGround), pos.z);
            spawntower.transform.position = pos;
            spawntower.transform.SetParent(this.transform);
            RemoveOverlappingAssets();
        }

        public void Destroy()
        {
            Generator.DestroyAssets();
        }

        public void RemoveOverlappingAssets()
        {
            //remove trees
            var treeComponent = this.ParentChunk.TreesComponent.GetComponent<ChunkTreeGenerator>();
            var FeatureCollider = Generator.FeatureAssets[0].GetComponentInChildren<Renderer>(); //get dungeon collider
            List<GameObject> treesForRemoval = new List<GameObject>();
            foreach (var tree in treeComponent.PlacedAssets)
            {
                var collider = tree.GetComponentInChildren<Renderer>();
                if (FeatureCollider.bounds.Intersects(collider.bounds))
                {
                    treesForRemoval.Add(tree);
                }
            }
            treeComponent.DestroyAssetRange(treesForRemoval);
            //var rockscomponent = ParentChunk.ChunkRocks;
            //List<GameObject> rocksForRemoval = new List<GameObject>();
            //foreach(var rock in rockscomponent.PlacedAssets)
            //{
            //    var collider = rock.GetComponentInChildren<Renderer>();
            //    if (FeatureCollider.bounds.Intersects(collider.bounds))
            //    {
            //        rocksForRemoval.Add(rock);
            //    }
            //}
            //rockscomponent.DestroyRockRange(rocksForRemoval);

        }
    }

    public class ChunkFeatureComponent : MonoBehaviour
    {
        #region Static Prefabloader + resource path
        private static List<GameObject> AssetPrefabs;
        private static string RESOURCE_PATH = "DungeonEntrance/Prefabs";
        #endregion

        private GameController GameController;

        public TerrainChunk ParentChunk { get; set; }
        public List<GameObject> PlacedAssets { get; set; }


        //component specific stuff
        public int Seed { get; set; }
        public ChunkFeatureGenerator FeatureGenerator { get; set; }

        #region static loader
        protected static void LoadPrefabs()
        {
            AssetPrefabs = new List<GameObject>();
            UnityEngine.Object[] loadedObj = Resources.LoadAll(ChunkFeatureComponent.RESOURCE_PATH, typeof(GameObject));

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
            //Singleton method for loading prefabs
            if (ChunkFeatureComponent.AssetPrefabs == null)
            {
                ChunkFeatureComponent.LoadPrefabs();
            }
            this.GameController = GameController.GetSharedInstance();
            this.PlacedAssets = new List<GameObject>();
           
        }

        public void SetFeature(FeatureType feature, int seed)
        {
            this.Seed = seed;
            switch (feature)
            {
                case FeatureType.DungeonEntrance:
                    FeatureGenerator = new DungeonFeature(seed, ParentChunk.Position);
                    break;
                case FeatureType.Town:
                    //no towns in this version!
                    break;
            }

        }

        public void Generate()
        {
            UnityEngine.Random.InitState(Seed);

            //pick a random position for the feature(s)
            var position = this.FeatureGenerator.GetRandomPointInChunk();
            float sampleheight = ParentChunk.Terrain.terrainData.GetHeight(position.X, position.Z);
            //set the offset for placing in the chunks world position X, Z. uses terrain height for Y
            Vector3 offsetPosition = new Vector3(
                (ParentChunk.Position.X * ParentChunk.Terrain.terrainData.size.x) + position.X,
                ParentChunk.Terrain.terrainData.GetHeight(position.X, position.Z),
                (ParentChunk.Position.Z * ParentChunk.Terrain.terrainData.size.z) + position.Z
                );
            Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            GameObject asset = ChooseRandomAsset();
            GameObject spawnedAsset = Instantiate(asset, offsetPosition, randomRotation);
            PlacedAssets.Add(spawnedAsset);
            spawnedAsset.transform.parent = ParentChunk.FeatureComponent.transform;

            //setting the seed for the dungeon in the tower we just placed
            DungeonEntranceTrigger enterancetrigger = spawnedAsset.gameObject.transform.GetComponentInChildren<DungeonEntranceTrigger>();
            enterancetrigger.Seed = ParentChunk.Position.X * ParentChunk.Position.Z + 123456;
            RemoveOverlappingAssets(spawnedAsset);
        }

        public void RemoveOverlappingAssets(GameObject feature)
        {
            var radius = feature.GetComponentInChildren<MeshRenderer>().bounds.extents.magnitude;
            var center = feature.transform.position;
            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            int i = 0;
            while (i < hitColliders.Length)
            {
                if (hitColliders[i].gameObject.tag != "Terrain")
                {
                    if (hitColliders[i].transform.parent.gameObject.tag == "Tree")
                    {

                        ParentChunk.TreesComponent.DestroyAsset(hitColliders[i].transform.parent.gameObject);
                    }
                    else if(hitColliders[i].transform.parent.gameObject.tag == "Rock")
                    {
                        ParentChunk.RocksComponent.DestroyAsset(hitColliders[i].transform.parent.gameObject);
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

    }
}
