using KNN;
using LibNoise.Generator;
using LibNoise.Operator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;
using UnityEngine;
using Utils;

namespace WorldGen
{
    public class WorldGenerator
    {
        public int BaseSeed = 12345678;
        public static int MAP_SIZE = 16;

        private Perlin WorldMapProvider;


        #region Chunk NoiseProviders
        public LibNoise.ModuleBase ChunkBlendProvider { get; set; }
        public LibNoise.ModuleBase OceanProvider { get; set; }
        public LibNoise.ModuleBase BeachProvider { get; set; }
        public LibNoise.ModuleBase PlainsProvider { get; set; }
        public LibNoise.ModuleBase MountainsProvider { get; set; }
        public LibNoise.ModuleBase HillsProvider { get; set; }


        public float ElevationMaxValue { get; private set; }
        public float ElevationMinValue { get; private set; }
        public float MoisutreMaxValue { get; private set; }
        public float MoisutreMinValue { get; private set; }

        //THOM: Refactor to wrap Noise configurations in cleaner handler class <INoiseProvider>
        /*
        public INoiseProvider ChunkBlendProvider { get; set; }
        public INoiseProvider MountProvider      { get; set; }
        public INoiseProvider OceanProvider      { get; set; }
        public INoiseProvider PlainsProvider     { get; set; }
        public INoiseProvider HillsProvider      { get; set; }


        */
        #endregion


        #region WorldMap Data
        public float[,] ElevationData;
        public float[,] MoistureData;
        #endregion

        #region Biome Handling
        public KLineGraph BiomeGraph;
        #endregion

        //feature handling
        Dictionary<Vector2i, FeatureType> ChunkFeatures;

        #region Flags
        public bool IsGenerated = false;
        #endregion



        public WorldGenerator()
        {
            this.ElevationMaxValue = 0.0f;
            this.ElevationMinValue = float.MaxValue;
            InitNoiseProviders();
            PopulateBiomeGraph();

        }

        #region Noise Provider Initalisation Routines
        /// <summary>
        /// Initalise all providers with seeds
        /// 
        /// TODO: Add seeds where appropriate
        /// </summary>
        public void InitNoiseProviders()
        {
            this.WorldMapProvider = InitWorldProvider();
            this.ChunkBlendProvider = InitChunkBlendProvider();
            this.OceanProvider = InitOceanProvider();
            this.BeachProvider = InitBeachProvider();
            this.HillsProvider = InitHillsProvider();
            this.PlainsProvider = InitPlainsProvider();
            this.MountainsProvider = InitMountainsProvider();
        }

        public Perlin InitWorldProvider()
        {
            Perlin provider = new Perlin()
            {
                Seed = BaseSeed,
                OctaveCount = 6,
                Frequency = 0.7f,
                Persistence = 0.3f
            };
            return provider;
        }

        public LibNoise.ModuleBase InitChunkBlendProvider()
        {
            Perlin provider = new Perlin()
            {
                Seed = 67657348,
                OctaveCount = 4,
                Frequency = 1.0f,
                Persistence = 0.225f,
                Lacunarity = 1.87
            };
            return provider;
        }

        public LibNoise.ModuleBase InitOceanProvider()
        {
            //Generate Smooth Perlin Noise
            //ScaleBias to be near 0 (OCEAN HEIGHT)
            Perlin baseTerrain = new Perlin()
            {
                OctaveCount = 8,
                Frequency = 0.7,
                Persistence = 0.225
                
            };
            ScaleBias provider = new ScaleBias(baseTerrain)
            {
                Scale = 0.125f,
                Bias = -1.0f
            };
            return provider;
        }

        public LibNoise.ModuleBase InitBeachProvider()
        {
            //Generate Smooth Perlin Noise
            //ScaleBias to be almost 0 to blend with (OCEAN HEIGHT)
            Perlin baseTerrain = new Perlin()
            {
                OctaveCount = 9,
                Frequency = 0.9,
                Persistence = 0.225

            };
            ScaleBias provider = new ScaleBias(baseTerrain)
            {
                Scale = 0.225f,
                Bias = -0.3f
            };
            return provider;
        }

        public LibNoise.ModuleBase InitPlainsProvider()
        {
            //Generate perlin noise
            //Scale to be lower, bias to plains range
            Perlin baseTerrain = new Perlin()
            {
                OctaveCount = 8,
                Frequency = 1.1,
                Persistence = 0.124,
                Seed = BaseSeed
            };
            Perlin secondaryTerrain = new Perlin()
            {
                OctaveCount = 4,
                Frequency = 1.5,
                Persistence = 0.123,
                Seed = -BaseSeed
                
            };
            Add adder = new Add(baseTerrain, secondaryTerrain);
            
            ScaleBias provider = new ScaleBias(adder)
            {
                Scale = 0.125,
                //Bias = 0.03f
            };
            return provider;
        }

        public LibNoise.ModuleBase InitHillsProvider()
        {
            //generate billow noise
            //turbulate
            //scale to hills range
            Billow baseTerrain = new Billow()
            {
                OctaveCount = 12,
                Lacunarity = 1.7,
                Frequency = 0.8,
                Persistence = 0.325
            };

            Perlin secondary = new Perlin()
            {
                OctaveCount = 8,
                Frequency = 1.1,
                Persistence = 0.235,
                Seed = -BaseSeed
            };

            Add adder = new Add(baseTerrain, secondary);
            Turbulence turbulator = new Turbulence(adder)
            {
                Power = 0.125f,
                //Roughness = 1,
                Frequency = 4f
            };
            ScaleBias provider = new ScaleBias(turbulator)
            {
                Scale = 0.6f,
            };
            return provider;

        }

