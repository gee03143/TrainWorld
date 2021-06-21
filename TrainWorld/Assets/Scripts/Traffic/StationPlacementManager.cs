using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rails;
using TrainWorld.AI;
using System.Linq;

namespace TrainWorld.Station
{
    public class StationPlacementManager : MonoBehaviour, InputHandler
    {
        [SerializeField]
        private UiTrain uiTrain;

        private TrainStation tempStation;

        public void OnMouseDown(Vector3 mousePosition)
        {
            PlaceStation(mousePosition);
        }

        private void PlaceStation(Vector3 mousePosition)
        {
            ClearTempStations();
            Rails.Rail railAtCursor = PlacementManager.GetRailViaMousePosition(mousePosition, true);

            if (railAtCursor != null && railAtCursor.IsTrafficSocketEmpty())
            {
                TrainStation newStation = railAtCursor.AddStation((PlacementManager.GetStations().Count).ToString());
                if (newStation != null)
                {
                    PlacementManager.GetStations().Add(newStation.StationName, newStation);
                    uiTrain.SetStationNameList(PlacementManager.GetStations().Keys.ToList());
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

            Rails.Rail railAtCursor = PlacementManager.GetRailViaMousePosition(mousePosition, true);

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

        public TrainStation GetStationOfName(string name)
        {
            return PlacementManager.GetStationOfName(name);
        }

        internal bool TryChangeName(string from, string to, TrainStation selectedStation)
        {
            if (PlacementManager.GetStations().ContainsKey(to) == false)
            {
                PlacementManager.GetStations().Remove(from);
                PlacementManager.GetStations().Add(to, selectedStation);
                uiTrain.SetStationNameList(PlacementManager.GetStations().Keys.ToList());
                return true;
            }
            else if (PlacementManager.GetStations().ContainsKey(from) == false)
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
            Debug.Log("Station Placement Enter");
        }

        public void OnExit()
        {
            ClearTempStations();
            Debug.Log("Station Placement Exit");
        }
    }
}
