using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code.Utils
{

public class TerrainSplatMapper : MonoBehaviour
    {

        // Using Serializable allows us to embed a class with sub properties in the inspector.
        [Serializable]
        public class TextureAttributes
        {
            public string name;
            public int index;
            public bool defaultTexture = false;
            [Range(0.0f, 1.0f)]
            public float minSteepness;
            [Range(0.0f, 1.0f)]
            public float maxSteepness;
            [Range(0.0f, 1.0f)]
            public float minAltitude;
            [Range(0.0f, 1.0f)]
            public float maxAltitude;
        }

        public List<TextureAttributes> listTextures = new List<TextureAttributes>();
        private Terrain terrain;
        private TerrainData terrainData;
        private int indexOfDefaultTexture;

        void Start()
        {
            PaintTerrain();
            
        }

        public void PaintTerrain()
        {
            // Get the attached terrain component
            terrain = GetComponent<Terrain>();

            // Get a reference to the terrain data
            terrainData = terrain.terrainData;

            //This is the # of textures you have added in the terrain editor
            int nbTextures = terrainData.alphamapLayers;

            //See below for the definition of GetMaxHeight
            float maxHeight = GetMaxHeight(terrainData, terrainData.heightmapWidth);

            // Your texture data (i.e. Splatmap) is stored internally as a 3d array of floats with x and y location as the first 2 dimensions of the array and the index of the texture to be used as the 3rd dimension
            float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

            //This is just in case someone mixed up min and max when completing the inspector for this script
            for (int i = 0; i < listTextures.Count; i++)
            {
                if (listTextures[i].minAltitude > listTextures[i].maxAltitude)
                {
                    float temp = listTextures[i].minAltitude;
                    listTextures[i].minAltitude = listTextures[i].maxAltitude;
                    listTextures[i].maxAltitude = temp;
                }

                if (listTextures[i].minSteepness > listTextures[i].maxSteepness)
                {
                    float temp2 = listTextures[i].minSteepness;
                    listTextures[i].minSteepness = listTextures[i].maxSteepness;
                    listTextures[i].maxSteepness = temp2;
                }
            }

            //For some reason you need a default texture in Unity
            for (int i = 0; i < listTextures.Count; i++)
            {
                if (listTextures[i].defaultTexture)
                {
                    indexOfDefaultTexture = listTextures[i].index;
                    break;
                }
            }


            for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
                for (int x = 0; x < terrainData.alphamapWidth; x++)
                {
                    // Normalise x/y coordinates to range 0-1
                    float y_01 = (float)y / (float)terrainData.alphamapHeight;
                    float x_01 = (float)x / (float)terrainData.alphamapWidth;

                    // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                    float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * terrainData.heightmapWidth));

                    //Normalise the height by dividing it by maxHeight
                    float normHeight = height / maxHeight;

                    // Calculate the steepness of the terrain at this location
                    float steepness = terrainData.GetSteepness(y_01, x_01);

                    // Normalise the steepness by dividing it by the maximum steepness: 90 degrees
                    float normSteepness = steepness / 90.0f;

                    //Erase existing splatmap at this point
                    for (int i = 0; i < terrainData.alphamapLayers; i++)
                    {
                        splatmapData[x, y, i] = 0.0f;
                    }

                    // Setup an array to record the mix of texture weights at this point
                    float[] splatWeights = new float[terrainData.alphamapLayers];


                    for (int i = 0; i < listTextures.Count; i++)
                    {

                        //The rules you defined in the inspector are being applied for each texture
                        if (normHeight >= listTextures[i].minAltitude && normHeight <= listTextures[i].maxAltitude && normSteepness >= listTextures[i].minSteepness && normSteepness <= listTextures[i].maxSteepness)
                        {
                            splatWeights[listTextures[i].index] += 1.0f;
                          
                        }
                    }

                    // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                    float z = splatWeights.Sum();

                    //If z is 0 at this location (i.e. no texture was applied), put default texture
                    if (Mathf.Approximately(z, 0.0f))
                    {
                        splatWeights[indexOfDefaultTexture] = 1.0f;
                    }

                    // Loop through each terrain texture
                    for (int i = 0; i < terrainData.alphamapLayers; i++)
                    {

                        // Normalize so that sum of all texture weights = 1
                        splatWeights[i] /= z;

                        // Assign this point to the splatmap array
                        splatmapData[x, y, i] = splatWeights[i];
                    }
                }
            }

            // Finally assign the new splatmap to the terrainData:
            terrainData.SetAlphamaps(0, 0, splatmapData);
        }

        //This is to get the maximum altitude of your terrain. For some reason TerrainData.
        private float GetMaxHeight(TerrainData tData, int heightmapWidth)
        {

            float maxHeight = 0f;

            for (int x = 0; x < heightmapWidth; x++)
            {
                for (int y = 0; y < heightmapWidth; y++)
                {
                    if (tData.GetHeight(x, y) > maxHeight)
                    {
                        maxHeight = tData.GetHeight(x, y);
                    }
                }
            }
            return maxHeight;
        }
    }


}
