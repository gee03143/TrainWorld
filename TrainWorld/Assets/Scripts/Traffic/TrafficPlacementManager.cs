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
        RailBlockManager railBlockManager;

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
                    RailBlock railBlockA;
                    RailBlock railBlockB;
                    (railBlockA, railBlockB) = railAtCursor.myRailblock.Divide((railAtCursor.Position, railAtCursor.Direction));

                    if (railBlockA == null && railBlockB == null) // fail to divide railblock
                    {
                        //do nothing
                    }
                    else
                    {
                        railBlockManager.railBlocks.Remove(railAtCursor.myRailblock);
                        railBlockManager.railBlocks.Add(railBlockA);
                        railBlockManager.railBlocks.Add(railBlockB);

                        railAtCursor.myRailblock.RemoveMyself();
                        railBlockA.UpdateRailsBlockReference();
                        railBlockB.UpdateRailsBlockReference();
                    }
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
