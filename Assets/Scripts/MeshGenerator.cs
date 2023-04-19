using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osman
{
    public static class MeshGenerator
    {
        public static MeshData GenerateTerrainMesh(float[,] noiseMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfSimplification){
            
            int meshSimplificationIncrement = levelOfSimplification == 0 ? 1 : levelOfSimplification * 2;

            int width = noiseMap.GetLength(0);
            int length = noiseMap.GetLength(1);

            int widthVertexCount = (width - 1) / meshSimplificationIncrement + 1;
            int lengthVertexCount = (length - 1) / meshSimplificationIncrement + 1;

            float leftX = width / -2f;
            float topZ = length / 2f;

            
            /*
            
            genislik 9 olsun basitleştirme de 2 olsa . . 3 . . 6 . . 9  buradan 4 tane köşe olmuş oldu yani köşe sayısı (w - 1) / basitleştirmeKatsayısı 
            bu 4 köşe yeni genişliğim oldu o yüzden üçgen indekslerini 4 ile hesaplamam gerekiyor

            */
            

            int vertexIndex = 0;
            MeshData meshData = new MeshData(widthVertexCount, lengthVertexCount);

            for(int x = 0; x < widthVertexCount; x++){
                for(int y= 0; y < lengthVertexCount; y++){
                    meshData.AddVertex(new Vector3(leftX + x * meshSimplificationIncrement,heightCurve.Evaluate(noiseMap[x * meshSimplificationIncrement,y* meshSimplificationIncrement])* heightMultiplier,topZ - y * meshSimplificationIncrement));
                    meshData.AddUV(new Vector2(x * meshSimplificationIncrement / (float)width, y * meshSimplificationIncrement / (float)length));

                    if(x < widthVertexCount-1 && y < lengthVertexCount-1){
                        meshData.AddTriangle( vertexIndex + widthVertexCount , vertexIndex + widthVertexCount + 1,vertexIndex); 
                        meshData.AddTriangle( vertexIndex + 1, vertexIndex, vertexIndex + widthVertexCount + 1);
                    }
                    vertexIndex++;
                }
            }

            return meshData;
        }
    }

    public class MeshData{
        private Vector3[] vertices;
        private int[] triangleIndexes;
        private Vector2[] uvs;

        private int vertexIndex = 0;
        private int triangleIndex = 0;
        private int uvIndex = 0;
        public MeshData(int width, int height){
            vertices = new Vector3[width  * height];
            triangleIndexes = new int[(width - 1) * (height - 1) * 6];
            uvs = new Vector2[width * height];
            
        }

        public void AddTriangle(int a,int b,int c){
            triangleIndexes[triangleIndex++] = a;
            triangleIndexes[triangleIndex++] = b;
            triangleIndexes[triangleIndex++] = c;
        }
        public void AddUV(Vector2 uv){
            uvs[uvIndex++] = uv;
        }

        public void AddVertex(Vector3 vertex){
            vertices[vertexIndex++] = vertex;
        }

        public Mesh CreateMesh(){
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangleIndexes;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
