using KNN;
using LibNoise.Operator;
using System.Threading;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WorldGen;
using LibNoise;
using LibNoise.Generator;
using Utils;

namespace TerrainGenerator
{
    public class TerrainChunk
    {
        public Vector2i Position { get; private set; }

        private Terrain Terrain { get; set; }

        private TerrainData Data { get; set; }

        private TerrainChunkSettings Settings { get; set; }

        private NoiseProvider NoiseProvider { get; set; }

        private TerrainChunkNeighborhood Neighborhood { get; set; }

        private WorldGenerator WorldGenerator { get; set; }

        private ChunkTreeGenerator TreeAssets { get; set; }

        public float[,] Heightmap { get; set; }

        public List<BiomeType> Biomes { get; set; }

        private object HeightmapThreadLockObject { get; set; }


        public Blend BlendModule { get; private set; }
        public bool HeightmapReady { get; private set; }
        public Perlin ChunkBlend { get; private set; }
        public GameObject ChunkFeature { get; private set; }

        public TerrainChunk(TerrainChunkSettings settings, WorldGenerator worldGenerator, NoiseProvider noiseProvider, int x, int z)
        {
            HeightmapThreadLockObject = new object();
            WorldGenerator = worldGenerator;
            Settings = settings;
            NoiseProvider = noiseProvider;

            Biomes = new List<BiomeType>();
            Neighborhood = new TerrainChunkNeighborhood();
            
            Position = new Vector2i(x, z);
        }

        #region Heightmap stuff

        public void GenerateHeightmap()
        {
            var thread = new Thread(GenerateHeightmapThread);
            thread.Start();
            //GenerateHeightmapThread();
        }

        private void GenerateHeightmapThread()
        {
            BiomeType[] impactors = new BiomeType[2]; //Out to BIOME_SAMPLES
            int offsetX = this.Position.X * (Settings.HeightmapResolution);
            int offsetZ = this.Position.Z * (Settings.HeightmapResolution);
            Heightmap = new float[Settings.HeightmapResolution, Settings.HeightmapResolution];
            lock (HeightmapThreadLockObject)
            {
                ChunkBlend = WorldGenerator.ChunkBlendProvider as Perlin;
                impactors = WorldGenerator.GetChunkBiomes(Position);
                this.Biomes.AddRange(impactors);
                var biomeA = GetBiomeNoiseProvider(impactors[0]);
                var biomeB = GetBiomeNoiseProvider(impactors[1]);

                var SelectModule = new Select(biomeA, biomeB, this.ChunkBlend);
                SelectModule.FallOff = 0.125f;
                SelectModule.SetBounds(0, 0.3);
                
                var heightmap = new float[Settings.HeightmapResolution, Settings.HeightmapResolution];

                for (var zRes = 0; zRes < Settings.HeightmapResolution; zRes++)
                {
                    for (var xRes = 0; xRes < Settings.HeightmapResolution; xRes++)
                    {
                        var xCoordinate = Position.X + (float)xRes / (Settings.HeightmapResolution - 1);
                        var zCoordinate = Position.Z + (float)zRes / (Settings.HeightmapResolution - 1);
                        
                        heightmap[zRes, xRes] = (float)SelectModule.GetValue(xCoordinate, 0, zCoordinate) / 2f +0.5f;
                    }
                }

                Heightmap = heightmap;
            }
            this.HeightmapReady = true;
        }


        /// <summary>
        /// Selects the correct NoiseProvider based on biome type
        /// </summary>
        /// <param name="biomeName"></param>
        /// <returns></returns>
        public LibNoise.ModuleBase GetBiomeNoiseProvider(BiomeType biomeName)
        {
            //Swap for INoiseProvider
            LibNoise.ModuleBase selectedModule;
            switch (biomeName)
            {
                case BiomeType.Ocean:
                    selectedModule = WorldGenerator.OceanProvider;
                    break;
                case BiomeType.Plains:
                    selectedModule = WorldGenerator.PlainsProvider;
                    break;
                case BiomeType.Hills:
                    selectedModule = WorldGenerator.HillsProvider;
                    break;
                case BiomeType.Mountains:
                    selectedModule = WorldGenerator.MountainsProvider;
                    break;
                
                default:
                    selectedModule = WorldGenerator.OceanProvider;
                    break;
            }
            return selectedModule;

        }

        public bool IsHeightmapReady()
        {
            return this.HeightmapReady;
        }

