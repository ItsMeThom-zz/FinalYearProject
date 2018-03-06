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

    public class ChunkFeatureComponent : MonoBehaviour
    {
        List<GameObject> FeatureAssets;
        ChunkFeatureGenerator Generator;
        Terrain ChunkTerrain;
        public Vector2i ChunkCoords { get; set; }
        private int Seed;

        
        private void Awake()
        {
            FeatureAssets = new List<GameObject>();
            ChunkTerrain = this.transform.parent.GetComponent<Terrain>();
            
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
            float sampleheight = ChunkTerrain.SampleHeight(new Vector3(position.X, position.Z));
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

            var snap = spawntower.transform;
            var distanceToGround = TerrainUtils.RaycastToTerrain(snap.position);
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
            var treecomponent = this.transform.parent.GetComponentInChildren<ChunkTreeGenerator>();
            var FeatureCollider = Generator.FeatureAssets[0].GetComponentInChildren<Renderer>(); //get dungeon collider
            List<GameObject> treesForRemoval = new List<GameObject>();
            foreach (var tree in treecomponent.TreesList)
            {
                var collider = tree.GetComponentInChildren<Renderer>();
                if (FeatureCollider.bounds.Intersects(collider.bounds))
                {
                    treesForRemoval.Add(tree);
                }
            }
            treecomponent.DestroyTreeRange(treesForRemoval);
        }
    }
}
