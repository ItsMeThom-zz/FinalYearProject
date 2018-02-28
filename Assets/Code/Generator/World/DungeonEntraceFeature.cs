using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;
using UnityEngine;

namespace WorldGen
{
    public class DungeonFeature : ChunkFeatureGenerator
    {

        public DungeonFeature(int seed, Vector2i position)
        {
            this.Seed = seed;
            this.ChunkPosition = position;
            this.FeatureAssets = new List<GameObject>();
        }
        public override void GenerateFeature()
        {
           /*
            Unused as of yet. In future versions, feature creations (Wether Grammar-Rewriting or Prefabbing will
            occur here and be stored in the FeatureAssets list. 
            
            This list is accessed by ChunkFeatureGeneratorComoponent
            which will simply instantate the object at its position
             */
        }
    }
}
