using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rails;
using System;

namespace TrainWorld.Traffic
{
    public class TrafficPlacementManager : MonoBehaviour, InputHandler
    {

        private Dictionary<(Vector3Int, Direction8way), TrafficSignal> signals;

        private TrafficSignal tempSignal;

        private void Awake()
        {
            signals = new Dictionary<(Vector3Int, Direction8way), TrafficSignal>();
        }

        public void OnMouseDown(Vector3 mousePosition)
        {
            PlaceTraffic(mousePosition);
        }

        private void PlaceTraffic(Vector3 mousePosition)
        {
            ClearTempSignal();
            Rails.Rail railAtCursor = PlacementManager.GetRailViaMousePosition(mousePosition, true);

            if (railAtCursor != null && railAtCursor.IsTrafficSocketEmpty())
            {
                TrafficSignal newStation = railAtCursor.AddTrafficSignal();
                if (newStation != null)
                {
                    signals.Add((railAtCursor.Position, railAtCursor.Direction), newStation);
                }
            }

        }

        public void OnMouseMove(Vector3 mousePosition)
        {
            MoveCursor(mousePosition);
        }

        private void MoveCursor(Vector3 mousePosition)
        {
            ClearTempSignal();

            Rails.Rail railAtCursor = PlacementManager.GetRailViaMousePosition(mousePosition, true);

            if (railAtCursor != null && railAtCursor.IsTrafficSocketEmpty())
            {
                tempSignal = railAtCursor.AddTempTrafficSignal();
            }
        }

        private void ClearTempSignal()
        {
            tempSignal?.gameObject.SetActive(false);
            tempSignal = null;
        }

        public void OnRInput()
        {
            throw new System.NotImplementedException();
        }

        public void OnEnter()
        {
            Debug.Log("Traffic Placement Enter");
        }

        public void OnExit()
        {
            ClearTempSignal();
            Debug.Log("Traffic Placement Exit");
        }
    }
}
