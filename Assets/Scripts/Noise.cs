using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osman
{
    public static class Noise
    {
        public static float[,] GenerateNoise(int width, int length, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
        {
            float[,] heightMap = new float[width, length];
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;

            System.Random prng = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];

            for(int i = 0; i < octaves; i++){
                float offsetX = prng.Next(-10000,10000)+ offset.x;
                float offsetY = prng.Next(-10000,10000)+ offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if(scale <= 0)
                scale = 0.0001f;

            float halfWidth = width / 2f;
            float halfLength = length / 2f;

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < length; y++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for(int o = 0; o < octaves; o++)
                    {
                        float sampleX = ((x-halfWidth ) / scale + octaveOffsets[o].x) * frequency;
                        float sampleY = ((y-halfLength ) / scale + octaveOffsets[o].y)  * frequency ;

                        noiseHeight += (Mathf.PerlinNoise(sampleX,sampleY) * 2 - 1) * amplitude;
                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    if(noiseHeight < min)
                        min = noiseHeight;
                    else if(noiseHeight > max)
                        max = noiseHeight;

                    heightMap[x,y] = noiseHeight;
                }
            }

            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < length; j++)
                {
                    heightMap[i,j] = Mathf.InverseLerp(min,max,heightMap[i,j]);
                }
            }

            return heightMap;
        }
    }
}
