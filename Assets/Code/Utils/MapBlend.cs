using System;
/// <summary>
/// Blending 2 noisemaps based on the values from a controlmap
/// this mimics the operation of LibNoise.Operators.Blend() but uses an existing controlmap
/// </summary>
public static class MapBlend
    {
        public static int SIZE = 129;

        public static float[,] ControlBlend(float[,]a, float[,] b, float[,] control)
        {
            float[,] result = new float[SIZE, SIZE];

            for(int x = 0; x < SIZE; x++)
            {
                for(int z = 0; z < SIZE; SIZE++){
                    result[x, z] = (float)LibNoise.Utils.InterpolateLinear(Convert.ToDouble(a[x,z]), Convert.ToDouble(b[x, z]), Convert.ToDouble(control[x, z]));

                }
            }

            return result;
        }


    }

