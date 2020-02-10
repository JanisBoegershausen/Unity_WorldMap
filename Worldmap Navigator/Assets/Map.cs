using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JB.WorldMap
{
    [CreateAssetMenu(fileName = "New Map", menuName = "JB/Worldmap/Map")]
    public class Map : ScriptableObject
    {
        public Sprite background;

        public List<Location> locations = new List<Location>();
    }
}