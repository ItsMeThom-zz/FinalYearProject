//-----------------------------------------------------------------------------
//
// Title: 		PerlinNoise
// Creator: 	FKM1900
// Based on: 	Herman Tulleken, http://devmag.org.za/2009/04/25/perlin-noise/
//
//-----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PerlinNoise : MonoBehaviour {
							
	// Generate perlin noise
	public static float[,] GeneratePerlinNoise(int seed, int octaveCount, float persistence, int noiseWidth, int noiseHeight) {
		
		List<float[,]> smoothNoiseOctaves = new List<float[,]>();
		
		// Get smooth noise octaves
		for(int i = 0; i < octaveCount; i++) {
			
			smoothNoiseOctaves.Add(SmoothNoise.GenerateSmoothNoise(WhiteNoise.GenerateWhiteNoise(seed, noiseWidth, noiseHeight), i, noiseWidth, noiseHeight));
		}
		
		float[,] noise = new float[noiseWidth, noiseHeight];
		float amplitude = 1.0f;
		float totalAmplitude = 0.0f;
		
		// Blend noise together
		for(int octave = octaveCount - 1; octave >= 0; octave--) {
			
			amplitude *= persistence;
			totalAmplitude += amplitude;
			
			for(int i = 0; i < noiseWidth; i++) {
				
				for(int j = 0; j < noiseHeight; j++) {
					
					noise[i, j] += smoothNoiseOctaves[octave][i, j] * amplitude;
				}
			}
		}
		
		// Normalise
		for(int i = 0; i < noiseWidth; i++) {
			
			for(int j = 0; j < noiseHeight; j++) {
				
				noise[i, j] /= totalAmplitude;
			}
		}
		
		return noise;
	}
}
