using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;
using UnityEngine;
using Utils;

namespace WorldGen
{
    public enum FeatureType { DungeonEntrance, Town,
        None
    }

    public abstract class ChunkFeatureGenerator
    {

        public int Seed { get; set; }

        public Vector2i ChunkPosition { get; set; }
        public FeatureType FeatureType { get; set; }

        public List<GameObject> FeatureAssets { get; set; }


        public abstract void GenerateFeature();

        //wipes out assets associated with this chunks features (Walls, rocks, towers etc)
        public void DestroyAssets()
        {
            while (FeatureAssets.Count > 0)
            {
                var asset = FeatureAssets[0];
                GameObject.Destroy(asset);
                FeatureAssets.RemoveAt(0);
            }
            FeatureAssets = null;
        }

        /// <summary>
        /// Pick a random point within the chunk, not on the edge
        /// </summary>
        /// <returns></returns>
        public Vector2i GetRandomPointInChunk()
        {
            int x = UnityEngine.Random.Range(32, TerrainChunkSettings.ChunkSize - 32);
            int z = UnityEngine.Random.Range(32, TerrainChunkSettings.ChunkSize - 32);
            return new Vector2i(x, z);
        }

    }

    internal class Feature{
        GameObject Asset      { get; set; }
        Vector3 WorldPolition { get; set; }
        Quaternion Rotation   { get; set; }
    }
}
