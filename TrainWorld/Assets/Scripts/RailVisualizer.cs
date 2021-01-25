using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class RailVisualizer : MonoBehaviour
    {
        [SerializeField]
        private GameObject railPrefab;

        internal void Visualize(Vector3Int placementStartPosition, Vector3Int placementEndPosition, Direction placementStartDirection, Direction placementEndDirection)
        {
            GameObject newObject = Instantiate(railPrefab) as GameObject;
            Rail rail = newObject.GetComponent<Rail>();

            rail.Visualize(placementStartPosition, placementEndPosition, placementStartDirection, placementEndDirection);

        }

        internal void VisualizeDeadEnd(Vector3Int placementStartPosition, Direction placementStartDirection)
        {
            throw new NotImplementedException();
        }
    }
}
