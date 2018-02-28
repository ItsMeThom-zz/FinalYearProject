using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KNN
{
        public class KNNBiomePoint
        {
            public float HeightValue = 0.0f;
            public float MoistureValue = 0.0f;
            public BiomeType Type;

            public KNNBiomePoint(float hv, float moist, BiomeType type)
            {
                this.HeightValue = hv;
                this.MoistureValue = moist;
                this.Type = type;
            }


            public float DistanceTo(float x2, float y2)
            {
                return (x2 - HeightValue) +
                        (y2 - MoistureValue);

            }


        }
   
}
