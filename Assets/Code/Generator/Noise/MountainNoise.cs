using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;
using LibNoise.Generator;
using LibNoise.Operator;

namespace Assets.NoiseProviders
{
    public class MountainNoise : NoiseBase, INoiseProvider
    {

        static MountainNoise _instance;


        public static MountainNoise Get()
        {
            if(_instance == null)
            {
                int seed = GameController.GetSharedInstance().BaseSeed;
                _instance = new MountainNoise(seed);
            }
            return _instance;
        }

        public static MountainNoise Get(int seed)
        {
            if (_instance == null)
            {
                _instance = new MountainNoise(seed);
            }
            return _instance;
        }

        MountainNoise(int seed) //specific seed provided
        {
            Seed = seed;
            ConfigureProviders(Seed);
        }

        protected override void ConfigureProviders(int seed)
        {
            //Generate Ridged Multifractal noise
            RidgedMultifractal provider = new RidgedMultifractal()
            {
                OctaveCount = 1,
                Frequency = 0.3f,
                Seed = seed
            };
            ScaleBias prov = new ScaleBias(provider)
            {
                Scale = 0.5f,
                //Bias = 0
            };
            this._baseProvider = prov;
        }

        //return a value from the specified provider
        public float GetValue(float x, float z)
        {
            return (float)_baseProvider.GetValue(x, 0, z);
        }
    }
}
