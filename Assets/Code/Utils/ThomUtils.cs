using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;
using UnityEngine;
using WorldGen;

namespace Utils
{
    public enum EdgeDirection {NORTH, SOUTH, EAST, WEST }

    public static class MeshUtils
    {
        public static Bounds GetCombinedChildColliderBounds(Transform transform)
        {
            Bounds bounds = new Bounds(transform.position, Vector3.one);
            Renderer[] meshes = transform.GetComponentsInChildren<Renderer>();
            if (meshes.GetLength(0) == 0) { Debug.Log("FUP"); }
            foreach (Renderer mesh in meshes)
            {
                bounds.Encapsulate(mesh.bounds);
            }
            return bounds;
        }
    }

    public static class MathUtils
    {
        /// <summary>
        /// Blends the edges of adjoining chunks using mirroring and weighted averages derived from Mathf.SmoothStep()
        /// </summary>
        /// <param name="chunkA">This chunk</param>
        /// <param name="chunkB">Nneighbour chunk</param>
        /// <param name="direction">Direction of Neighbour Chunk</param>
        /// <param name="dual">Are we blending both (neighbour ungenerated) or just this (neighbour generated)</param>
        public static void AverageBothChunks(ref float[,] chunkA, ref float[,] chunkB, EdgeDirection direction, bool dual)
        {
            //Debug.Log("ASize: " + chunkA.GetLength(0) +", " + chunkA.GetLength(1));
            //Debug.Log("BSize: " + chunkB.GetLength(0) + ", " + chunkB.GetLength(1));
            //int CHUNK_SIZE = TerrainChunkSettings.ChunkSize;
            //int distance = 32;
            //float average = 0.0f;
            //float oldA;
            //float oldB;

            //switch (direction)
            //{
            //    case EdgeDirection.NORTH: //blend A northedge with B southedge
            //        //aX is 0->chunksize
            //        //aZ is 0->20
            //        //bX is aX
            //        //bZ is chunksize-aZ (mirrors aZ in z-axis)
            //        for (int aX = 0; aX < CHUNK_SIZE; aX++)
            //        {
            //            for (int aZ = 0; aZ < distance; aZ++)
            //            {
            //                var weight = Mathf.SmoothStep(0, 1, (aZ / distance));
            //                Debug.Log("::" + aX + "," + aZ);
            //                Debug.Log(chunkA[0, 0]);
            //                try
            //                {
            //                    oldA = chunkA[aX, aZ];
            //                }catch(IndexOutOfRangeException ex)
            //                {
            //                    Debug.Log("Bad Index A: " + aX + ", " + aZ);

            //                }
            //                oldA = chunkA[aX, aZ];
            //                oldB = chunkB[aX, (CHUNK_SIZE -1) - aZ]; //mirror point across z axis
            //                average = (oldA + oldB) / 2;
            //                //new heights
            //                chunkA[aX, aZ] = oldA + (average - oldA) * weight;
            //                if (dual)
            //                {
            //                    chunkB[aX, (CHUNK_SIZE - 1) - aZ] = oldB + (average - oldB) * weight;
            //                }

            //            }
            //        }
            //        break;
            //    case EdgeDirection.SOUTH: //blendA southedge with B northedge
            //        for (int aX = 0; aX < CHUNK_SIZE; aX++)
            //        {
            //            for (int aZ = CHUNK_SIZE - 1; aZ > CHUNK_SIZE - distance; aZ--)
            //            {
            //                var weight = Mathf.SmoothStep(0, 1, ((CHUNK_SIZE - aZ) / distance));
            //                try
            //                {
            //                    oldA = chunkA[aX, aZ];
            //                }catch(Exception ex)
            //                {
            //                    Debug.Log("ERROR ACCESSING CHUNK A[" + aX + "," + aZ + "]");
            //                }
            //                oldA = 0.0f;
            //                oldB = chunkB[aX, 0 + ((CHUNK_SIZE - 1) - aZ)]; //mirror point across z axis
            //                average = (oldA + oldB) / 2;
            //                //new heights
            //                chunkA[aX, aZ] = oldA + (average - oldA) * weight;
            //                if (dual)
            //                {
            //                    chunkB[aX, CHUNK_SIZE -1 - aZ] = oldB + (average - oldB) * weight;
            //                }

            //            }
            //        }
            //        break;
            //    case EdgeDirection.EAST: //blendA eastedge with B westedge
            //        for (int aZ = 0; aZ < CHUNK_SIZE; aZ++)
            //        {
            //            for (int aX = CHUNK_SIZE; aX > CHUNK_SIZE - distance; aX--)
            //            {

            //                var weight = Mathf.SmoothStep(0, 1, ((CHUNK_SIZE - aX) / distance));
            //                oldA = chunkA[aX, aZ];
            //                oldB = chunkB[(CHUNK_SIZE - 1 - aX), aZ]; //mirror point across x axis
            //                average = (oldA + oldB) / 2;
            //                chunkA[aX, aZ] = oldA + (average - oldA) * weight;
            //                if (dual)
            //                {
            //                    chunkB[(CHUNK_SIZE - aX), aZ] = oldB + (average - oldB) * weight;
            //                }

            //            }
            //        }
            //        break;
            //    case EdgeDirection.WEST: //blendA westedge with B eastedge
            //        for (int aZ = 0; aZ < CHUNK_SIZE; aZ++)
            //        {
            //            for (int aX = 0; aX < distance; aX++)
            //            {
            //                var weight = Mathf.SmoothStep(0, 1, (aX / distance));
            //                oldA = chunkA[aX, aZ];
            //                oldB = chunkB[CHUNK_SIZE - aX, aZ];
            //                average = (oldA + oldB) / 2;
            //                chunkA[aX, aZ] = oldA + (average - oldA) * weight;
            //                if (dual)
            //                {
            //                    chunkB[(-aX + CHUNK_SIZE), aZ] = oldB + (average - oldB) * weight;
            //                }
            //            }
            //        }
            //        break;
            //}
        }

