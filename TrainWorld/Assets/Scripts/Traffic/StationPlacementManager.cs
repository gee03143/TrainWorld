using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rail;
using TrainWorld.AI;
using System.Linq;

namespace TrainWorld.Station
{
    public class StationPlacementManager : MonoBehaviour, InputHandler
    {
        [SerializeField]
        private RailPlacementManager railPlacementManager;
        [SerializeField]
        private UiTrain uiTrain;

        private Dictionary<String, TrainStation> stations;

        private TrainStation tempStation;

        void Awake()
        {
            stations = new Dictionary<string, TrainStation>();
        }

        internal void PlaceStation(Vector3 mousePosition)
        {
            ClearTempStations();
            Rail.Rail railAtCursor = railPlacementManager.GetRailViaMousePosition(mousePosition, true);

            if (railAtCursor != null && railAtCursor.IsTrafficSocketEmpty())
            {
                TrainStation newStation = railAtCursor.AddStation((stations.Count).ToString());
                if (newStation != null)
                {
                    stations.Add(newStation.StationName, newStation);
                    uiTrain.SetUpDropdown(stations.Keys.ToList());
                }
            }
        }

        public void MoveCursor(Vector3 mousePosition)
        {
            ClearTempStations();

            Rail.Rail railAtCursor = railPlacementManager.GetRailViaMousePosition(mousePosition, true);

            if(railAtCursor != null && railAtCursor.IsTrafficSocketEmpty())
            {
                tempStation = railAtCursor.AddTempStation();
            }
        }

        private void ClearTempStations()
        {
            tempStation?.gameObject.SetActive(false);
            tempStation = null;
        }

        internal TrainStation GetStationOfName(string name)
        {
            if (stations.ContainsKey(name))
            {
                return stations[name];
            }
            else
                return null;
        }

        internal bool TryChangeName(string from, string to, TrainStation selectedStation)
        {
            if (stations.ContainsKey(to) == false)
            {
                stations.Remove(from);
                stations.Add(to, selectedStation);
                uiTrain.SetUpDropdown(stations.Keys.ToList());
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
