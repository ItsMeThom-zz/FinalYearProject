using KNN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KNN
{
    /// <summary>
    /// stand in replacement for KNN biomegraph as there is unresolvable issues with threading
    /// </summary>
    public class KLineGraph
    {

        public List<GraphPoint> Points { get; set; }


        public KLineGraph()
        {
            GraphPoint ocean     = new GraphPoint(BiomeType.Ocean, 0.0f);
            //GraphPoint beach     = new GraphPoint(BiomeType.Beach, 0.2f);
            GraphPoint plains    = new GraphPoint(BiomeType.Plains, 0.3f);
            GraphPoint hills     = new GraphPoint(BiomeType.Hills, 0.6f);
            GraphPoint mountains = new GraphPoint(BiomeType.Mountains, 0.95f);
            this.Points = new List<GraphPoint>();
            Points.Add(ocean);
            //Points.Add(beach);
            Points.Add(plains);
            Points.Add(hills);
            Points.Add(mountains);

        }


        public List<BiomeType> FindNearest(float value)
        {
            List<BiomeType> impactors = new List<BiomeType>();
            BiomeType nearLeft = BiomeType.Ocean;
            BiomeType nearRight = BiomeType.Mountains;
            float leftDist = float.MaxValue;
            float rightDist = float.MaxValue;

            foreach(var p in Points)
            {
                if(p.Point < value) //closest value to left of point
                {
                    var dist = value - p.Point;
                    if(dist <= leftDist)
                    {
                        leftDist = dist;
                        nearLeft = p.Biome;
                    }
                }
                else if(p.Point > value) //closes to right
                {
                    var dist = p.Point - value;
                    if (dist <= rightDist)
                    {
                        
                        rightDist = dist;
                        nearRight = p.Biome;
                    }
                }
                else
                {
                    impactors.Add(p.Biome);
                    Debug.Log("SINGLE BIOME FOUND");
                    return impactors;
                }
            }
            Debug.Log("Value was: " + value + ". Chose (" + nearLeft + ", " + nearRight + ")");
            impactors.Add(nearLeft);
            impactors.Add(nearRight);
            return impactors;
        }

    }




    public class GraphPoint
    {
        public BiomeType Biome { get; set; }
        public float Point { get; set; }

        public GraphPoint(BiomeType biome, float val)
        {
            Biome = biome;
            Point = val;
        }
    }
}
