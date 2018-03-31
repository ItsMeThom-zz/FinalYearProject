using LibNoise.Generator;
using LibNoise.Operator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;

namespace Assets.NoiseProviders
{
    public class BeachNoise : NoiseBase, INoiseProvider
    {
        static BeachNoise _instance;

        public static BeachNoise Get()
        {
            if (_instance == null)
            {
                int seed = GameController.GetSharedInstance().BaseSeed;
                _instance = new BeachNoise(seed);
            }
            return _instance;
        }

        BeachNoise(int seed) //specific seed provided
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
            //ScaleBias to be almost 0 to blend with (OCEAN HEIGHT)
            Perlin baseTerrain = new Perlin()
            {
                Seed = seed,
                OctaveCount = 9,
                Frequency = 0.9,
                Persistence = 0.225

            };
            ScaleBias provider = new ScaleBias(baseTerrain)
            {
                Scale = 0.225f,
                Bias = -0.3f
            };
            this._baseProvider = provider;
        }
    }
}