        //Normalise a value between 0 (min) and 1 (max)
        public static float Normalise(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }
    }

    public static class TerrainUtils
    {

        /// <summary>
        /// Raycasts from an object to find terrain (Up/Down)
        /// Returns the distance as a negative in the case of down (for subtracting from object height)
        /// or positive (for adding)
        /// in the case of not finding any terrain, returns 0.0f
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float RaycastToTerrain(Vector3 position)
        {
            float distance = 0.0f;
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(position, Vector3.down, out hit, 1000))
            {
                Debug.DrawLine(position, new Vector3(position.x, position.y - 500, position.z), Color.green);
                if (hit.collider.gameObject.name == "Terrain")
                {
                    //Debug.Log("Hit terrain down");
                    return -hit.distance;
                }
            }
            else if (Physics.Raycast(position, Vector3.up, out hit, 1000))
            {
                Debug.DrawLine(position, new Vector3(position.x, position.y + 500, position.z), Color.red);
                if (hit.collider.gameObject.name == "Terrain")
                {
                    //Debug.Log("Hit terrain up");
                    return hit.distance;
                }
               
            }
            //Debug.DrawLine(position, new Vector3(position.x, position.y + 500, position.z));
            return distance;
        }


        public static float[,] GetEdgeMask(ChunkEdge edgetype)
        {
            float[,] mask = new float[2,2];
            float[,] scaledmask;
            switch (edgetype)
            {
                case ChunkEdge.TL:
                    mask = new float[,] { 
                        { 0, 0 }, 
                        { 0, 1 }
                    };
                    scaledmask = MathUtils.BilinearInterpolationScale(mask, 129, 129);
                    break;
                case ChunkEdge.T:
                    mask = new float[,] {
                        { 0, 0 },
                        { 1, 1 }
                    };
                    scaledmask = MathUtils.BilinearInterpolationScale(mask, 129, 129);
                    for(int x = 0; x < 129; x++)
                    {
                        scaledmask[x, TerrainChunkSettings.ChunkSize - 1] = 1.0f;
                    }
                    break;
                case ChunkEdge.TR:
                    mask = new float[,] {
                        { 0, 0 },
                        { 1, 0 }
                    };
                    scaledmask = MathUtils.BilinearInterpolationScale(mask, 129, 129);
                    break;
                case ChunkEdge.R:
                    mask = new float[,] {
                        { 1, 0 },
                        { 1, 0 }
                    };
                    scaledmask = MathUtils.BilinearInterpolationScale(mask, 129, 129);
                    for(int z = 0; z < TerrainChunkSettings.ChunkSize; z++)
                    {
                        scaledmask[0, z] = 1.0f;
                    }
                    break;
                case ChunkEdge.BR:
                    mask = new float[,] {
                        { 1, 0 },
                        { 0, 0 }
                    };
                    scaledmask = MathUtils.BilinearInterpolationScale(mask, 129, 129);
                    break;
                case ChunkEdge.B:
                    mask = new float[,] {
                        { 1, 1 },
                        { 0, 0 }
                    };
                    scaledmask = MathUtils.BilinearInterpolationScale(mask, 129, 129);
                    for(int x = 0; x < TerrainChunkSettings.ChunkSize; x++)
                    {
                        scaledmask[x, 0] = 1.0f;
                    }
                    break;
                case ChunkEdge.BL:
                    mask = new float[,] {
                        { 0, 1 },
                        { 0, 0 }
                    };
                    scaledmask = MathUtils.BilinearInterpolationScale(mask, 129, 129);
                    break;
                case ChunkEdge.L:
                    mask = new float[,] {
                        { 0, 1 },
                        { 0, 1 }
                    };
                    scaledmask = MathUtils.BilinearInterpolationScale(mask, 129, 129);
                    for(int z = 0; z < TerrainChunkSettings.ChunkSize; z++)
                    {
                        scaledmask[TerrainChunkSettings.ChunkSize - 1, z] = 1.0f;
                    }
                    break;
            }
            scaledmask = MathUtils.BilinearInterpolationScale(mask, 129, 129);
            return scaledmask;
        }

