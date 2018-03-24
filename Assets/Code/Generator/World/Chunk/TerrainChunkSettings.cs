using UnityEngine;

namespace TerrainGenerator
{
    public enum ChunkEdge { TL, L, BL, B, BR, R, TR, T,
        NotEdge
    }

    public class TerrainChunkSettings
    {
        public static int ChunkSize = 129;
        public static float SeaLevelGlobal = 11.0f;
        public int HeightmapResolution { get; private set; }

        public int AlphamapResolution { get; private set; }

        public int Length { get; private set; }

        public int Height { get; private set; }

        public Texture2D FlatTexture { get; private set; }

        public Texture2D SteepTexture { get; private set; }

        public Texture2D SandTexture { get; private set; }

        public Material TerrainMaterial { get; private set; }

        public float Scale = 26.543f;

        public int EdgeFalloff = 20; //falloff distance for blending edge seams

        public TerrainChunkSettings(int heightmapResolution, int alphamapResolution, int length, int height, Texture2D flatTexture, Texture2D steepTexture, Texture2D sandTexture, Material terrainMaterial)
        {
            HeightmapResolution = heightmapResolution;
            AlphamapResolution = alphamapResolution;
            Length = length;
            Height = height;
            FlatTexture = flatTexture;
            SteepTexture = steepTexture;
            SandTexture = sandTexture;
            TerrainMaterial = terrainMaterial;
        }
    }
}