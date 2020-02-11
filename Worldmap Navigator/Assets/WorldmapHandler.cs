using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JB.WorldMap
{
    public class WorldmapHandler : MonoBehaviour
    {
        public WorldMapHandlerSettings settings;
        public WorldMapPrefabs prefabs;
        public WorldMapReferences references;

        private Map currentMap;

        private Vector2 lastMousePosition;

        // Zoom
        private Vector2 zoomOrigin = new Vector2();
        private float previousZoom = 1;
        private float currentZoom = 1;
        private float targetZoom = 1;
        private float zoomTimer = 0;

        private Transform currentLocationInfoTarget;

        void Start()
        {
            CheckSettings();
            Initialize();
            if (settings.startingMap)
                SetMap(settings.startingMap);
        }

        /// <summary>
        /// Apply settings and get references.
        /// </summary>
        private void Initialize()
        {
            // Add Trigger for dragging
            UnityEngine.EventSystems.EventTrigger.Entry entry = new UnityEngine.EventSystems.EventTrigger.Entry();
            entry.eventID = UnityEngine.EventSystems.EventTriggerType.Drag;
            entry.callback.AddListener((data) => { OnDrag(); });
            references.mapParent.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(entry);

            // Create Map Background
            references.mapBackground = new GameObject("Map Background").AddComponent<Image>();
            references.mapBackground.gameObject.transform.parent = references.mapParent;
            references.mapBackground.gameObject.transform.localPosition = Vector2.zero;

            // Create Location Marker parent
            references.locationMarkerParent = new GameObject("Location Markers").transform;
            references.locationMarkerParent.parent = references.mapParent;
            references.locationMarkerParent.localPosition = Vector2.zero;
        }

        /// <summary>
        /// Check if all settings are set correctly for the script to work.
        /// </summary>
        private void CheckSettings()
        {
            if (!references.mapParent)
            {
                Debug.LogError("'mapParent' must be defined in references!");
                return;
            }
        }

        void Update()
        {
            NavigationUpdate();
        }

        void NavigationUpdate()
        {
            lastMousePosition = Input.mousePosition;

            if (settings.navigation.allowZoom)
                GetZoomInput();

            UpdateZoom();

            // Move Location Info Pannel
            if(currentLocationInfoTarget)
                references.locationInfo.pannel.transform.position = currentLocationInfoTarget.position;
        }

        /// <summary>
        /// Get scroll input and clamp the target zoom
        /// </summary>
        private void GetZoomInput()
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                SetZoom(targetZoom + (targetZoom * 0.5f * Input.mouseScrollDelta.y));
            }
            targetZoom = Mathf.Clamp(targetZoom, settings.navigation.minZoom, settings.navigation.maxZoom);
        }

        /// <summary>
        /// Updated the maps scale and position to zoom in or out.
        /// Based on: https://forum.unity.com/threads/scale-around-point-similar-to-rotate-around.232768/
        /// </summary>
        private void UpdateZoom()
        {
            // Increase zoom timer used for smooth zoom
            if (settings.navigation.zoomDuration > 0 && zoomTimer < 1)
                zoomTimer += Time.deltaTime / settings.navigation.zoomDuration;
            // Change scale and position to zoom onto zoom origin
            Vector3 difference = references.mapParent.position - (Vector3)zoomOrigin;
            currentZoom = Mathf.SmoothStep(previousZoom, targetZoom, zoomTimer);
            float aspectRatio = currentZoom / references.mapParent.transform.localScale.x;
            Vector3 FP = (Vector3)zoomOrigin + difference * aspectRatio;
            references.mapParent.transform.localScale = new Vector3(currentZoom, currentZoom, 1);
            references.mapParent.transform.position = FP;
        }

        public void SetZoom(float newZoom)
        {
            zoomOrigin = Input.mousePosition;
            previousZoom = currentZoom;
            targetZoom = newZoom;
            zoomTimer = 0;
        }

        public void OnDrag()
        {
            if(settings.navigation.allowDrag)
                references.mapParent.position += Input.mousePosition - (Vector3)lastMousePosition;
        }

        public void ShowUI()
        {

        }

        public void HideUI()
        {

        }

        public void SetMap(Map newMap)
        {
            currentMap = newMap;
            UpdateMapUI();
        }

        /// <summary>
        /// Update UI to display currentMap.
        /// </summary>
        private void UpdateMapUI()
        {
            ClearMap();

            references.mapBackground.sprite = currentMap.background;
            references.mapBackground.SetNativeSize();

            foreach (Location location in currentMap.locations)
            {
                GameObject marker = new GameObject("Location: " + location.name);
                marker.transform.parent = references.locationMarkerParent;
                marker.transform.localPosition = location.position;
                marker.transform.localScale = new Vector3(settings.markerScale, settings.markerScale, 1);

                marker.AddComponent<Image>();
                if (location.customMarkerSprite)
                    marker.GetComponent<Image>().sprite = location.customMarkerSprite;
                else
                    marker.GetComponent<Image>().sprite = settings.mapMarkerSprite;


                // Add Hover Trigger
                UnityEngine.EventSystems.EventTrigger.Entry entry = new UnityEngine.EventSystems.EventTrigger.Entry();
                entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
                entry.callback.AddListener((data) => { ShowLocationInfo(location); });
                marker.AddComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(entry);
            }
        }

        /// <summary>
        /// Delete all mapmarkers and prepare for a new map to be displayed.
        /// </summary>
        private void ClearMap()
        {
            // Destroy all location markers
            for (int i = 0; i < references.locationMarkerParent.childCount; i++)
            {
                Destroy(references.locationMarkerParent.GetChild(i));
            }
        }

        private void ShowLocationInfo(Location location)
        {
            if(settings.locationInfoIsPopup)
            {
                references.locationInfo.pannel.SetActive(true);
                currentLocationInfoTarget = GameObject.Find("Location: " + location.name).transform;
            }
            // Set Info fields
            if (references.locationInfo.name)
                references.locationInfo.name.GetComponent<Text>().text = location.name;
            if (references.locationInfo.description)
                references.locationInfo.description.GetComponent<Text>().text = location.descripting;
        }

        public void HideLocationInfo()
        {
            if (!settings.locationInfoIsPopup)
                return;
            references.locationInfo.pannel.SetActive(false);
            currentLocationInfoTarget = null;
        }

        [System.Serializable]
        public class WorldMapHandlerSettings
        {
            [Header("Mode")]
            public bool EnableTraveling = true;
            public enum MapModess
            {
                OpenWorld,
                Network,
                Free
            }
            public MapModess mode;

            [Header("Start")]
            public bool openOnStart = true;
            public Map startingMap;

            [Header("Behaviour")]
            public bool openSubMapsAutomaticly = true;

            public NavigationSettings navigation;

            [Header("Graphics")]
            public Sprite mapMarkerSprite;
            public float markerScale = 0.3f;
            public bool locationInfoIsPopup = true;

            [System.Serializable]
            public class NavigationSettings
            {
                [Header("Movement")]
                public bool allowDrag = true;
                public bool enableBoundaries = true;

                [Header("Zoom")]
                public bool allowZoom = true;
                public float minZoom = 1;
                public float maxZoom = 10;
                public float zoomDuration = 1;
            }

        }

        [System.Serializable]
        public class WorldMapPrefabs
        {

        }

        [System.Serializable]
        public class WorldMapReferences
        {
            // Inspector:

            [Tooltip("Parent of map content. Is moved/scaled for map navigation.")]
            public Transform mapParent;

            [Tooltip("Pannel showing Location information.")]
            public LocationInfo locationInfo;
            [System.Serializable]
            public class LocationInfo
            {
                public GameObject pannel;
                public GameObject name;
                public GameObject description;
            }

            // Hidden:

            [HideInInspector()]
            public Image mapBackground;
            [HideInInspector()]
            public Transform locationMarkerParent;
        }
    }
}