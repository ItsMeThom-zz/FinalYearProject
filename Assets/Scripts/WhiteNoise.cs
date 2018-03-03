//-----------------------------------------------------------------------------
//
// Title: 		WhiteNoise
// Creator: 	FKM1900
// Based on: 	Herman Tulleken, http://devmag.org.za/2009/04/25/perlin-noise/
//
//-----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class WhiteNoise {

	// Generate white noise
	public static float[,] GenerateWhiteNoise(int seed, int noiseWidth, int noiseHeight) {
	
		float[,] noise = new float[noiseWidth, noiseHeight];
		System.Random random = new System.Random(seed);		
		
		for(int x = 0; x < noiseWidth; x++) {
		
			for(int y = 0; y < noiseHeight; y++) {
			
				noise[x, y] = (float) random.NextDouble() % 1;
			}
		}
		
		return noise;
	}
}
