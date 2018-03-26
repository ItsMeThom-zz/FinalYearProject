//using Assets.Code.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldGen;
using Utils;

namespace TerrainGenerator
{
    public class TerrainChunkGenerator : MonoBehaviour
    {
        public bool DEV = false;
        public Material TerrainMaterial;

        public Texture2D FlatTexture;
        public Texture2D SteepTexture;
        public Texture2D SandTexture;

        private TerrainChunkSettings Settings;

        private NoiseProvider NoiseProvider;
        public WorldGenerator WorldGenerator;
        private ChunkCache Cache;

        Vector2i debugOldplaypos;

        private void Awake()
        {
            WorldGenerator = new WorldGenerator();
            WorldGenerator.GenerateWorldMap();

            var BiomeWorldmap = WorldGenerator.ElevationData;
            Settings = new TerrainChunkSettings(129, 129, 129, 40, FlatTexture, SteepTexture, SandTexture, TerrainMaterial);
           // Debug.Log("Settings Created");
            NoiseProvider = new NoiseProvider();

            Cache = gameObject.AddComponent<ChunkCache>();

            //Debug.Log("Cache Created");
        }

        private void Update()
        {

            Cache.Update();
        }

        private void GenerateChunk(int x, int z)
        {
            //Debug.Log("TCG.GenerateChunk()  => " + "[" + x + "," + z +"]");
            if (Cache.ChunkCanBeAdded(x, z))
            {
                ChunkEdge edgetype = CheckChunkIsEdge(new Vector2i(x, z));
                var chunk = new TerrainChunk(Settings, WorldGenerator, edgetype, x, z);
                Cache.AddNewChunk(chunk);
            }
        }

        private void RemoveChunk(int x, int z)
        {
            if (Cache.ChunkCanBeRemoved(x, z))
                Cache.RemoveChunk(x, z);
        }

        private List<Vector2i> GetChunkPositionsInRadius(Vector2i chunkPosition, int radius)
        {
            var result = new List<Vector2i>();

            for (var zCircle = -radius; zCircle <= radius; zCircle++)
            {
                for (var xCircle = -radius; xCircle <= radius; xCircle++)
                {
                    if (xCircle * xCircle + zCircle * zCircle < radius * radius)
                        result.Add(new Vector2i(chunkPosition.X + xCircle, chunkPosition.Z + zCircle));
                }
            }

            return result;
        }

        public void UpdateTerrain(Vector3 worldPosition, int radius)
        {
            var chunkPosition = GetChunkPosition(worldPosition);
            var newPositions = GetChunkPositionsInRadius(chunkPosition, radius);

            var loadedChunks = Cache.GetGeneratedChunks();
            var chunksToRemove = loadedChunks.Except(newPositions).ToList();

            var positionsToGenerate = newPositions.Except(chunksToRemove).ToList();
            foreach (var position in positionsToGenerate)
                GenerateChunk(position.X, position.Z);

            foreach (var position in chunksToRemove)
            {
                RemoveChunk(position.X, position.Z);
            }
                
        }

        public Vector2i GetChunkPosition(Vector3 worldPosition)
        {
            var x = (int)Mathf.Floor(worldPosition.x / Settings.Length);
            var z = (int)Mathf.Floor(worldPosition.z / Settings.Length);

            return new Vector2i(x, z);
        }

        /// <summary>
        /// Checks chunks world position and applies height masking as nessecary
        /// to turn edge chunks into beaches
        /// </summary>
        /// <param name="Position">World Position of this chunk</param>
        /// <returns></returns>
        public ChunkEdge CheckChunkIsEdge(Vector2i Position)
        {
            ChunkEdge edgetype = ChunkEdge.NotEdge;
            var mapwidth = WorldGenerator.MAP_SIZE -  1;
            var mapheight = WorldGenerator.MAP_SIZE - 1;
            if (Position.X == (int)(mapwidth / 2) - mapwidth) //leftmost edge
            {
                if (Position.Z == (int)(mapheight / 2) - mapheight)
                {
                    //top left
                    
                    edgetype = ChunkEdge.TL;
                }
                else if (Position.Z == (int)(mapheight / 2))
                {
                    //bottom left
                    edgetype = ChunkEdge.BL;
                }
                else
                {
                    //left
                    edgetype = ChunkEdge.L;
                }
            }
            else if (Position.X == (int)(mapwidth / 2)) //right edge
            {
                if (Position.Z == (int)(mapheight / 2) - mapheight)
                {
                    //top right
                    edgetype = ChunkEdge.TR;
                }
                else if (Position.Z == (int)(mapheight / 2))
                {
                    //bottom right
                    edgetype = ChunkEdge.BR;
                }
                else
                {
                    //right
                    edgetype = ChunkEdge.R;
                }
            }
            else if (Position.Z == (int)(mapheight / 2) - mapwidth) //top edge
            {
                if (Position.X == (int)(mapwidth / 2) - mapwidth)
                {
                    //top left
                    edgetype = ChunkEdge.TL;
                }
                else if (Position.X == (int)(mapwidth / 2))
                {
                    //top right
                    edgetype = ChunkEdge.TR;
                }
                else
                {
                    //top
                    edgetype = ChunkEdge.T;
                }
            }
            else if (Position.Z == (int)(mapwidth / 2)) //bottom edge
            {
                if (Position.X == (int)(mapheight / 2) - mapheight)
                {
                    //bottom left
                    edgetype = ChunkEdge.BL;
                }
                else if (Position.X == (int)(mapheight / 2))
                {
                    //bottom right
                    edgetype = ChunkEdge.BR;
                }
                else
                {
                    //bottom
                    edgetype = ChunkEdge.B;
                }
            }
            else
            {
                edgetype = ChunkEdge.NotEdge;
            }
            //Debug.Log("ctor: Chunk is " + edgetype);
            return edgetype;
        }

        public bool IsTerrainAvailable(Vector3 worldPosition)
        {
            var chunkPosition = GetChunkPosition(worldPosition);
            return Cache.IsChunkGenerated(chunkPosition);
        }

        public float GetTerrainHeight(Vector3 worldPosition)
        {
            var chunkPosition = GetChunkPosition(worldPosition);
            var chunk = Cache.GetGeneratedChunk(chunkPosition);
            if (chunkPosition != null)
                return chunk.GetTerrainHeight(worldPosition);

            return 0;
        }
    }
}