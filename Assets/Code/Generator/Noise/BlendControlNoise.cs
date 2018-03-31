using LibNoise.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;

namespace Assets.NoiseProviders
{
    public class BlendControlNoise : NoiseBase, INoiseProvider
    {
        static BlendControlNoise _instance;

        public static BlendControlNoise Get()
        {
            if (_instance == null)
            {
                int seed = GameController.GetSharedInstance().BaseSeed;
                _instance = new BlendControlNoise(seed);
            }
            return _instance;
        }

        BlendControlNoise(int seed) //specific seed provided
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
            Perlin provider = new Perlin()
            {
                Seed = seed,
                OctaveCount = 4,
                Frequency = 1.0f,
                Persistence = 0.225f,
                Lacunarity = 1.87
            };

            this._baseProvider = provider;
        }
    }
}
