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
using Assets.NoiseProviders;

namespace TerrainGenerator
{
    public class TerrainChunk
    {
        public Vector2i Position { get; private set; }

        public Terrain Terrain { get; set; }

        private TerrainData Data { get; set; }

        private TerrainChunkSettings Settings { get; set; }


        private TerrainChunkNeighborhood Neighborhood { get; set; }

        private WorldGenerator WorldGenerator { get; set; }


        public float[,] Heightmap { get; set; }

        public ChunkEdge EdgeType { get; set; }

        public List<BiomeType> Biomes { get; set; }

        private object HeightmapThreadLockObject { get; set; }


        public Blend BlendModule         { get; private set; }
        public bool HeightmapReady       { get; private set; }
        public NoiseBase ChunkBlend      { get; private set; }


        public ChunkFeatureComponent FeatureComponent { get; private set; }
        public ChunkRockGenerator    RocksComponent   { get; private set; }
        public ChunkTreeGenerator    TreesComponent   { get; private set; }

        public TerrainChunk(TerrainChunkSettings settings, WorldGenerator worldGenerator, ChunkEdge edgetype, int x, int z)
        {
            HeightmapThreadLockObject = new object();
            WorldGenerator = worldGenerator;
            Settings = settings;
            EdgeType = edgetype;

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
                ChunkBlend = WorldGenerator.ChunkBlendProvider;
                impactors = WorldGenerator.GetChunkBiomes(Position);
                this.Biomes.AddRange(impactors);
                //Debug.Log("Chunk " + this.Position.X + ", " + this.Position.Z + ":: " + impactors[0] + ", " + impactors[1]);
                var biomeA = GetBiomeNoiseProvider(impactors[0]);
                var biomeB = GetBiomeNoiseProvider(impactors[1]);
                var SelectModule = new Select(biomeA, biomeB, this.ChunkBlend.Provider);
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

                //if this chunk is a world edge, we need to  mask it to blend with the ocean
                if(this.EdgeType != ChunkEdge.NotEdge)
                {
                    Debug.Log("[" + Position.X + "," + Position.Z + "] is a " + this.EdgeType + " edge so will mask.");
                    //interpolation causes seams, we fix that here:
                    
                    float[,] mask = TerrainUtils.GetEdgeMask(this.EdgeType);
                    for(int x = 0; x < TerrainChunkSettings.ChunkSize; x++)
                    {
                        for (int z = 0; z < TerrainChunkSettings.ChunkSize; z++)
                        {
                          heightmap[x, z] *= mask[x, z];
                            
                        }
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
                    selectedModule = WorldGenerator.OceanNoise.Provider;
                    break;
                case BiomeType.Beach:
                    selectedModule = WorldGenerator.BeachNoise.Provider;
                    break;
                case BiomeType.Plains:
                    selectedModule = WorldGenerator.PlainsNoise.Provider;
                    break;
                case BiomeType.Hills:
                    selectedModule = WorldGenerator.HillsNoise.Provider;
                    break;
                case BiomeType.Mountains:
                    selectedModule = WorldGenerator.MountainsNoise.Provider;
                    break;
                default:
                    selectedModule = WorldGenerator.OceanNoise.Provider;
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
            if(Terrain == null) { Debug.Log("SampleHeight for terrain was null"); return 100.0f; }
            return Terrain.SampleHeight(worldPosition);
        }



        #endregion

        #region Main terrain generation


        //NOT Threaded (Can it be?)
        //TODO: Cleanup this and seperate functionality out.
        public void CreateTerrain()
        {
            GameController GameController = GameController.GetSharedInstance();
            int randVal = GameController.BaseSeed * Mathf.Abs(Position.X) * Mathf.Abs(Position.Z);
            Random.InitState(randVal);
            var x = Random.value + Random.value + Random.value;
            //Debug.Log("Chunk["+Position.X+","+Position.Z+"].CreateTerrain()");
            Data = new TerrainData();
            Data.heightmapResolution = Settings.HeightmapResolution;
            Data.alphamapResolution = Settings.AlphamapResolution;
            Data.SetHeights(0, 0, Heightmap);
            ApplyTextures(Data);
            //yield return 0;

            // GRASS ADDING
            Data.size = new Vector3(Settings.Length, Settings.Height, Settings.Length);
            AddDetailGrass(Data);
            //yield return 0;
            GameObject newTerrainGameObject = Terrain.CreateTerrainGameObject(Data);
            newTerrainGameObject.name = "Chunk: [" + Position.X + ", " + Position.Z + "]";

            //slightly shift edge chunks up, theyre off due to bilinear interpolation floating point errors
            float yPos = (this.EdgeType != ChunkEdge.NotEdge) ? 0.1f : 0.0f;
            newTerrainGameObject.transform.position = new Vector3(Position.X * Settings.Length, yPos, Position.Z * Settings.Length);
            newTerrainGameObject.tag = "Terrain";

            Terrain = newTerrainGameObject.GetComponent<Terrain>();
            Terrain.heightmapPixelError = 8;
            Terrain.materialType = UnityEngine.Terrain.MaterialType.Custom;
            Terrain.materialTemplate = Settings.TerrainMaterial;
            Terrain.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            Terrain.Flush();
            //yield return 0;


            //Adding Trees to this chunk
            if (EdgeType == ChunkEdge.NotEdge)
            {
                AddTrees();
                //yield return 0;
            }
           
            AddRocks();
            //yield return 0;
            //Add feature if nessecary
            AddFeature(newTerrainGameObject);
        }

        private void ApplyTextures(TerrainData terrainData)
        {
            
            var flatSplat = new SplatPrototype();
            var steepSplat = new SplatPrototype();
            var sandSplat = new SplatPrototype();

            flatSplat.texture = Settings.FlatTexture;
            steepSplat.texture = Settings.SteepTexture;
            sandSplat.texture = Settings.SandTexture;

            //TODO: Check biome to get textures to apply
            // ocean: sands and bluerock
            // mountains: grass and darkrock
            // hills: grass and rock
            // Eat some fucking food
            if(EdgeType != ChunkEdge.NotEdge)
            {
                terrainData.splatPrototypes = new SplatPrototype[]
                {
                    flatSplat, //sand is majority on edge tiles
                    steepSplat,
                    sandSplat
                };
            }
            else
            {
                terrainData.splatPrototypes = new SplatPrototype[]
                {
                    flatSplat,
                    steepSplat,
                    sandSplat
                };

            }

            terrainData.RefreshPrototypes();

            var splatMap = new float[terrainData.alphamapResolution, terrainData.alphamapResolution, 3];

            for (var zRes = 0; zRes < terrainData.alphamapHeight; zRes++)
            {
                for (var xRes = 0; xRes < terrainData.alphamapWidth; xRes++)
                {
                    float height = terrainData.GetHeight(xRes, zRes);
                    var normalizedX = (float)xRes / (terrainData.alphamapWidth - 1);
                    var normalizedZ = (float)zRes / (terrainData.alphamapHeight - 1);

                    var steepness = terrainData.GetSteepness(normalizedX, normalizedZ);
                    var steepnessNormalized = Mathf.Clamp(steepness / 1.5f, 0, 1f);

                    //apply rock and grass textures
                    splatMap[zRes, xRes, 0] = 1f - steepnessNormalized;
                    splatMap[zRes, xRes, 1] = steepnessNormalized;

                    if(height <= 0.28f) //add sand below edge
                    {
                        if(height >= 0.27f)
                        {
                            splatMap[zRes, xRes, 2] = Mathf.Clamp01(height / 2f);
                        }
                        else
                        {
                            splatMap[zRes, xRes, 2] = 1.0f;
                        }
                        
                    }
                }
            }
            terrainData.SetAlphamaps(0, 0, splatMap);
        }

        private void AddTrees()
        {
            // Add a child object to the chunk to store trees in.
            // And to hold the ChunkTreesGenerator component
            GameObject TreeHolderObj = new GameObject();
            TreeHolderObj.name = "TreesAssets";
            TreeHolderObj.transform.parent = this.Terrain.transform;
            // Add reference to the ChunkTreeGenerator component to this terrainchunk
            // so other components can access it easily
            TreesComponent = TreeHolderObj.AddComponent<ChunkTreeGenerator>();
            TreesComponent.ParentChunk = this;
            TreesComponent.Generate();
        }

        private void AddRocks()
        {
            //Add rocks to this chunk
            GameObject RockHolderObj = new GameObject();
            RockHolderObj.name = "RockAssets";
            RockHolderObj.transform.parent = Terrain.transform;
            RocksComponent = RockHolderObj.AddComponent<ChunkRockGenerator>();
            RocksComponent.ParentChunk = this;
            RocksComponent.Generate();
        }

        private void AddFeature(GameObject terrain)
        {
            //Adds dungeon feature to chunk
            if (WorldGenerator.IsFeatureChunk(this.Position))
            {
                FeatureType type = WorldGenerator.GetChunkFeature(this.Position);
                GameObject FeatureHolderObj = new GameObject();
                FeatureHolderObj.name = "FeatureAssets";
                FeatureHolderObj.transform.parent = Terrain.transform;
                FeatureComponent = FeatureHolderObj.AddComponent<ChunkFeatureComponent>();
                FeatureComponent.ParentChunk = this;
                FeatureComponent.SetFeature(type, UnityEngine.Random.Range(-10000, 10000));
                FeatureComponent.Generate();
            }
        }

        // THOM: This is causing most of the stutter when loading new chunks.
        // Find a way to offset this, either through threading, or coroutines
        // ALSO: Clean the bloody comments up!
        private void AddDetailGrass(TerrainData terrainData)
        {
            
            Texture2D grasstex = (Texture2D)Resources.Load("Textures/detailgrass");
            if(grasstex == null) { Debug.Log("NULL GRASS TEXTURE"); }
            //undocumented feature: Set the value of the detail prototype array directly
            DetailPrototype[] grassdetail = new DetailPrototype[1];
            terrainData.SetDetailResolution(258, 8);
            terrainData.wavingGrassTint = new Color(128.0f / 255, 128.0f / 255, 128.0f / 255); //remove the fucking tint!
            grassdetail[0] = new DetailPrototype();
            grassdetail[0].prototypeTexture = grasstex;
            grassdetail[0].renderMode = DetailRenderMode.GrassBillboard;
            grassdetail[0].minWidth = 1.0f;
            grassdetail[0].maxWidth = 2.0f;
            grassdetail[0].minHeight = 0.8f;
            grassdetail[0].maxHeight = 1.0f;
            grassdetail[0].noiseSpread = 0.1f;
            grassdetail[0].healthyColor = new Color(177.0f / 255, 218.0f / 255, 160.0f / 255);
            grassdetail[0].dryColor = new Color(148.0f / 255, 191.0f / 255, 142.0f / 255);
           
            terrainData.detailPrototypes = grassdetail;
            //Debug.Log("NAME:" + terrainData.detailPrototypes[1].prototypeTexture.name);
            float detailStrength = 0.3f; //A fair value to make the grass thick and luscious

            int myTextureLayer = 0; //Assumed to be the first  texture layer - this is the grass texture from which we wish to sprout grass
            int myDetailLayer = 0; //Assumed to be the first detail layer - this is the grass we wish to auto-populate

            // get the alhpa maps - i.e. all the ground texture layers
            float[,,] alphaMapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
            //get the detail map for the grass layer we're after
            int[,] map = new int[258, 258];
            //now copy-paste the alpha map onto the detail map, pixel by pixel
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                for (int y = 0; y < terrainData.alphamapHeight; y++)
                {
                    //Check the Detail Resolution and the Control Texture Resolution in the terrain settings.
                    //By default the detail resolution is twice the alpha resolution! So every detail co-ordinate is going to have to affect a 2x2 square!
                    //Would be nice if I could so some anti aliasing but this will have to do for now
                    int x1 = x * 2;
                    int x2 = (x * 2) + 1;
                    int y1 = y * 2;
                    int y2 = (y * 2) + 1;
                    if(terrainData.GetHeight(y,x) > 11.0f) //a little up from sealevel.
                    {
                        map[x1, y1] = (int)(alphaMapData[x, y, myTextureLayer] + detailStrength);
                        map[x1, y2] = (int)(alphaMapData[x, y, myTextureLayer] + detailStrength);
                        map[x2, y1] = (int)(alphaMapData[x, y, myTextureLayer] + detailStrength);
                        map[x2, y2] = (int)(alphaMapData[x, y, myTextureLayer] + detailStrength);
                    }
                    //if the resolution was the same we could just do the following instead: map [x, y] = (int)alphaMapData [x, y, myTextureLayer] * 10;                 
                }
            }
            terrainData.SetDetailLayer(0, 0, myDetailLayer, map);
        
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

            if(FeatureComponent != null)
            {
                FeatureComponent.DestroyAllAssets();
            }
            if(TreesComponent != null)
            {
                TreesComponent.DestroyAllAssets();
            }
            if(RocksComponent != null)
            {
                RocksComponent.DestroyAllAssets();
            }
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

    }
}