using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;
using UnityEngine;

namespace Utils
{
    public enum EdgeDirection {NORTH, SOUTH, EAST, WEST }

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
            int CHUNK_SIZE = TerrainChunkSettings.ChunkSize;
            int distance = 100;
            float average = 0.0f;
            float oldA;
            float oldB;

            switch (direction)
            {
                case EdgeDirection.NORTH: //blend A northedge with B southedge
                    //aX is 0->chunksize
                    //aZ is 0->20
                    //bX is aX
                    //bZ is chunksize-aZ (mirrors aZ in z-axis)
                    for (int aX = 0; aX <= CHUNK_SIZE; aX++)
                    {
                        for (int aZ = 0; aZ < distance; aZ++)
                        {
                            var weight = Mathf.SmoothStep(0, 1, (aZ / distance));
                            oldA = chunkA[aX, aZ];
                            oldB = chunkB[aX, (CHUNK_SIZE) - aZ]; //mirror point across z axis
                            average = (oldA + oldB) / 2;
                            //new heights
                            chunkA[aX, aZ] = oldA + (average - oldA) * weight;
                            if (dual)
                            {
                                chunkB[aX, (CHUNK_SIZE) - aZ] = oldB + (average - oldB) * weight;
                            }

                        }
                    }
                    break;
                case EdgeDirection.SOUTH: //blendA southedge with B northedge
                    for (int aX = 0; aX <= CHUNK_SIZE; aX++)
                    {
                        for (int aZ = CHUNK_SIZE; aZ > CHUNK_SIZE - distance; aZ--)
                        {
                            var weight = Mathf.SmoothStep(0, 1, ((CHUNK_SIZE - aZ) / distance));
                            oldA = chunkA[aX, aZ];
                            oldB = chunkB[aX, 0 + ((CHUNK_SIZE) - aZ)]; //mirror point across z axis
                            average = (oldA + oldB) / 2;
                            //new heights
                            chunkA[aX, aZ] = oldA + (average - oldA) * weight;
                            if (dual)
                            {
                                chunkB[aX, CHUNK_SIZE - aZ] = oldB + (average - oldB) * weight;
                            }

                        }
                    }
                    break;
                case EdgeDirection.EAST: //blendA eastedge with B westedge
                    for (int aZ = 0; aZ <= CHUNK_SIZE; aZ++)
                    {
                        for (int aX = CHUNK_SIZE; aX > CHUNK_SIZE - distance; aX--)
                        {

                            var weight = Mathf.SmoothStep(0, 1, ((CHUNK_SIZE - aX) / distance));
                            oldA = chunkA[aX, aZ];
                            oldB = chunkB[(CHUNK_SIZE - aX), aZ]; //mirror point across x axis
                            average = (oldA + oldB) / 2;
                            chunkA[aX, aZ] = oldA + (average - oldA) * weight;
                            if (dual)
                            {
                                chunkB[(CHUNK_SIZE - aX), aZ] = oldB + (average - oldB) * weight;
                            }

                        }
                    }
                    break;
                case EdgeDirection.WEST: //blendA westedge with B eastedge
                    for (int aZ = 0; aZ <= CHUNK_SIZE; aZ++)
                    {
                        for (int aX = 0; aX < distance; aX++)
                        {
                            var weight = Mathf.SmoothStep(0, 1, (aX / distance));
                            oldA = chunkA[aX, aZ];
                            oldB = chunkB[CHUNK_SIZE - aX, aZ];
                            average = (oldA + oldB) / 2;
                            chunkA[aX, aZ] = oldA + (average - oldA) * weight;
                            if (dual)
                            {
                                chunkB[(-aX + CHUNK_SIZE), aZ] = oldB + (average - oldB) * weight;
                            }
                        }
                    }
                    break;
            }
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
            if (Physics.Raycast(position, Vector3.down, out hit, 500))
            {
                Debug.DrawLine(position, new Vector3(position.x, position.y - 500, position.z), Color.green);
                if (hit.collider.gameObject.name == "Terrain")
                {
                    Debug.Log("Hit terrain down");
                    return -hit.distance;
                }
            }
            else if (Physics.Raycast(position, Vector3.up, out hit, 500))
            {
                Debug.DrawLine(position, new Vector3(position.x, position.y + 500, position.z), Color.red);
                if (hit.collider.gameObject.name == "Terrain")
                {
                    Debug.Log("Hit terrain up");
                    return hit.distance;
                }
               
            }
            //Debug.DrawLine(position, new Vector3(position.x, position.y + 500, position.z));
            return distance;
        }

    }
}
