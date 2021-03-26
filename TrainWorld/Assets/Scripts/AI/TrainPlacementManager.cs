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

        private List<AiAgent> trains;

        private Rail.Rail railAtCursor;

        private void Awake()
        {
            trains = new List<AiAgent>();
        }

        public void OnMouseDown(Vector3 mousePosition)
        {
            PlaceTrain(mousePosition);
        }

        private void PlaceTrain(Vector3 mousePosition)
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
                trains.Add(newAgent);
            }
        }

        public void RemoveAgent(AiAgent removeTarget)
        {
            trains.Remove(removeTarget);
            Destroy(removeTarget.gameObject);
        }

        public void OnEnter()
        {
            Debug.Log("Train Placement Enter");
        }

        public void OnExit()
        {
            Debug.Log("Train Placement Exit");
        }

        public void OnMouseMove(Vector3 mousePosition)
        {
            //throw new NotImplementedException();
        }

        public void OnRInput()
        {
           // throw new NotImplementedException();
        }
    }
}
