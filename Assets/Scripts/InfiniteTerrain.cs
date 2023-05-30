using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osman
{
    public class InfiniteTerrain : MonoBehaviour
    {
        [SerializeField] Transform terrainParent;
        [SerializeField] Transform playerTransform;
        [SerializeField] float viewDistance = 100f;

        private Vector2 currentInstancePos = new Vector2();
        private Vector2 currentCoord = new Vector2();
        private int visibleTerrainCoord;
        private int chunkSize;
        private Dictionary<Vector2, GameObject> terrains = new Dictionary<Vector2, GameObject>();

        private List<GameObject> lastSeenTerrains = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            chunkSize = (MapGenerator.mapWidthVertexCount - 1);
            visibleTerrainCoord = Mathf.RoundToInt(viewDistance / chunkSize);
        }

        // Update is called once per frame
        void Update()
        {
            foreach(GameObject terrain in lastSeenTerrains){
                terrain.SetActive(false);
                
            }
            lastSeenTerrains.Clear();

            currentCoord.x = Mathf.RoundToInt(playerTransform.position.x / chunkSize);
            currentCoord.y = Mathf.RoundToInt(playerTransform.position.z / chunkSize);

            
            for(int mapChunkX = -visibleTerrainCoord; mapChunkX < visibleTerrainCoord; mapChunkX++){
                for(int mapChunkY = -visibleTerrainCoord; mapChunkY < visibleTerrainCoord; mapChunkY++){
                    currentInstancePos.x = currentCoord.x + mapChunkX;
                    currentInstancePos.y = currentCoord.y + mapChunkY;

                    if(terrains.ContainsKey(currentInstancePos)){
                        terrains[currentInstancePos].SetActive(true);
                        lastSeenTerrains.Add(terrains[currentInstancePos]);
                    }else{
                        Vector3 terrainPos = new Vector3(currentInstancePos.x, 0, currentInstancePos.y);
                        terrainPos *= chunkSize;
                        
                        GameObject terrain = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        terrain.transform.parent = terrainParent;
                        terrain.transform.position = terrainPos; 
                        terrain.transform.localScale = Vector3.one / 10 * chunkSize;
                        terrains.Add(new Vector2(currentInstancePos.x, currentInstancePos.y), terrain);
                        lastSeenTerrains.Add(terrain);
                    }
                }
            }
        }
    }
}