        public float GetTerrainHeight(Vector3 worldPosition)
        {
            return Terrain.SampleHeight(worldPosition);
        }



        #endregion

        #region Main terrain generation

        public void CreateTerrain()
        {
            //Debug.Log("Chunk["+Position.X+","+Position.Z+"].CreateTerrain()");
            Data = new TerrainData();
            Data.heightmapResolution = Settings.HeightmapResolution;
            Data.alphamapResolution = Settings.AlphamapResolution;
            Data.SetHeights(0, 0, Heightmap);
            ApplyTextures(Data);

            Data.size = new Vector3(Settings.Length, Settings.Height, Settings.Length);
            var newTerrainGameObject = Terrain.CreateTerrainGameObject(Data);
            newTerrainGameObject.transform.position = new Vector3(Position.X * Settings.Length, 0, Position.Z * Settings.Length);

            Terrain = newTerrainGameObject.GetComponent<Terrain>();
            Terrain.heightmapPixelError = 8;
            Terrain.materialType = UnityEngine.Terrain.MaterialType.Custom;
            Terrain.materialTemplate = Settings.TerrainMaterial;
            Terrain.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            Terrain.Flush();

            //Adding Trees to this chunk
            AddTrees(newTerrainGameObject);
            //Add feature if nessecary
            AddFeature(newTerrainGameObject);
        }

        private void ApplyTextures(TerrainData terrainData)
        {
            var flatSplat = new SplatPrototype();
            var steepSplat = new SplatPrototype();

            flatSplat.texture = Settings.FlatTexture;
            steepSplat.texture = Settings.SteepTexture;

            terrainData.splatPrototypes = new SplatPrototype[]
            {
                flatSplat,
                steepSplat
            };

            terrainData.RefreshPrototypes();

            var splatMap = new float[terrainData.alphamapResolution, terrainData.alphamapResolution, 2];

            for (var zRes = 0; zRes < terrainData.alphamapHeight; zRes++)
            {
                for (var xRes = 0; xRes < terrainData.alphamapWidth; xRes++)
                {
                    var normalizedX = (float)xRes / (terrainData.alphamapWidth - 1);
                    var normalizedZ = (float)zRes / (terrainData.alphamapHeight - 1);

                    var steepness = terrainData.GetSteepness(normalizedX, normalizedZ);
                    var steepnessNormalized = Mathf.Clamp(steepness / 1.5f, 0, 1f);

                    splatMap[zRes, xRes, 0] = 1f - steepnessNormalized;
                    splatMap[zRes, xRes, 1] = steepnessNormalized;
                }
            }

            terrainData.SetAlphamaps(0, 0, splatMap);
        }

        private void AddTrees(GameObject terrain)
        {
            GameObject ChunkTrees = new GameObject();
            ChunkTrees.name = "TreesAssets";
            ChunkTrees.transform.parent = terrain.transform;
            TreeAssets = ChunkTrees.AddComponent<ChunkTreeGenerator>();
            
            TreeAssets.ChunkHeightmap = Heightmap;
            TreeAssets.ChunkTerrain = Terrain;
            TreeAssets.ChunkPosition = Position;
            TreeAssets.ChunkSize = Settings.HeightmapResolution;
            TreeAssets.GenerateTrees();
        }

        private void AddFeature(GameObject terrain)
        {
            if (WorldGenerator.IsFeatureChunk(this.Position))
            {
                FeatureType type = WorldGenerator.GetChunkFeature(this.Position);
                this.ChunkFeature = new GameObject();
                ChunkFeature.name = "DungeonAssets";
                ChunkFeature.transform.parent = terrain.transform;
                var component = ChunkFeature.AddComponent<ChunkFeatureComponent>();
                component.ChunkCoords = this.Position;
                component.SetFeature(type, 100);
                component.Generate();
            }
        }

        #endregion

        #region Distinction

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as TerrainChunk;
            if (other == null)
                return false;

            return this.Position.Equals(other.Position);
        }

        #endregion

        #region Chunk removal

