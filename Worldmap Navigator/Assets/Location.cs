using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JB.WorldMap
{
    [CreateAssetMenu(fileName = "New Location", menuName = "JB/Worldmap/Location")]
    public class Location : ScriptableObject
    {
        [Header("Information")]
        public new string name;
        public string type;
        [Space(5)]
        public string descripting;

        [Space(10)]
        public Vector2 position;

        [Header("Graphics")]
        public Sprite customMarkerSprite;

        [Header("Advanced")]
        public Map subMap;
    }
}