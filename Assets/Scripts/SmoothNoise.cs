//-----------------------------------------------------------------------------
//
// Title: 		SmoothNoise
// Creator: 	FKM1900
// Based on: 	Herman Tulleken, http://devmag.org.za/2009/04/25/perlin-noise/
//
//-----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class SmoothNoise {
	
	// Generate smooth noise
	public static float[,] GenerateSmoothNoise(float[,] baseNoise, int octave, int noiseWidth, int noiseHeight) {
				
		float[,] noise = new float[noiseWidth, noiseHeight];
		int samplePeriod = 1 << octave; // Wavelength (2 ^ k)
		float sampleFrequency = 1.0f / samplePeriod; // Frequency (1 / 2 ^ k)
		
		for(int x = 0; x < noiseWidth; x++) {
			
			// Calculate horizontal sampling indices
			int sample_x0 = (x / samplePeriod) * samplePeriod;
			int sample_x1 = (sample_x0 + samplePeriod) % noiseWidth; // Wrap around
			float horizontal_blend = (x - sample_x0) * sampleFrequency;
			
			for(int y = 0; y < noiseHeight; y++) {
				
				// Calculate vertical sampling indices
				int sample_y0 = (y / samplePeriod) * samplePeriod;
				int sample_y1 = (sample_y0 + samplePeriod) % noiseHeight; // Wrap around
				float vertical_blend = (y - sample_y0) * sampleFrequency;
				
				// Blend top two corners
				float top = Interpolate(baseNoise[sample_x0, sample_y0], baseNoise[sample_x1, sample_y0], horizontal_blend);
				
				// Blend bottom two corners
				float bottom = Interpolate(baseNoise[sample_x0, sample_y1], baseNoise[sample_x1, sample_y1], horizontal_blend);
				
				// Final blend
				noise[x, y] = Interpolate(top, bottom, vertical_blend);
			}
		}
		
		return noise;
	}
	
	// Interpolate
	static float Interpolate(float x0, float x1, float alpha) {
		
		return x0 * (1f - alpha) + alpha * x1;
	}
}
