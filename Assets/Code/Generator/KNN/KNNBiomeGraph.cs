using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KNN
{


    /*
            BiomeMap map = new BiomeMap();
            BiomePoint ocean = new BiomePoint(0.0f, 1.0f, "ocean");
            BiomePoint plains = new BiomePoint(0.3f, 0.4f, "plains");
            BiomePoint hills = new BiomePoint(0.5f, 0.4f, "hills");
            BiomePoint mountains = new BiomePoint(0.7f, 0.1f, "mountains");
            BiomePoint jungle = new BiomePoint(0.3f, 0.8f, "jungle");
            BiomePoint desert = new BiomePoint(0.45f, 0.1f, "desert");
            BiomePoint swamp = new BiomePoint(0.2f, 0.7f, "swamp");
            map.AddPoint(ocean);
            map.AddPoint(plains);
            map.AddPoint(hills);
            map.AddPoint(mountains);
            map.AddPoint(jungle);
            map.AddPoint(desert);
            map.AddPoint(swamp);


            //now test a sample point
            Dictionary<string, float> nearest = map.FindNearest(3, new float[2] { 0.5f, 0.3f });

     */

    /// <summary>
    /// Poor mans K-Nearest Neighbour Graph. Calculates impact probabilities for K nearest points to sample
    /// </summary>
    public class KNNBiomeGraph
    {

        List<KNNBiomePoint> Points;


        public KNNBiomeGraph()
        {
            this.Points = new List<KNNBiomePoint>();
        }

        public KNNBiomeGraph(List<KNNBiomePoint> input)
        {
            this.Points = new List<KNNBiomePoint>();
            this.Points.AddRange(input);
        }

        public void Train(KNNBiomePoint[] trainingset)
        {
           this.Points.AddRange(trainingset);
        }


        public void AddPoint(KNNBiomePoint bp)
        {
            this.Points.Add(bp);
        }

        public Dictionary<BiomeType, float> FindNearest(int n, Pair<float,float> point)
        {
            SortedDictionary<float, BiomeType> knn_map = new SortedDictionary<float, BiomeType>();
            foreach (KNNBiomePoint pt in Points)
            {
                float distance = pt.DistanceTo(point.A, point.B);
                BiomeType name = pt.Type;
                knn_map.Add(distance, name);
            }

            Dictionary<BiomeType, float> returnvals = new Dictionary<BiomeType, float>();
            float total = 0f; //normalising impact value
            for (int i = 0; i < n; i++)
            {
                total += knn_map.ElementAt(i).Key;
            }
            //reassign distances as "impact" normalisation
            for (int i = 0; i < n; i++)
            {
                var proportion = knn_map.ElementAt(i).Key / total;
                returnvals[knn_map.ElementAt(i).Value] = proportion;
            }

            return returnvals;
        }
    }
}
