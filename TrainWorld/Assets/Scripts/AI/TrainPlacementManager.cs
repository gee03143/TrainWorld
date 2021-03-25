using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rail;

namespace TrainWorld.AI
{
    public class TrainPlacementManager : MonoBehaviour, InputHandler
    {
        [SerializeField]
        private RailPlacementManager railPlacementManager;

        [SerializeField]
        private GameObject trainPrefab;

        Rail.Rail railAtCursor;


        internal void PlaceTrain(Vector3 mousePosition)
        {
            List<Rail.Rail> rails = railPlacementManager.GetRailsAtPosition(Vector3Int.RoundToInt(mousePosition));
            if (rails.Count != 2)
            {
                return;
            }
            else
            {
                railAtCursor = railPlacementManager.GetRailViaMousePosition(mousePosition);

                GameObject newObject = Instantiate(trainPrefab, railAtCursor.Position,
                    Quaternion.Euler(DirectionHelper.ToEuler(railAtCursor.Direction)));

                AiAgent newAgent = newObject.GetComponent<AiAgent>();
                newAgent.Init(railAtCursor.Position, railAtCursor.Direction, railPlacementManager);

            }
        }

        public void OnEnter()
        {
            Debug.Log("Train Placement Enter");
        }

        public void OnExit()
        {
            Debug.Log("Train Placement Exit");
        }
    }
}
