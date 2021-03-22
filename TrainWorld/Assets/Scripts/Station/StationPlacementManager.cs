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

        private Dictionary<String, TrainStation> stations;

        Rail.Rail railAtCursor;
        Transform placementTargetPos;

        void Awake()
        {
            stations = new Dictionary<string, TrainStation>();
        }

        internal void PlaceStation(Vector3 mousePosition)
        {
            List<Rail.Rail> rails = railPlacementManager.GetRailsAtPosition(Vector3Int.RoundToInt(mousePosition));
            if (rails.Count != 1)
            {
                //cannot place train on multiple rail 
                //do nothing
                return;
            }
            else
            {
                railAtCursor = rails[0];
                placementTargetPos = railAtCursor.GetClosestTrainSocket(mousePosition);
                TrainStation newStation = railAtCursor.AddStation(stationPrefab, placementTargetPos);
                if (newStation != null)
                {
                    newStation.StationName = (stations.Count).ToString();
                    Debug.Log(newStation.StationName);
                    stations.Add(newStation.StationName, newStation);
                }
            }
        }

        internal bool TryChangeName(string from, string to, TrainStation selectedStation)
        {
            if (stations.ContainsKey(to) == false)
            {
                stations.Remove(from);
                stations.Add(to, selectedStation);
                return true;
            }
            else if (stations.ContainsKey(from) == false)
            {
                Debug.Log("No station with name : " + from + " Failure at change name");
                return false;
            }
            else
            {
                Debug.Log("station name : " + to + " is already taken");
                return false;
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
