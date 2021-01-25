using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class PlacementManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject railPrefab;

        Dictionary<Vector3Int, Type> placementDictionary;

        public bool placementMode { get; private set; }

        private void Start()
        {
            placementDictionary = new Dictionary<Vector3Int, Type>();
        }

        public Type? GetPlacementDataAt(Vector3Int position)
        {
            if(placementDictionary.ContainsKey(position) == false)
                return null;

            return placementDictionary[position];
        }

        internal void AddTempObjectToDictionary(Vector3Int position, Type data)
        {
            if (placementDictionary.ContainsKey(position) == false)
            {
                placementDictionary.Add(position, data);
            }

        }
    }
}
