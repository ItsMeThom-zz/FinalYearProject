using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code.Generator.Noise
{
    public abstract class NoiseBase
    {
        public static int BaseSeed;
        private LibNoise.ModuleBase provider;

        protected abstract void ConfigureProviders(int seed);

    }
}
