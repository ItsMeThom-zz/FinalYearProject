using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.NoiseProviders
{
    /// <summary>
    /// Abstract base class that all NoiseProviders inherit from
    /// 
    /// Seed:                   Base Seed for RNG
    /// _baseProvider:          LibNoise.Generator module that is fed input from
    ///                         module chain
    /// 
    /// ConfigureProviders() : overridden method to configure unique
    ///                         module chain for each noise type required
    /// </summary>
    public abstract class NoiseBase
    {
        public static int Seed;
        protected LibNoise.ModuleBase _baseProvider;

        public LibNoise.ModuleBase Provider {
            get
            {
                return _baseProvider;
            }
        }
        //moved to interface
        //public abstract float GetValue(float x, float z);
        protected abstract void ConfigureProviders(int seed);

    }
}
