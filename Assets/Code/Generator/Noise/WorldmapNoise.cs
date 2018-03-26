using LibNoise.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;

namespace Assets.NoiseProviders
{
    public class WorldmapNoise : NoiseBase, INoiseProvider
    {
        static WorldmapNoise _instance;

        public static WorldmapNoise Get()
        {
            if (_instance == null)
            {
                int seed = UnityEngine.Random.Range(0, int.MaxValue);
                _instance = new WorldmapNoise(seed);
            }
            return _instance;
        }

        WorldmapNoise(int seed) //specific seed provided
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
                OctaveCount = 6,
                Frequency = 0.7f,
                Persistence = 0.3f
            };
            this._baseProvider = provider;
        }
    }
}