        public LibNoise.ModuleBase InitMountainsProvider()
        {
            //Generate Ridged Multifractal noise
            RidgedMultifractal provider = new RidgedMultifractal()
            {
                OctaveCount = 1,
                Frequency = 0.3f,
                Seed = BaseSeed
            };
            ScaleBias prov= new ScaleBias(provider)
            {
                Scale = 0.5f,
                //Bias = 0
            };
            return prov;
        }

        #endregion

        #region World Map Initalisation
        public void InitWorldmapData()
        {
            this.ElevationData = new float[MAP_SIZE, MAP_SIZE];
            this.MoistureData = new float[MAP_SIZE, MAP_SIZE];

            ChunkFeatures = new Dictionary<Vector2i, FeatureType>();
        }

        public void GenerateWorldMap()
        {
            //float[,] islandmask = TerrainUtils.GetIslandMask(MAP_SIZE);

            this.InitWorldmapData();
            //worldperlin generates the overall worldmap for sampling heights from for biomes
            float SCALE = 20.0f; //scaling the perlin to be larger (low res)
            float moisutreOffset = UnityEngine.Random.Range(-10000, 10000);
            for (int x = 0; x < MAP_SIZE; x++)
            {
                for (int z = 0; z < MAP_SIZE; z++)
                {
                    float biomeX = x / SCALE;
                    float biomeZ = z / SCALE;
                    float moistX = biomeX + moisutreOffset;
                    float moistZ = biomeZ + moisutreOffset;

                    ElevationData[x, z] = ((float)(WorldMapProvider.GetValue(biomeX, 0, biomeZ)) + 2f / 0.5f);// * islandmask[x,z];
                    //track the max and min values for normalisation later
                    if(ElevationData[x,z] > this.ElevationMaxValue)
                    {
                        ElevationMaxValue = ElevationData[x, z];
                    }
                    if (ElevationData[x,z] < this.ElevationMinValue)
                    {
                        ElevationMinValue = ElevationData[x, z];
                    }
                    MoistureData[x, z] = (float)(WorldMapProvider.GetValue(moistX, 0, moistZ)) + 2f / 0.5f;
                    if (MoistureData[x, z] > this.MoisutreMaxValue)
                    {
                        MoisutreMaxValue = MoistureData[x, z];
                    }
                    if (MoistureData[x, z] < this.MoisutreMinValue)
                    {
                        MoisutreMinValue = MoistureData[x, z];
                    }

                }
            }

            //set the random chunks to put features in <STATIC FOR DEV>
            ChunkFeatures.Add(new Vector2i(0, 0),  FeatureType.DungeonEntrance);
            ChunkFeatures.Add(new Vector2i(-1, -1), FeatureType.DungeonEntrance);
            ChunkFeatures.Add(new Vector2i(5, 5),  FeatureType.DungeonEntrance);
            this.IsGenerated = true;
            Debug.Log("World Map is Generated");
        }

        public void PopulateBiomeGraph()
        {
            //static values for now. move later
            this.BiomeGraph = new KLineGraph();

        }
        #endregion


        #region Chunk Handling
        /// <summary>
        /// Samples KNN biomegraph to return biomes impacting this chunk in the world
        /// returns a preset ocean biometype if the chunk position is outside the map bounds
        /// </summary>
        /// <param name="position">(X,Z) coordinates of chunk</param>
        /// <returns>BiomeType[] biomes that impact this point</returns>
        public BiomeType[] GetChunkBiomes(Vector2i position)
        {
            
            BiomeType[] impactors = new BiomeType[2];
            if (IsValidMapPosition(position))
            {
                Pair<float, float> worldmapSamples = GetChunkSampleValues(position);
                impactors = new BiomeType[] { BiomeType.Hills, BiomeType.Mountains }; //BiomeGraph.FindNearest(worldmapSamples.A).ToArray();
            }
            else //this is a pure ocean chunk, its outside the generated map bounds
            {
                impactors = new BiomeType[2] { BiomeType.Ocean, BiomeType.Ocean };
            }
            return impactors;
        }

        /// <summary>
        /// Returns an elevation/moisture pair for the chunk sample
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Pair<float, float> GetChunkSampleValues(Vector2i position)
        {
            //select the correct pixel. map_size / 2 is chunk 0,0 for reference
            int x0 = 0;
            int z0 = 0;
            x0 = (MAP_SIZE / 2) + position.X;
            z0 = (MAP_SIZE / 2) + position.Z;

            float elevationSample = 0.0f;
            float moisutreSample = 0.0f;
            //handling the player moving outside the mapzone, i.e. into the infinite ocean
            elevationSample =  Utils.MathUtils.Normalise(this.ElevationData[x0, z0], this.ElevationMinValue, this.ElevationMaxValue);
            moisutreSample  =  Utils.MathUtils.Normalise( this.MoistureData[x0, z0], this.MoisutreMinValue, this.MoisutreMaxValue);
            Pair<float, float> samples = new Pair<float, float>(elevationSample, moisutreSample);
            return samples;
        }

        //is the chunk position within the worldmaps bounds?
        public bool IsValidMapPosition(Vector2i position)
        {
            int x0 = (MAP_SIZE / 2) + position.X;
            int z0 = (MAP_SIZE / 2) + position.Z;
            if (x0 < 0 || x0 > ElevationData.GetLength(0) || z0 < 0 || z0 > ElevationData.GetLength(1))
            {
                Debug.Log("Chunk is outside the map: " + position.X + "," + position.Z);
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsFeatureChunk(Vector2i position)
        {
            return this.ChunkFeatures.ContainsKey(position);
        }

        public FeatureType GetChunkFeature(Vector2i position)
        {
            if (this.ChunkFeatures.ContainsKey(position))
            {
                return ChunkFeatures[position];
            }
            else
            {
                return FeatureType.None;
            }
        }

        #endregion
    }
}

