using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace osman
{
    public class MapGenerator : MonoBehaviour
    { 
        
        [SerializeField] private DrawMode drawMode = DrawMode.Noise;
        [SerializeField] TerrainType[] regions;

        //Mesh Parameters
        //
        [Space(2)]
        [Tooltip("Mesh Parameters")]
        [SerializeField] private AnimationCurve heightCurve;
        [SerializeField] private float heightMultiplier = 1f;
        [Range(0, 6)]
        public int levelOfSimplification;

        //Noise Parameters
        //
        [Space(2)]
        [Tooltip("Noise Parameters")]
        public const int mapChunkSize = 241;
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
        [SerializeField]
        SerializableDict<Region, Color> regionColors;

        [Space(2)]
        public bool autoUpdate;
        public void GenerateMap()
        {
            MapDisplay display = FindObjectOfType<MapDisplay>();
            WFCGenerator wfcGen = FindObjectOfType<WFCGenerator>();


            float[,] noiseMap = Noise.GenerateNoise(mapChunkSize, mapChunkSize,seed,scale, octaves, persistence, lacunarity, offset);
            Region[,] wfcMap = wfcGen.GenerateRegionGrid();
            
            switch (drawMode)
            {
                case DrawMode.Noise:
                    display.DrawTexture(TextureGenerator.GenerateNoiseTexture(noiseMap));
                    break;
                case DrawMode.ColorMap:
                    display.DrawTexture(TextureGenerator.GenerateColorMap(noiseMap, regions));
                    break;
                case DrawMode.Mesh:
                    display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, heightCurve, levelOfSimplification, 1), TextureGenerator.GenerateColorMap(noiseMap, regions));
                    break;
                case DrawMode.WaveFunction:
                    display.DrawTexture(TextureGenerator.GenerateRegionTexture(wfcGen.GenerateRegionGrid(), regionColors));
                    break;
                case DrawMode.WaveFunctionMesh:
                    display.DrawMesh(MeshGenerator.GenerateTerrainMesh(wfcGen.GenerateRegionHeightGrid(wfcMap), heightMultiplier, heightCurve, levelOfSimplification, noiseMap.GetLength(0) / wfcMap.GetLength(0)), TextureGenerator.GenerateRegionTexture(wfcMap, regionColors));
                    //display.UpdateMeshScale(noiseMap.GetLength(0) / wfcMap.GetLength(0));
                    break;
                case DrawMode.Combined:
                    //display.DrawMesh(MeshGenerator.GenerateTerrainMesh(wfcGen.GenerateRegionHeightGrid(wfcMap), heightMultiplier, heightCurve, levelOfSimplification), TextureGenerator.GenerateRegionTexture(wfcMap, regionColors));
                    
                    break;

            }
        } 
        //private void Combined

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