        public void Remove()
        {
            Heightmap = null;
            Settings = null;

            if (Neighborhood.XDown != null)
            {
                Neighborhood.XDown.RemoveFromNeighborhood(this);
                Neighborhood.XDown = null;
            }
            if (Neighborhood.XUp != null)
            {
                Neighborhood.XUp.RemoveFromNeighborhood(this);
                Neighborhood.XUp = null;
            }
            if (Neighborhood.ZDown != null)
            {
                Neighborhood.ZDown.RemoveFromNeighborhood(this);
                Neighborhood.ZDown = null;
            }
            if (Neighborhood.ZUp != null)
            {
                Neighborhood.ZUp.RemoveFromNeighborhood(this);
                Neighborhood.ZUp = null;
            }

            if(ChunkFeature != null)
            {
                var feature = ChunkFeature.GetComponent<ChunkFeatureComponent>();
                feature.Destroy();
            }
            TreeAssets.DestroyAllTrees();
            if (Terrain != null)
            {
                GameObject.Destroy(Terrain.gameObject);
            }
           
            

        }

        public void RemoveFromNeighborhood(TerrainChunk chunk)
        {
            if (Neighborhood.XDown == chunk)
                Neighborhood.XDown = null;
            if (Neighborhood.XUp == chunk)
                Neighborhood.XUp = null;
            if (Neighborhood.ZDown == chunk)
                Neighborhood.ZDown = null;
            if (Neighborhood.ZUp == chunk)
                Neighborhood.ZUp = null;
        }

        #endregion

        #region Neighborhood

        public void SetNeighbors(TerrainChunk chunk, TerrainNeighbor direction)
        {
            if (chunk != null)
            {
                switch (direction)
                {
                    case TerrainNeighbor.XUp:
                        Neighborhood.XUp = chunk;
                        break;

                    case TerrainNeighbor.XDown:
                        Neighborhood.XDown = chunk;
                        break;

                    case TerrainNeighbor.ZUp:
                        Neighborhood.ZUp = chunk;
                        break;

                    case TerrainNeighbor.ZDown:
                        Neighborhood.ZDown = chunk;
                        break;
                }
            }
        }

        public void UpdateNeighbors()
        {
            if (Terrain != null)
            {
                var xDown = Neighborhood.XDown == null ? null : Neighborhood.XDown.Terrain;
                var xUp = Neighborhood.XUp == null ? null : Neighborhood.XUp.Terrain;
                var zDown = Neighborhood.ZDown == null ? null : Neighborhood.ZDown.Terrain;
                var zUp = Neighborhood.ZUp == null ? null : Neighborhood.ZUp.Terrain;
                Terrain.SetNeighbors(xDown, zUp, xUp, zDown);
                Terrain.Flush();
            }
        }

        #endregion

        #region Edge Blending (Deprecated)
        //public float[] GetChunkEdge(EdgeDirection direction)
        //{
        //    int max = Settings.HeightmapResolution;
        //    float[] edge = new float[max];

        //    switch (direction)
        //    {
        //        case EdgeDirection.NORTH:
        //            for(int z = 0; z < max; z++)
        //            {
        //                edge[z] = this.Heightmap[0, z];
        //            }
        //            break;
        //        case EdgeDirection.SOUTH:
        //            for (int z = 0; z < max; z++)
        //            {
        //                edge[z] = this.Heightmap[max-1, z];
        //            }
        //            break;
        //        case EdgeDirection.EAST:
        //            for (int x = 0; x < max; x++)
        //            {
        //                edge[x] = this.Heightmap[x, max-1];
        //            }
        //            break;
        //        case EdgeDirection.WEST:
        //            for (int x = 0; x < max; x++)
        //            {
        //                edge[x] = this.Heightmap[x, 0];
        //            }
        //            break;
        //    }

        //    return edge;
        //}

        //public void SetChunkEdge(EdgeDirection direction, float[] newvalues)
        //{
        //    int max = Settings.HeightmapResolution;
        //    float[] edge = new float[max];

        //    switch (direction)
        //    {
        //        case EdgeDirection.NORTH:
        //            for (int z = 0; z < max; z++)
        //            {
        //                this.Heightmap[0, z] = newvalues[z];
        //            }
        //            break;
        //        case EdgeDirection.SOUTH:
        //            for (int z = 0; z < max; z++)
        //            {
        //                this.Heightmap[max-1, z] = newvalues[z];
        //            }
        //            break;
        //        case EdgeDirection.EAST:
        //            for (int x = 0; x < max; x++)
        //            {
        //                this.Heightmap[x, max-1] = newvalues[x];
        //            }
        //            break;
        //        case EdgeDirection.WEST:
        //            for (int x = 0; x < max; x++)
        //            {
        //                this.Heightmap[x, 0] = newvalues[x];
        //            }
        //            break;
        //    }

        //}

        #endregion
    }
}