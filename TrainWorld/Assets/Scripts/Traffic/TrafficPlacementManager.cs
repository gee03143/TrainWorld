using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rails;
using System;

namespace TrainWorld.Traffic
{
    public class TrafficPlacementManager : MonoBehaviour, InputHandler
    {
        [SerializeField]
        private RailBlockManager railBlockManager;

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
                TrafficSignal newSignal = railAtCursor.AddTrafficSignal();
                if (newSignal != null)
                {
                    signals.Add((railAtCursor.Position, railAtCursor.Direction), newSignal);
                    if (railAtCursor.myRailblock == null)
                    {
                        Debug.Log("myrailblock is null");
                    }
                    railBlockManager.Split(railAtCursor.myRailblock, railAtCursor.Position, railAtCursor.Direction);
                }
            }
            railBlockManager.ShowRailBlockDisplay();
        }

        internal void RemoveTraffic(TrafficSignal trafficSignal)
        {
            Rail railAtPosition = PlacementManager.GetRailAt(trafficSignal.Position, trafficSignal.Direction);
            railAtPosition.RemoveTrafficSocket();
            RailBlock blockA = railAtPosition.myRailblock;
            RailBlock blockB = PlacementManager.GetRailAt(trafficSignal.Position, trafficSignal.Direction.Opposite()).myRailblock;
            railBlockManager.Unite(blockA, new List<RailBlock> { blockB });
            blockA.UpdateRailsBlockReference();
            signals.Remove((trafficSignal.Position, trafficSignal.Direction));
            railBlockManager.ShowRailBlockDisplay();
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
            railBlockManager.ShowRailBlockDisplay();
            Debug.Log("Traffic Placement Enter");
        }

        public void OnExit()
        {
            ClearTempSignal();
            railBlockManager.DisableRailBlockDisplay();
            Debug.Log("Traffic Placement Exit");
        }
    }
}
