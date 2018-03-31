using LibNoise.Generator;
using LibNoise.Operator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;

namespace Assets.NoiseProviders
{
    public class HillsNoise : NoiseBase, INoiseProvider
    {

        static HillsNoise _instance;


        public static HillsNoise Get()
        {
            if (_instance == null)
            {
                int seed = GameController.GetSharedInstance().BaseSeed;
                _instance = new HillsNoise(seed);
            }
            return _instance;
        }

        HillsNoise(int seed) //specific seed provided
        {
            Seed = seed;
            ConfigureProviders(Seed);
        }

        protected override void ConfigureProviders(int seed)
        {
            //generate billow noise
            //turbulate
            //scale to hills range
            Billow baseTerrain = new Billow()
            {
                Seed = seed,
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
                Seed = -seed
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
            this._baseProvider = provider;
        }

        public float GetValue(float x, float z)
        {
            return (float)_baseProvider.GetValue(x, 0, z);
        }

        
    }
}
