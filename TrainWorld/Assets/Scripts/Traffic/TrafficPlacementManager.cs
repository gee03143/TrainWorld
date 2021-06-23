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

        }

        internal void RemoveTraffic(TrafficSignal trafficSignal)
        {
            RailBlock blockA = PlacementManager.GetRailAt(trafficSignal.Position, trafficSignal.Direction).myRailblock;
            RailBlock blockB = PlacementManager.GetRailAt(trafficSignal.Position, trafficSignal.Direction.Opposite()).myRailblock;
            railBlockManager.Unite(blockA, new List<RailBlock> { blockB });
            blockA.UpdateRailsBlockReference();
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
