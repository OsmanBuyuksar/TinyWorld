using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osman
{
    public class MapGenerator : MonoBehaviour
    { 
        
        [SerializeField] private DrawMode drawMode = DrawMode.Noise;
        [SerializeField] TerrainType[] regions;

        //Mesh Parameters
        //
        [Space(2)]
        [SerializeField] private AnimationCurve heightCurve;
        [SerializeField] private float heightMultiplier = 1f;
        [Range(0, 6)]
        public int levelOfSimplification;

        //Noise Parameters
        //
        [Space(2)]
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
        //SerializableDict

        [Space(2)]
        public bool autoUpdate;
        public void GenerateMap()
        {
            float[,] noiseMap = Noise.GenerateNoise(mapChunkSize, mapChunkSize,seed,scale, octaves, persistence, lacunarity, offset);
            
            MapDisplay display = FindObjectOfType<MapDisplay>();
            WFCGenerator wfcGen = FindObjectOfType<WFCGenerator>();

            switch (drawMode)
            {
                case DrawMode.Noise:
                    display.DrawTexture(TextureGenerator.GenerateNoiseTexture(noiseMap));
                    break;
                case DrawMode.ColorMap:
                    display.DrawTexture(TextureGenerator.GenerateColorMap(noiseMap, regions));
                    break;
                case DrawMode.Mesh:
                    display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, heightCurve, levelOfSimplification), TextureGenerator.GenerateColorMap(noiseMap, regions));
                    break;
                case DrawMode.WaveFunction:
                    display.DrawTexture(TextureGenerator.GenerateRegionTexture(wfcGen.GenerateRegionGrid(), regionColors));
                    break;
            }
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
