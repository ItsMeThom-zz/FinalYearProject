using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibNoise;
using Utils;
using UnityEngine;

namespace KNN
{


    public static class MapUtils
    {
        /// <summary>
        /// Performs Bilinear Interpolation of a 2d array of floats
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="newXSize"></param>
        /// <param name="newYSize"></param>
        /// <returns></returns>
        public static float[,] BilinearInterpolationScale(float[,] arr, int newXSize, int newYSize)
        {

            float[,] newArr = new float[newXSize, newYSize];

            int[] temp = new int[newXSize * newYSize];
            int x, y;
            float A, B, C, D;
            float x_ratio = ((float)(arr.GetLength(0) - 1)) / newXSize;
            float y_ratio = ((float)(arr.GetLength(1) - 1)) / newYSize;

            float w, h;

            for (int i = 0; i < newYSize; i++)
            {
                for (int j = 0; j < newXSize; j++)
                {
                    x = (int)(x_ratio * j);
                    y = (int)(y_ratio * i);
                    w = (x_ratio * j) - x;
                    h = (y_ratio * i) - y;

                    A = arr[x, y];
                    B = arr[x + 1, y];
                    C = arr[x, y + 1];
                    D = arr[x + 1, y + 1];

                    var r = (A * (1 - w) * (1 - h) + B * (w) * (1 - h) + C * (h) * (1 - w) + D * (w * h));
                    // for the purposes of this game, clamp values to 0
                    //r = (r < 0) ? 0 : r;
                    var newFloat = r;

                    newArr[j, i] = newFloat;
                }
            }

            return newArr;
        }

        /// <summary>
        /// Creates an radial island mask that can be used to subtract from noisemaps to mask them as island shaped.
        /// </summary>
        /// <param name="size">Map Width</param>
        /// <param name="heightScale">Dampening at the centerpoint, 1 by default (no dampening)</param>
        /// <returns></returns>
        public static float[,] CreateInverseRadialGradient(int size, float heightScale = 0.8f)
        {
            float radius = size / 2;

            float[,] heightMap = new float[size, size];

            for (int iy = 0; iy < size; iy++)
            {
                //int stride = iy * size;
                for (int ix = 0; ix < size; ix++)
                {
                    float centerToX = ix - radius;
                    float centerToY = iy - radius;

                    float distanceToCenter = (float)Math.Sqrt(centerToX * centerToX + centerToY * centerToY);
                    heightMap[iy, ix] = distanceToCenter / radius * heightScale;
                }
            }

            return heightMap;
        }

        public static float[] AverageEdges(float[] edgeA, float[] edgeB)
        {
            float[] newvals = new float[edgeA.GetLength(0)];
            for(int i = 0; i < edgeA.GetLength(0); i++)
            {
                newvals[i] = (edgeA[i] + edgeB[i]) / 2f;
            }
            return newvals;
        }
       
    }
    }
