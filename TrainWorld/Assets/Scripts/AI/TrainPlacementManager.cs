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
        private RailArrowUI railArrowUI;
        [SerializeField]
        private GameObject trainPrefab;

        private List<AiAgent> trains;

        private Rail railUnderCursor;

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
                railUnderCursor = PlacementManager.GetRailViaMousePosition(mousePosition);

                GameObject newObject = Instantiate(trainPrefab, railUnderCursor.Position,
                    Quaternion.Euler(railUnderCursor.Direction.ToEuler()));

                AiAgent newAgent = newObject.GetComponent<AiAgent>();
                newAgent.Init(railUnderCursor.Position, railUnderCursor.Direction);
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
            railArrowUI.HideRailArrowUI();
            Debug.Log("Train Placement Exit");
        }

        public void OnMouseMove(Vector3 mousePosition)
        {
            MoveCursorAtTrainPlacement(mousePosition);
        }

        internal void MoveCursorAtTrainPlacement(Vector3 cursorPosition)
        {
            railUnderCursor = PlacementManager.GetRailViaMousePosition(cursorPosition);
            railArrowUI.ShowRailUI(railUnderCursor);
        }

        public void OnRInput()
        {
           // throw new NotImplementedException();
        }
    }
}
