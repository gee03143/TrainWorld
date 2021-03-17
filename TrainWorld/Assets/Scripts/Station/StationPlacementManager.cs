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
        Direction8way placementStartDirection = Direction8way.N;

        [SerializeField]
        private RailPlacementManager railPlacementManager;

        [SerializeField]
        private RailModelManager railModelManager;

        [SerializeField]
        private GameObject stationPrefab;

        [SerializeField]
        private GameObject trainPrefab;

        List<TrainStation> stations;

        RailModel railModelAtCursor;

        void Awake()
        {
            stations = new List<TrainStation>();
        }

        internal void PlaceStation(Vector3Int position)
        {
            TrainStation newStation = railModelAtCursor.AddStation(stationPrefab);

            //이미 station이 존재하는 위치일 경우 false 반환
            if (newStation != null)
                stations.Add(newStation);
        }

        internal void MoveCursor(Vector3 position)
        {
            railModelAtCursor = railModelManager.GetRailModelViaMousePosition(position);
        }

        internal void RotateCursor()
        {
            placementStartDirection = DirectionHelper.Next(placementStartDirection);

            if(stations.Count >= 2)
            {
                TrainStation depart = stations[0];
                TrainStation arrival = stations[1];
                List<Vertex> path = RailGraphPathfinder.AStarSearch(depart.Position, depart.Direction, arrival.Position, true, railPlacementManager.railGraph);
                GameObject train = Instantiate(trainPrefab, depart.Position, Quaternion.Euler(DirectionHelper.ToEuler(depart.Direction))) as GameObject;
                AiAgent ai = train.GetComponent<AiAgent>();
                ai.Initialize(path, false);
                
                if (path.Count > 0)
                {
                    foreach (Vertex vertex in path)
                    {
                        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //cube.transform.position = vertex.Position;
                    }
                }
                else
                {
                    Debug.Log("Non Reachable Position");
                }

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
