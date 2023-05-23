using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace osman
{
    public class Cell
    {
        //cell position
        public int _x { get; private set; }
        public int _y { get; private set; }

        //current possible states
        public HashSet<Region> _possibleStates { get; private set; } = new HashSet<Region>();
        //public HashSet<Region> _excludedStates = new HashSet<Region>();

        bool isFinal = false;


        public Cell(int x, int y, List<Region> allRegions)
        {
            _x = x;
            _y = y;
            foreach(Region r in allRegions)
                _possibleStates.Add(r);
        }
        public void RemoveStates(HashSet<Region> regionsToRemove)
        {
            foreach (Region r in regionsToRemove)
                if(regionsToRemove.Contains(r)) _possibleStates.Remove(r);
        }

        public void PickRandomState()
        {
            Region r = _possibleStates.ToArray()[Random.Range(0, _possibleStates.Count)];

            _possibleStates.Clear();
            _possibleStates.Add(r);

            isFinal = true;
        }

        public int StateCount()
        {
            return _possibleStates.Count;
        }

        ///<summary>
        ///Sets the position of the cell
        ///</summary>
        public void SetPos(int x, int y)
        {
            _x = x;
            _y = y;
        }


    }
}
