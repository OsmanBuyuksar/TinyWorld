using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osman
{
    public class ContactType: MonoBehaviour
    {
        public Region _moduleRegion;
        [SerializeField]
        public List<Region> _excludedRegions; // note to myself: don't allow duplicates
    }
}
