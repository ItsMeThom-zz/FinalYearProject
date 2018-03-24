using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainGenerator;
using UnityEngine;
using WorldGen;

namespace Assets.Code.Utils
{
    class TestViewer : MonoBehaviour
    {

        WorldController controller;

        WorldGenerator generatorRef;

        public Renderer WorldMap;
        public Renderer BiomeA;
        public Renderer BiomeB;
        public Renderer ChunkBlend;
        public Renderer ChunkHeightmap;

        private void Start()
        {
            controller = transform.parent.GetComponent<WorldController>();
            Debug.Log(controller);
            controller.DEV = true; //turn on debugging mode
            var tcg = controller.Generator;
            
            if(tcg != null)
            {
                Debug.Log("Got TCG");
                generatorRef = tcg.WorldGenerator;
                tcg.DEV = true;
                Debug.Log("DEBUG: Got WorldGenerator");
            }
            else
            {
                Debug.Log("Couldnt get TCG");
            }
           
        }

        public void DrawNoiseMap(Renderer renderer, float[,] noiseMap)
        {

            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);

            Texture2D texture = new Texture2D(width, height);

            Color[] colourMap = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                }
            }
            texture.SetPixels(colourMap);
            texture.Apply();

            renderer.material.mainTexture = texture;
            renderer.transform.localScale = new Vector3(width, 1, height);
        }
    }
}
