using LibNoise.Generator;
using LibNoise.Operator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;

namespace Assets.NoiseProviders
{
    public class PlainsNoise : NoiseBase, INoiseProvider
    {
        static PlainsNoise _instance;


        public static PlainsNoise Get()
        {
            if (_instance == null)
            {
                int seed = GameController.GetSharedInstance().BaseSeed;
                _instance = new PlainsNoise(seed);
            }
            return _instance;
        }

        PlainsNoise(int seed) //specific seed provided
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
            //Generate perlin noise
            //Scale to be lower, bias to plains range
            Perlin baseTerrain = new Perlin()
            {
                OctaveCount = 8,
                Frequency = 1.1,
                Persistence = 0.124,
                Seed = seed
            };
            Perlin secondaryTerrain = new Perlin()
            {
                OctaveCount = 4,
                Frequency = 1.5,
                Persistence = 0.123,
                Seed = -seed

            };
            Add adder = new Add(baseTerrain, secondaryTerrain);

            ScaleBias provider = new ScaleBias(adder)
            {
                Scale = 0.125,
                //Bias = 0.03f
            };
            this._baseProvider = provider;
        }
    }
}
