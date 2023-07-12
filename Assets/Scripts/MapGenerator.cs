using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace osman
{
    public class MapGenerator : MonoBehaviour
    { 
        
        [SerializeField] public DrawMode drawMode = DrawMode.Noise;
        [SerializeField] TerrainType[] regions;

        //Mesh Parameters
        //
        [Space(2)]
        [Tooltip("Mesh Parameters")]
        [SerializeField] private AnimationCurve heightCurve;
        [SerializeField] private float heightMultiplier = 1f;
        [Range(0, 6)]
        [SerializeField] private int levelOfSimplification;
        [Range(0,1)]
        [SerializeField] private float wfcToNoiseRatio;
        [SerializeField] private AnimationCurve wfcEffectCurve;

        //Noise Parameters
        //
        [Space(2)]
        [Tooltip("Noise Parameters")]
        public const int mapWidthVertexCount = 241;
        public float scale;
        public float lacunarity = 1f;
        [Range(0,1)]
        public float persistence = 1f;
        public int octaves = 1;
        public int seed;
        public Vector2 offset;

        //WFC Parameters
        //
        [Space(2)]
        [Tooltip("WFC Parameters")]
        [SerializeField] SerializableDict<Region, Color> regionColors;

        [Space(2)]
        public bool autoUpdate;
        public void IncrementHeight(float incrementation)
        {
            if (heightMultiplier > 20)
                return;

            heightMultiplier += incrementation;
            GenerateMap();
        }
        public void GenerateMap()
        {
            MapDisplay display = FindObjectOfType<MapDisplay>();
            WFCGenerator wfcGen = FindObjectOfType<WFCGenerator>();


            float[,] noiseMap = Noise.GenerateNoise(mapWidthVertexCount, mapWidthVertexCount,seed,scale, octaves, persistence, lacunarity, offset);  //this counts vertex 
            Region[,] wfcMap = wfcGen.GenerateRegionGrid(); //this counts edges
            float[,] combinedMap = CombinedHeightMap(noiseMap, wfcGen.GenerateRegionHeightGrid(wfcMap));


            switch (drawMode)
            {
                case DrawMode.Noise:
                    display.DrawTexture(TextureGenerator.GenerateNoiseTexture(noiseMap));
                    break;
                case DrawMode.ColorMap:
                    display.DrawTexture(TextureGenerator.GenerateColorMap(noiseMap, regions));
                    break;
                case DrawMode.Mesh:
                    display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, heightCurve, levelOfSimplification, 1), TextureGenerator.GenerateNoiseTexture(noiseMap));
                    break;
                case DrawMode.ColoredMesh:
                    display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, heightCurve, levelOfSimplification, 1), TextureGenerator.GenerateColorMap(noiseMap, regions));
                    break;
                case DrawMode.WaveFunction:
                    display.DrawTexture(TextureGenerator.GenerateRegionTexture(wfcGen.GenerateRegionGrid(), regionColors));
                    break;
                case DrawMode.WaveFunctionMesh:
                    display.DrawMesh(MeshGenerator.GenerateTerrainMesh(wfcGen.GenerateRegionHeightGrid(wfcMap), heightMultiplier, heightCurve, levelOfSimplification, noiseMap.GetLength(0) / wfcMap.GetLength(0)), TextureGenerator.GenerateRegionTexture(wfcMap, regionColors));
                    break;
                case DrawMode.Combined:
                    display.DrawMesh(MeshGenerator.GenerateTerrainMesh(combinedMap, heightMultiplier, heightCurve, levelOfSimplification, 1), TextureGenerator.GenerateColorMap(combinedMap, regions));
                    
                    break;

            }
        } 
        //TODO: this needs some refactoring too MESSY
        private float[,] CombinedHeightMap(float[,] noiseMap, float[,] regionHeightMap) 
        {
            //deneme için noisemap 241,241 e regionmap 24,24 boyutlarýný kullanýlacak region kenar bazlý noisemap köþe!!
            int width = noiseMap.GetLength(0);
            int regionWidth = regionHeightMap.GetLength(0);

            float[,] combinedMap = new float[width, width];
            float scale = (float)(width) / regionWidth;

            //stores the region index values to be used in interpolation calculation
            int regionX;
            int regionY;

            for (int y = 0; y < width; y++)
            for (int x = 0; x < width; x++)
                {
                    regionX = (int)(x / scale); 
                    regionY = (int)(y / scale);

                    //half of the value comes from x axis and half of it comes from y axis thats why multiply by 0.5f
                    if (x % scale < scale / 2) //if its on the left of the cell interpolate between the left cell and current cell !! check if its on the most left
                    {
                        
                        if (regionX <= 0)
                            combinedMap[x, y] += 0.5f * wfcToNoiseRatio * regionHeightMap[regionX, regionY];
                        else
                            combinedMap[x, y] += 0.5f * wfcToNoiseRatio * Mathf.Lerp(regionHeightMap[regionX - 1, regionY],
                                                                                    regionHeightMap[regionX, regionY],
                                                                                    (2 / ((float)scale - 1)) * (x % scale)); ////the formula is: t = 1 / (s-1) * x % (scale) + 0.5
                    }
                    else
                    {
                        if(regionX >= regionWidth - 1)
                            combinedMap[x, y] += 0.5f * wfcToNoiseRatio * regionHeightMap[regionX, regionY];
                        else
                            combinedMap[x, y] += 0.5f * wfcToNoiseRatio * Mathf.Lerp(regionHeightMap[regionX + 1, regionY],
                                                                                     regionHeightMap[regionX, regionY],
                                                                                     (-2 / ((float)scale - 1)) * (x % scale) + 2); //(-1 / ((float)scale - 1)) * (x % scale) + 1.5f
                    }

                    if(y % scale < scale / 2) //if its on the bottom of the cell the formula is:
                    {
                        if (regionY <= 0)
                            combinedMap[x, y] += 0.5f * wfcToNoiseRatio * regionHeightMap[regionX, regionY];
                        else
                            combinedMap[x, y] += 0.5f * wfcToNoiseRatio * Mathf.Lerp(regionHeightMap[regionX, regionY - 1],
                                                                                     regionHeightMap[regionX, regionY],
                                                                                     (2 / ((float)scale - 1)) * (y % scale));
                    }
                    else
                    {
                        if (regionY >= regionWidth - 1)
                            combinedMap[x, y] += 0.5f * wfcToNoiseRatio * regionHeightMap[regionX, regionY];
                        else
                            combinedMap[x, y] += 0.5f * wfcToNoiseRatio * Mathf.Lerp(regionHeightMap[regionX, regionY + 1],
                                                                                     regionHeightMap[regionX, regionY],
                                                                                     (-2 / ((float)scale - 1)) * (y % scale) + 2);
                    }
                    combinedMap[x, y] += noiseMap[x, y] * (1 - wfcToNoiseRatio);
                    // combinedMap[x, y] = noiseMap[x, y] * (1 - wfcToNoiseRatio) + regionHeightMap[x / scale, y / scale] * wfcToNoiseRatio * ((wfcEffectCurve.Evaluate((x % scale) / (float)scale) + wfcEffectCurve.Evaluate((y % scale) / (float)scale)));
                }

            return combinedMap;
        }

        private void OnValidate() {
            if(lacunarity < 1){
                lacunarity = 1;
            }
            if(octaves < 0){
                octaves = 0;
            }
        }
    }
    [System.Serializable]
    public struct TerrainType{  
        public string name;
        public float height;
        public Color color;
    }
}
