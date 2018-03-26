using LibNoise.Generator;
using LibNoise.Operator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;

namespace Assets.NoiseProviders
{
    public class OceanNoise : NoiseBase, INoiseProvider
    {
        static OceanNoise _instance;

        public static OceanNoise Get()
        {
            if (_instance == null)
            {
                int seed = UnityEngine.Random.Range(0, int.MaxValue);
                _instance = new OceanNoise(seed);
            }
            return _instance;
        }

        OceanNoise(int seed) //specific seed provided
        {
            Seed = seed;
            ConfigureProviders(Seed);
        }

        public float GetValue(float x, float z)
        {
            return (float)_baseProvider.GetValue(x, 0, z);
        }

        protected override void ConfigureProviders(int seed)
        {
            //Generate Smooth Perlin Noise
            //ScaleBias to be near 0 (OCEAN HEIGHT)
            Perlin baseTerrain = new Perlin()
            {
                Seed = seed,
                OctaveCount = 8,
                Frequency = 0.7,
                Persistence = 0.225

            };
            ScaleBias provider = new ScaleBias(baseTerrain)
            {
                Scale = 0.125f,
                Bias = -1.0f
            };
            this._baseProvider = provider;
        }
    }
}
