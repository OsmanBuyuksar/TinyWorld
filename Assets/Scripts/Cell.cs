using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osman
{
    public class Cell
    {
        private List<Region> possibleStates { get;}

        public void AddState(Region region)
        {
            possibleStates.Add(region);
        }
    }
}
