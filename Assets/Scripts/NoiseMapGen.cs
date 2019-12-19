using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapGen : MonoBehaviour
{
    public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ, Wave[] waves)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                // calculate sample indices based on the coordinates and the scale
                float sampleX = (xIndex + offsetX) / scale;
                float sampleZ = (zIndex + offsetZ)/ scale;

                float noise = 0f;
                float normalization = 0f; ;
                // generate noise value using PerlinNoise
                for (int i = 0; i < waves.Length; i++)
                {
                    noise += waves[i].amplitude * Mathf.PerlinNoise(sampleX * waves[i].frequency + waves[i].seed, sampleZ * waves[i].frequency + waves[i].seed);
                    normalization += waves[i].amplitude;
                }
                // normalize the noise value so that it is within 0 and 1
                noise /= normalization;
                noiseMap[zIndex, xIndex] = noise;
            }
        }

        return noiseMap;
    }

    [System.Serializable]
    public class Wave
    {
        public float seed;
        public float frequency;
        public float amplitude;
    }
}
