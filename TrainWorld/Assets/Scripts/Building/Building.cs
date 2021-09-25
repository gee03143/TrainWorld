using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using TrainWorld.Traffic;
using System;

namespace TrainWorld.Buildings
{
    public class Building : MonoBehaviour, ISelectableObject
    {
        public Storage storage;

        [SerializeField]
        private Vector3Int position;

        private TrainStation connectedStation;

        public TrainStation ConnectedStation
        {
            get { return connectedStation; }
            set { connectedStation = value; }
        }


        public Vector3Int Position
        {
            get { return position; }
            private set { position = value; }
        }

        internal void SetConnectedBuilding(string stationName)
        {
            if (stationName == "empty")
            {
                connectedStation = null;
            }
            else
            {
                connectedStation =
                    PlacementManager.GetStationOfName(stationName);
                connectedStation.AddConnectedBuilding(storage);
            }
        }

        [SerializeField]
        private int possibleStationSearchDistance;


        [SerializeField]
        private int sizeX;

        [SerializeField]
        private int sizeZ;

        BuildingColorChanger bcc;

        internal void Init(Vector3Int position)
        {
            this.position = position;
            bcc = GetComponent<BuildingColorChanger>();
            storage = GetComponent<Storage>();

            List<TrainStation> adjascentStations = GetAllPossibleStations();
            if(adjascentStations.Count != 0)
            {
                connectedStation = adjascentStations.First();
                connectedStation.AddConnectedBuilding(storage);
            }
        }

        public Vector3Int GetMinPosition()
        {
            Vector3Int minPos = new Vector3Int(position.x - sizeX / 2, 0, position.z - sizeZ / 2);
            if (sizeX % 2 == 0)
                minPos.x--;
            if (sizeZ % 2 == 0)
                minPos.z--;

            return minPos;
        }

        public Vector3Int GetMaxPosition()
        {
            Vector3Int maxPos = new Vector3Int(position.x + sizeX / 2, 0, position.z + sizeZ / 2);
            return maxPos;
        }

        public List<TrainStation> GetAllPossibleStations()
        {
            List<TrainStation> trainStations = new List<TrainStation>();

            List<TrainStation> allStations = PlacementManager.GetStations().Values.ToList();

            foreach (var station in allStations)
            {
                if(MathFunctions.ManhattanDiscance(station.Position, position) <= possibleStationSearchDistance)
                {
                    trainStations.Add(station);
                }
            }

            return trainStations;
        }
        public SelectableObjectType GetSelectableObjectType()
        {
            return SelectableObjectType.Building;
        }

        public void ShowMyUI()
        {
            if(connectedStation != null)
                Debug.Log("Connected Station : " + connectedStation.StationName);
            else
            {
                Debug.Log("No Connected Station");
            }
        }

        internal void DestroyMyself()
        {
            connectedStation.RemoveConnectedBuilding(storage);
            Destroy(gameObject);
        }
    }
}
