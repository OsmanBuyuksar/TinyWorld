using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace osman
{
    public class WFCGenerator : MonoBehaviour
    {
        [SerializeField]
        private int gridSizeX = 5;
        [SerializeField]
        private int gridSizeY = 5;
        [SerializeField]
        private List<Region> allRegions = new List<Region>();
        [SerializeField]
        private ContactType[] allContactTypes;
        [SerializeField]
        private float cellSize = 1;

        private Cell[,] cellMatrix;

        private void InitializeGrid()
        {
            cellMatrix = new Cell[gridSizeX,gridSizeY];

            for(int x = 0; x < gridSizeX; x++)
            for(int y = 0; y < gridSizeY; y++)
                {
                    cellMatrix[x, y] = new Cell(x, y, allRegions);
                }
        }

        private List<Cell> neighbourCells(Cell cell)
        {
            int x = cell._x;
            int y = cell._y;

            List<Cell> neighbours = new List<Cell>();

            if(y != 0) neighbours.Add(cellMatrix[x, y - 1]);
            if (y != cellMatrix.GetLength(1) - 1) neighbours.Add(cellMatrix[x, y + 1]);

            if (x != cellMatrix.GetLength(0) - 1) neighbours.Add(cellMatrix[x + 1, y]);
            if (x != 0) neighbours.Add(cellMatrix[x - 1, y]);

            return neighbours;
        }

        /// <summary>
        /// Returns the regions that can't be neighbours with the given param region
        /// </summary>
        /// <param name="region"></param>
        /// <returns>Unsuitable regions</returns>
        private List<Region> ReturnUncompatibleRegions(Region region)
        {
            return allContactTypes.First(contact => contact._moduleRegion == region)._excludedRegions;
        }

        private float ReturnRegionHeight(Region region)
        {
            float f = allContactTypes.First(contact => contact._moduleRegion == region).height;
            return f;
        }
        /// <summary>
        /// Adds to the exclusion list of the collapsing cell if the given cell has uncompatible regions
        /// </summary>
        /// <param name="nbrCell"></param>
        /// <param name="exclusionList"></param>
        private void TryUpdateCellStates(Cell currentCell, Cell nbrCell)
        {
            HashSet<Region> possibleExclusions = new HashSet<Region>();

            foreach (Region region in allRegions)
                if(nbrCell._possibleStates.All(r => ReturnUncompatibleRegions(r).Contains(region)))
                    possibleExclusions.Add(region);

            currentCell.RemoveStates(possibleExclusions);
        }

        //yar�m fps yolda :D
        private void CollapseCell(Cell cellToCollapse)
        {
            //Check neighbours and update possible states
            List<Cell> neighbours = neighbourCells(cellToCollapse); //neighbours

            foreach (Cell nbrCell in neighbours)
                TryUpdateCellStates(cellToCollapse, nbrCell);

            //then give random state from the possible states
            cellToCollapse.PickRandomState();

            //after that update others accordingly
            foreach(Cell nbrCell in neighbours)
                foreach (Cell neighbourOfNbrCell in neighbourCells(nbrCell))
                    TryUpdateCellStates(nbrCell, neighbourOfNbrCell);
        }

        private Cell LeastEntropyCell(Cell[] unfinishedCells)
        {
            var cellsWithSuperposition = unfinishedCells.Where(cell => cell.StateCount() > 1).ToArray();

            if (cellsWithSuperposition.Length <= 0)
                return null;

            int minStateCount = cellsWithSuperposition.Min(cell => cell.StateCount());

            return cellsWithSuperposition.First(cell => cell.StateCount() == minStateCount);
        }


        /// <summary>
        /// Generate a region map using WFC alghoritm
        /// </summary>
        private void GenerateMap()
        {
            InitializeGrid();
            Cell[] unfinishedCells = cellMatrix.Cast<Cell>().ToArray();
            Cell currentCell;

            int i = 0;
            while(unfinishedCells.Length > 1)
            {
                currentCell = LeastEntropyCell(unfinishedCells);

                if (currentCell == null) 
                    break;

                CollapseCell(currentCell);
                i++;
            }
        }

        public Region[,] GenerateRegionGrid()
        {
            GenerateMap();

            Region[,] regionGrid = new Region[cellMatrix.GetLength(0), cellMatrix.GetLength(1)];

            for (int i = 0; i < cellMatrix.GetLength(0); i++)
                for (int j = 0; j < cellMatrix.GetLength(1); j++)
                    regionGrid[i, j] = cellMatrix[i, j]._possibleStates.First();

            return regionGrid;
        }

        public float[,] GenerateRegionHeightGrid(Region[,] regionGrid)
        {
            float[,] regionHeightGrid = new float[regionGrid.GetLength(0), regionGrid.GetLength(1)];

            for (int i = 0; i < regionGrid.GetLength(0); i++)
                for (int j = 0; j < regionGrid.GetLength(1); j++){
                    regionHeightGrid[i, j] = ReturnRegionHeight(regionGrid[i, j]);
                }
                    

            return regionHeightGrid;
        } 
    }
}
