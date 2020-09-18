using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JB.WorldMap
{
    /// <summary>
    /// Class containing data and methods connected to the player position, actions, etc.
    /// </summary>
    public class WorldMapPlayerController : MonoBehaviour
    {
        public Location currentLocation;

        public PlayerEvents events;
        [System.Serializable]
        public class PlayerEvents
        {
            public UnityEngine.Events.UnityEvent OnLocationArive;
        }

        private void Update()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartTravel(Location location)
        {
            float distance = Vector2.Distance(currentLocation.position, location.position);

            // Wait for seconds and update stamina based on distance

            currentLocation = location;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Teleport(Location location)
        {
            transform.position = location.position;
        }
    }
}