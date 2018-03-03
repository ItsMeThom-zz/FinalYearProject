//-----------------------------------------------------------------------------
//
// Title: 		Clouds
// Creator: 	FKM1900
// Based on: 	-
//
//-----------------------------------------------------------------------------


using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Clouds : MonoBehaviour {
	
	Texture2D texture;
	
	public int width = 128;
	public int height = 128;
	
	public Color cloudColor = Color.white;
	
	public float scale = 5f;
	public int octaves = 6;
	public float persistence = 0.6f;
	public int seed = 0;
	
	public float contrastLow = 0f;
	public float contrastHigh = 1f;
	public float brightnessOffset = 0f;
	
	public float xSpeed = 0.001f;
	public float ySpeed = 0.0005f;

    private Renderer rendr;
	
	// Start
	void Start () {

        rendr = GetComponent<Renderer>();
		texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        rendr.sharedMaterial.mainTexture = texture;
		
		GenerateCloudNoise(width, height);
	}
	
	// Update
	void Update () {
				
		rendr.sharedMaterial.mainTextureOffset = new Vector2(rendr.material.mainTextureOffset.x + (xSpeed / 100), rendr.sharedMaterial.mainTextureOffset.y + (ySpeed / 100));
	}
	
	// Generate cloud noise
	void GenerateCloudNoise(int noiseWidth, int noiseHeight) {
	
		float[,] perlinNoise = PerlinNoise.GeneratePerlinNoise(seed, octaves, persistence, noiseWidth, noiseHeight);
		float noiseValue;
		
		for(int y = 0; y < noiseWidth; y++) {
			
			for(int x = 0; x < noiseHeight; x++) {
				
				noiseValue = perlinNoise[x, y];
				noiseValue *= SimplexNoise.SeamlessNoise((float) x / (float) width, (float) y / (float) height, scale, scale, 0f);

				noiseValue = Mathf.Clamp(noiseValue, contrastLow, contrastHigh + contrastLow) - contrastLow;
				noiseValue = Mathf.Clamp(noiseValue, 0f, 1f);
				
				float r = Mathf.Clamp(cloudColor.r + brightnessOffset, 0f, 1f);
				float g = Mathf.Clamp(cloudColor.g + brightnessOffset, 0f, 1f);
				float b = Mathf.Clamp(cloudColor.b + brightnessOffset, 0f, 1f);
				
				texture.SetPixel(x, y, new Color(r, g, b, noiseValue));
			}
		}
		
		texture.Apply();
	}
}
