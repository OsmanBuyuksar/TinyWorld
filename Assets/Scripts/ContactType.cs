using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osman
{
    public class ContactType: MonoBehaviour
    {
        public Region _moduleRegion;
        public float height;
        [SerializeField]
        public List<Region> _excludedRegions; // note to myself: don't allow duplicates
    }
}
