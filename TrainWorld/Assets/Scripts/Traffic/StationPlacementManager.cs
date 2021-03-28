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
        private PlacementManager placementManager;
        [SerializeField]
        private UiTrain uiTrain;

        private Dictionary<String, TrainStation> stations;

        private TrainStation tempStation;

        void Awake()
        {
            stations = new Dictionary<string, TrainStation>();
        }

        public void OnMouseDown(Vector3 mousePosition)
        {
            PlaceStation(mousePosition);
        }

        private void PlaceStation(Vector3 mousePosition)
        {
            ClearTempStations();
            Rail.Rail railAtCursor = placementManager.GetRailViaMousePosition(mousePosition, true);

            if (railAtCursor != null && railAtCursor.IsTrafficSocketEmpty())
            {
                TrainStation newStation = railAtCursor.AddStation((stations.Count).ToString());
                if (newStation != null)
                {
                    stations.Add(newStation.StationName, newStation);
                    uiTrain.SetStationNameList(stations.Keys.ToList());
                }
            }
        }

        public void OnMouseMove(Vector3 mousePosition)
        {
            MoveCursor(mousePosition);
        }

        public void MoveCursor(Vector3 mousePosition)
        {
            ClearTempStations();

            Rail.Rail railAtCursor = placementManager.GetRailViaMousePosition(mousePosition, true);

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

        public void OnRInput()
        {
            //throw new NotImplementedException();
        }

        public void SetDestinationToAgent(List<string> stationNames, AiAgent target)
        {
            List<TrainStation> stations = new List<TrainStation>();
            foreach (string name in stationNames)
            {
                TrainStation station = GetStationOfName(name);
                if (station != null)
                    stations.Add(station);
                else
                {
                    Debug.Log("station with this name is null" + name);
                    return;
                }
            }

            if (target == null)
            {
                Debug.Log("Target Agent is null");
                return;
            }

            target.SetUpSchedule(stations);

        }

        private TrainStation GetStationOfName(string name)
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
                uiTrain.SetStationNameList(stations.Keys.ToList());
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
