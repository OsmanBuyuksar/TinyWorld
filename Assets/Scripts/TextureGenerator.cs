using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osman
{
    public static class TextureGenerator
    {
        public static Texture2D GenerateNoiseTexture(float[,] noiseMap){
            int width = noiseMap.GetLength(0);
            int length = noiseMap.GetLength(1);

            Texture2D texture = new Texture2D(width, length);

            Color[] colourMap = new Color[width * length];
            for(int y = 0; y < length; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x,y]);
                }
            }
            texture.SetPixels(colourMap);
            texture.Apply();

            return texture;
        }

        public static Texture2D GenerateColorMap(float[,] noiseMap, TerrainType[] regions){
            int width = noiseMap.GetLength(0);
            int length = noiseMap.GetLength(1);

            Texture2D texture = new Texture2D(width, length);
            texture.wrapModeU = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;

            Color[] colourMap = new Color[width * length];
            for(int y = 0; y < length; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    for(int i = 0; i< regions.Length; i++){
                        if(noiseMap[x,y] <= regions[i].height){
                            colourMap[y * width + x] = regions[i].color;
                            break;
                        }
                    }
                    
                }
            }
            texture.SetPixels(colourMap);
            texture.Apply();

            return texture;
        }

        public static Texture2D GenerateRegionTexture(Region[,] regionGrid, SerializableDict<Region, Color> regions)
        {
            int width = regionGrid.GetLength(0);
            int length = regionGrid.GetLength(1);

            Texture2D texture = new Texture2D(width, length);
            texture.wrapModeU = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;

            Color[] colourMap = new Color[width * length];


            //colourMap[] = regions[region. <...>]  ?
            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colourMap[y * width + x] = regions[regionGrid[x, y]];
                }
            }
            texture.SetPixels(colourMap);
            texture.Apply();



            return texture;
        }
    }
}
