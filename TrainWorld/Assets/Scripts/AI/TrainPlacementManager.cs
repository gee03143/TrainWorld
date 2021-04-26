using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rails;

namespace TrainWorld.AI
{
    public class TrainPlacementManager : MonoBehaviour, InputHandler
    {
        [SerializeField]
        private GameObject trainPrefab;

        private List<AiAgent> trains;

        private Rails.Rail railAtCursor;

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
            List<Rails.Rail> rails = PlacementManager.GetRailsAtPosition(Vector3Int.RoundToInt(mousePosition));
            if (rails.Count != 2)
            {
                return;
            }
            else
            {
                railAtCursor = PlacementManager.GetRailViaMousePosition(mousePosition);

                GameObject newObject = Instantiate(trainPrefab, railAtCursor.Position,
                    Quaternion.Euler(DirectionHelper.ToEuler(railAtCursor.Direction)));

                AiAgent newAgent = newObject.GetComponent<AiAgent>();
                newAgent.Init(railAtCursor.Position, railAtCursor.Direction);
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
