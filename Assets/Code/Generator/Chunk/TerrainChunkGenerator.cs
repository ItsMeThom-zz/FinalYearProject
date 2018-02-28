using Assets.Code.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldGen;

namespace TerrainGenerator
{
    public class TerrainChunkGenerator : MonoBehaviour
    {
        public bool DEV = false;
        public Material TerrainMaterial;

        public Texture2D FlatTexture;
        public Texture2D SteepTexture;

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
            Settings = new TerrainChunkSettings(129, 129, 129, 40, FlatTexture, SteepTexture, TerrainMaterial);
           // Debug.Log("Settings Created");
            NoiseProvider = new NoiseProvider();

            Cache = new ChunkCache();

            //Debug.Log("Cache Created");
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //}
            if (DEV)
            {
                
                var tester = GameObject.FindObjectOfType<TestViewer>();
                if(tester != null)
                {
                    var controller = GameObject.FindObjectOfType<GameController>();
                    if (controller.IsReady)
                    {
                        Debug.Log("Doing Debug map draw");
                        var playerPosition = GameObject.FindObjectOfType<GameController>().PreviousPlayerChunkPosition;
                        var currentChunk = Cache.LoadedChunks[playerPosition];
                        if(playerPosition != debugOldplaypos)
                        {
                            //tester.DrawNoiseMap(tester.ChunkHeightmap.GetComponent<Renderer>(), currentChunk.Heightmap);
                            //tester.DrawNoiseMap(tester.BiomeA.GetComponent<Renderer>(), currentChunk.BiomeA);
                            //tester.DrawNoiseMap(tester.BiomeB.GetComponent<Renderer>(), currentChunk.BiomeB);
                            //tester.DrawNoiseMap(tester.ChunkBlend.GetComponent<Renderer>(), currentChunk.ChunkBlended);
                            //tester.DrawNoiseMap(tester.WorldMap.GetComponent<Renderer>(), WorldGenerator.ElevationData);
                            //debugOldplaypos = playerPosition;
                        }
                        
                    }
                }
                else
                {
                    Debug.Log("Couldnt get TestViewer object");
                }
            }
            
            Cache.Update();
        }

        private void GenerateChunk(int x, int z)
        {
            //Debug.Log("TCG.GenerateChunk()  => " + "[" + x + "," + z +"]");
            if (Cache.ChunkCanBeAdded(x, z))
            {
                var chunk = new TerrainChunk(Settings, WorldGenerator, NoiseProvider, x, z);
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
                RemoveChunk(position.X, position.Z);
        }

        public Vector2i GetChunkPosition(Vector3 worldPosition)
        {
            var x = (int)Mathf.Floor(worldPosition.x / Settings.Length);
            var z = (int)Mathf.Floor(worldPosition.z / Settings.Length);

            return new Vector2i(x, z);
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