        public static class MathUtils
        {


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

                        var newFloat = r;

                        newArr[j, i] = newFloat;
                    }
                }

                return newArr;
            }
        }
    




    //deprecated
    public static float[,] GetIslandMask(int MAP_SIZE)
        {
            float[,] mask = new float[MAP_SIZE, MAP_SIZE];
            for(int x = 0; x < MAP_SIZE; x++)
            {
                for(int z =0; z < MAP_SIZE; z++)
                {
                    float distance_x = Mathf.Abs(x - MAP_SIZE * 0.5f);
                    float distance_z = Mathf.Abs(z - MAP_SIZE * 0.5f);
                    float distance = Mathf.Sqrt((distance_x * distance_x) + (distance_z * distance_z)); // circular mask
                    float max_width = MAP_SIZE * 0.5f - 10.0f;
                    float delta = distance / max_width;
                    float gradient = delta * delta;
                    mask[x,z] = Mathf.Max(0.0f, 1.0f - gradient);
                }

            }
            return mask;
        }

    }

    public class RandomPointOnMesh : MonoBehaviour
    {
        public MeshCollider lookupCollider;

        public bool bangGetPoint;
        private Vector3 randomPoint;
       
        public List<Vector3> debugPoints;

        void Update()
        {

            //click the checkbox to generate a point, and have it shown in a debug gizmo.
            //here's a blogpost on it http://nottheinternet.com/blog/banging-things-in-Unity/
            //if (bangGetPoint)
            //{
            //    Vector3 randomPoint = GetRandomPointOnMesh(lookupCollider.sharedMesh);
            //    randomPoint += lookupCollider.transform.position;
            //    debugPoints.Add(randomPoint);
            //    Debug.Log(randomPoint);
            //    bangGetPoint = false;
            //}
        }


        //public void OnDrawGizmos()
        //{
        //    foreach (Vector3 debugPoint in debugPoints)
        //    {
        //        Gizmos.DrawSphere(debugPoint, 1f);
        //    }
        //}


        public static Vector3 GetRandomPointOnMesh(Mesh mesh)
        {
            //if you're repeatedly doing this on a single mesh, you'll likely want to cache cumulativeSizes and total
            float[] sizes = GetTriSizes(mesh.triangles, mesh.vertices);
            float[] cumulativeSizes = new float[sizes.Length];
            float total = 0;

            for (int i = 0; i < sizes.Length; i++)
            {
                total += sizes[i];
                cumulativeSizes[i] = total;
            }

            //so everything above this point wants to be factored out

            float randomsample = UnityEngine.Random.value * total;

            int triIndex = -1;

            for (int i = 0; i < sizes.Length; i++)
            {
                if (randomsample <= cumulativeSizes[i])
                {
                    triIndex = i;
                    break;
                }
            }

            if (triIndex == -1) Debug.LogError("triIndex should never be -1");

            Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
            Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
            Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

            
            //generate random barycentric coordinates

            float r = UnityEngine.Random.value;
            float s = UnityEngine.Random.value;

            if (r + s >= 1)
            {
                r = 1 - r;
                s = 1 - s;
            }
           
            //and then turn them back to a Vector3
            Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
            return pointOnMesh;

        }

        public static float[] GetTriSizes(int[] tris, Vector3[] verts)
        {
            int triCount = tris.Length / 3;
            float[] sizes = new float[triCount];
            for (int i = 0; i < triCount; i++)
            {
                sizes[i] = .5f * Vector3.Cross(verts[tris[i * 3 + 1]] - verts[tris[i * 3]], verts[tris[i * 3 + 2]] - verts[tris[i * 3]]).magnitude;
                
            }

            return sizes;

            /*
             * 
             * more readably:
             * 
                for(int ii = 0 ; ii < indices.Length; ii+=3)
                {
                    Vector3 A = Points[indices[ii]];
                    Vector3 B = Points[indices[ii+1]];
                    Vector3 C = Points[indices[ii+2]];
                    Vector3 V = Vector3.Cross(A-B, A-C);
                    Area += V.magnitude * 0.5f;
                }
             * 
             * 
             * */
        }

    }

    public static class ExtensionUtils
    {
        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        //list shuffle extention method
        // uses a seeded RNG for consistency
        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, UnityEngine.Random.Range(i, list.Count));
        }

        //List quick swap extension method
        public static void Swap<T>(this IList<T> list, int a, int b)
        {
            T tmp = list[a];
            list[a] = list[b];
            list[b] = tmp;
        }

        public static T Pop<T>(this IList<T> list)
        {
            T firstitem = list[0];
            list.RemoveAt(0);
            return firstitem;
        }
    }

    public static class StringUtils
    {
        /// <summary>
        /// Convert a string (AAAA) to an integer representing its unicode value (65656565)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int StringToUnicode(string s)
        {
            int num = 1;
            foreach(char c in s)
            {
                num += Convert.ToInt32(c);
            }
            return num;
        }

    }
    
}
