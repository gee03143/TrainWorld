using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rail;
using TrainWorld.AI;

namespace TrainWorld.Station
{
    public class StationPlacementManager : MonoBehaviour, InputHandler
    {
        [SerializeField]
        private RailPlacementManager railPlacementManager;

        [SerializeField]
        private GameObject stationPrefab;

        [SerializeField]
        private GameObject trainPrefab;

        [SerializeField]
        List<TrainStation> stations;

        Rail.Rail railAtCursor;
        Transform placementTargetPos;

        void Awake()
        {
            stations = new List<TrainStation>();
        }

        internal void PlaceStation(Vector3Int position)
        {
            if(railAtCursor != null && placementTargetPos != null)
            {
                TrainStation newStation = railAtCursor.AddStation(stationPrefab, placementTargetPos);
                if(newStation != null)
                    stations.Add(newStation);
            }
        }

        internal void MoveCursor(Vector3 mousePosition)
        {
            List<Rail.Rail> rails = railPlacementManager.GetRailsAtPosition(Vector3Int.RoundToInt(mousePosition));
            if (rails.Count != 1)
            {
                //do nothing
                railAtCursor = null;
                placementTargetPos = null;
            }
            else
            {
                railAtCursor = rails[0];
                placementTargetPos = railAtCursor.GetClosestTrainSocket(mousePosition);
            }
        }

        internal void RotateCursor()
        {
            if(stations.Count >= 2)
            {
                TrainStation depart = stations[0];
                TrainStation arrival = stations[1];
                Debug.Log(depart.ToString() + arrival.ToString());
                List<Vertex> path = RailGraphPathfinder.AStarSearch(depart.Position, depart.Direction, arrival.Position, true, railPlacementManager.railGraph);
                GameObject train = Instantiate(trainPrefab, depart.Position, Quaternion.Euler(DirectionHelper.ToEuler(depart.Direction))) as GameObject;
                AiAgent ai = train.GetComponent<AiAgent>();
                ai.Initialize(path, false);
            }
        }

        public void OnEnter()
        {
            Debug.Log("Station Placement Exit");
        }

        public void OnExit()
        {
            Debug.Log("Station Placement Exit");
        }
    }
}
