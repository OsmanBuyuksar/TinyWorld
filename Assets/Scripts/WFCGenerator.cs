using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osman
{
    public class WFCGenerator : MonoBehaviour
    {
        [SerializeField]
        private int gridSizeX = 5;
        private int gridSizeY = 5;
        [SerializeField]
        private List<Region> regions;
        [SerializeField]
        private GameObject[] modules;
        [SerializeField]
        private float cellSize;

        private Cell[,] cellGrid;

        
        void Start()
        {
            
        }

        private void InitGrid()
        {
            cellGrid = new Cell[gridSizeX, gridSizeY];

            for(int x = 0; x < gridSizeX; x++)
            for(int y = 0; y < gridSizeY; y++)
                {
                    foreach(Region region in regions)
                    {
                        cellGrid[x, y].AddState(region);
                    }
                }
        }

        private void CollapseCell()
        {

        }

        private void CreateRegionGrid()
        {

        }
    }
}
