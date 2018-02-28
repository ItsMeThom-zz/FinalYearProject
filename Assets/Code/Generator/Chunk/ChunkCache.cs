using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace TerrainGenerator
{
    public class ChunkCache
    {
        private readonly int MaxChunkThreads = 3;

        private Dictionary<Vector2i, TerrainChunk> RequestedChunks { get; set; }

        private Dictionary<Vector2i, TerrainChunk> ChunksBeingGenerated { get; set; }

        public Dictionary<Vector2i, TerrainChunk> LoadedChunks { get; set; }

        private HashSet<Vector2i> ChunksToRemove { get; set; }

        public OnChunkGeneratedDelegate OnChunkGenerated { get; set; }

        public ChunkCache()
        {
            RequestedChunks = new Dictionary<Vector2i, TerrainChunk>();
            ChunksBeingGenerated = new Dictionary<Vector2i, TerrainChunk>();
            LoadedChunks = new Dictionary<Vector2i, TerrainChunk>();
            ChunksToRemove = new HashSet<Vector2i>();
        }

        public void Update()
        {
            TryToDeleteQueuedChunks();

            GenerateHeightmapForAvailableChunks();
            CreateTerrainForReadyChunks();
        }

        public void AddNewChunk(TerrainChunk chunk)
        {
            RequestedChunks.Add(chunk.Position, chunk);
            GenerateHeightmapForAvailableChunks();
        }

        public void RemoveChunk(int x, int z)
        {
            ChunksToRemove.Add(new Vector2i(x, z));
            TryToDeleteQueuedChunks();
        }

        public bool ChunkCanBeAdded(int x, int z)
        {
            var key = new Vector2i(x, z);
            return
                !(RequestedChunks.ContainsKey(key)
                || ChunksBeingGenerated.ContainsKey(key)
                || LoadedChunks.ContainsKey(key));
        }

        public bool ChunkCanBeRemoved(int x, int z)
        {
            var key = new Vector2i(x, z);
            return
                RequestedChunks.ContainsKey(key)
                || ChunksBeingGenerated.ContainsKey(key)
                || LoadedChunks.ContainsKey(key);
        }

        public bool IsChunkGenerated(Vector2i chunkPosition)
        {
            return GetGeneratedChunk(chunkPosition) != null;
        }

        public TerrainChunk GetGeneratedChunk(Vector2i chunkPosition)
        {
            if (LoadedChunks.ContainsKey(chunkPosition))
                return LoadedChunks[chunkPosition];

            return null;
        }

        public List<Vector2i> GetGeneratedChunks()
        {
            return LoadedChunks.Keys.ToList();
        }

        private void GenerateHeightmapForAvailableChunks()
        {
            var requestedChunks = RequestedChunks.ToList();
            if (requestedChunks.Count > 0 && ChunksBeingGenerated.Count < MaxChunkThreads)
            {
                var chunksToAdd = requestedChunks.Take(MaxChunkThreads - ChunksBeingGenerated.Count);
                foreach (var chunkEntry in chunksToAdd)
                {
                    ChunksBeingGenerated.Add(chunkEntry.Key, chunkEntry.Value);
                    RequestedChunks.Remove(chunkEntry.Key);

                    chunkEntry.Value.GenerateHeightmap();
                }
            }
        }

        private void CreateTerrainForReadyChunks()
        {
            var anyTerrainCreated = false;

            var chunks = ChunksBeingGenerated.ToList();
            foreach (var chunk in chunks)
            {
                if (chunk.Value.IsHeightmapReady())
                {
                    /*
                     Need to average chunk edges with neighbours here
                     in order to remove seams at chunk edges
                     */
                    #region Old Edge blending code
                    //Vector2i chunkKey = chunk.Key;
                    //TerrainChunk xUp;
                    //TerrainChunk xDown;
                    //TerrainChunk zUp;
                    //TerrainChunk zDown;

                    ////if we cant find the neighbour in the generating queue, check loaded chunks
                    //if (!ChunksBeingGenerated.TryGetValue(new Vector2i(chunkKey.X + 1, chunkKey.Z), out xUp))
                    //{
                    //    LoadedChunks.TryGetValue(new Vector2i(chunkKey.X + 1, chunkKey.Z), out xUp);
                    //}
                    //if (!ChunksBeingGenerated.TryGetValue(new Vector2i(chunkKey.X - 1, chunkKey.Z), out xDown))
                    //{
                    //    LoadedChunks.TryGetValue(new Vector2i(chunkKey.X + 1, chunkKey.Z), out xUp);
                    //}
                    //if (!ChunksBeingGenerated.TryGetValue(new Vector2i(chunkKey.X, chunkKey.Z + 1), out zUp))
                    //{
                    //    LoadedChunks.TryGetValue(new Vector2i(chunkKey.X, chunkKey.Z + 1), out zUp);
                    //}
                    //if (!ChunksBeingGenerated.TryGetValue(new Vector2i(chunkKey.X, chunkKey.Z - 1), out zDown))
                    //{
                    //    LoadedChunks.TryGetValue(new Vector2i(chunkKey.X, chunkKey.Z - 1), out zDown);
                    //}

                    //var chunkHeightmap = chunk.Value.Heightmap;
                    //if (xUp != null) //east neighbour
                    //{
                    //    Debug.Log("Blending edge of" + chunk.Value.Position.X + ", " + chunk.Value.Position.Z + " to east");
                    //    var chunkBHeightmap = xUp.Heightmap;
                    //    MathUtils.AverageBothChunks(ref chunkHeightmap, ref chunkBHeightmap, EdgeDirection.NORTH, true);
                    //    chunk.Value.Heightmap = chunkHeightmap;
                    //    xUp.Heightmap = chunkBHeightmap;
                    //}
                    //if (xDown != null) //west neighbour
                    //{
                    //    var chunkBHeightmap = xDown.Heightmap;
                    //    MathUtils.AverageBothChunks(ref chunkHeightmap, ref chunkBHeightmap, EdgeDirection.SOUTH, true);
                    //    chunk.Value.Heightmap = chunkHeightmap;
                    //    xDown.Heightmap = chunkBHeightmap;
                    //}
                    //if (zUp != null) // north neighbour
                    //{
                    //    var chunkBHeightmap = zUp.Heightmap;
                    //    MathUtils.AverageBothChunks(ref chunkHeightmap, ref chunkBHeightmap, EdgeDirection.EAST, true);
                    //    chunk.Value.Heightmap = chunkHeightmap;
                    //    zUp.Heightmap = chunkBHeightmap;

                    //}
                    //if (zDown != null) //south neighbour
                    //{
                    //    var chunkBHeightmap = zDown.Heightmap;
                    //    MathUtils.AverageBothChunks(ref chunkHeightmap, ref chunkBHeightmap, EdgeDirection.WEST, true);
                    //    chunk.Value.Heightmap = chunkHeightmap;
                    //    zDown.Heightmap = chunkBHeightmap;
                    //}
                    #endregion

                    ChunksBeingGenerated.Remove(chunk.Key);
                    LoadedChunks.Add(chunk.Key, chunk.Value);

                    chunk.Value.CreateTerrain();

                    anyTerrainCreated = true;
                    if (OnChunkGenerated != null)
                        OnChunkGenerated.Invoke(ChunksBeingGenerated.Count);

                    SetChunkNeighborhood(chunk.Value);
                }
            }

            if (anyTerrainCreated)
                UpdateAllChunkNeighbors();
        }

        private void TryToDeleteQueuedChunks()
        {
            var chunksToRemove = ChunksToRemove.ToList();
            foreach (var chunkPosition in chunksToRemove)
            {
                if (RequestedChunks.ContainsKey(chunkPosition))
                {
                    RequestedChunks.Remove(chunkPosition);
                    ChunksToRemove.Remove(chunkPosition);
                }
                else if (LoadedChunks.ContainsKey(chunkPosition))
                {
                    var chunk = LoadedChunks[chunkPosition];
                    chunk.Remove();

                    LoadedChunks.Remove(chunkPosition);
                    ChunksToRemove.Remove(chunkPosition);
                }
                else if (!ChunksBeingGenerated.ContainsKey(chunkPosition))
                    ChunksToRemove.Remove(chunkPosition);
            }
        }

        private void SetChunkNeighborhood(TerrainChunk chunk)
        {
            TerrainChunk xUp;
            TerrainChunk xDown;
            TerrainChunk zUp;
            TerrainChunk zDown;

            LoadedChunks.TryGetValue(new Vector2i(chunk.Position.X + 1, chunk.Position.Z), out xUp);
            LoadedChunks.TryGetValue(new Vector2i(chunk.Position.X - 1, chunk.Position.Z), out xDown);
            LoadedChunks.TryGetValue(new Vector2i(chunk.Position.X, chunk.Position.Z + 1), out zUp);
            LoadedChunks.TryGetValue(new Vector2i(chunk.Position.X, chunk.Position.Z - 1), out zDown);

            if (xUp != null)
            {
                chunk.SetNeighbors(xUp, TerrainNeighbor.XUp);
                xUp.SetNeighbors(chunk, TerrainNeighbor.XDown);
            }
            if (xDown != null)
            {
                chunk.SetNeighbors(xDown, TerrainNeighbor.XDown);
                xDown.SetNeighbors(chunk, TerrainNeighbor.XUp);
            }
            if (zUp != null)
            {
                chunk.SetNeighbors(zUp, TerrainNeighbor.ZUp);
                zUp.SetNeighbors(chunk, TerrainNeighbor.ZDown);
            }
            if (zDown != null)
            {
                chunk.SetNeighbors(zDown, TerrainNeighbor.ZDown);
                zDown.SetNeighbors(chunk, TerrainNeighbor.ZUp);
            }
        }

        private void UpdateAllChunkNeighbors()
        {
            foreach (var chunkEntry in LoadedChunks)
                chunkEntry.Value.UpdateNeighbors();
        }
    }
}