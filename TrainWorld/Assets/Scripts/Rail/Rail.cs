using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Station;

namespace TrainWorld.Rail
{
    public class Rail : MonoBehaviour
    {
        [SerializeField]
        private Vector3Int position;

        public Vector3Int Position
        {
            get { return position; }
            private set { position = value; }
        }

        [SerializeField]
        private Direction8way direction;

        public Direction8way Direction
        {
            get { return direction; }
            private set { direction = value; }
        }

        [SerializeField]
        private List<RailModel> models;

        [SerializeField]
        private List<Transform> stationTransforms;

        private Dictionary<Transform, TrainStation> stations;

        internal void Init(Vector3Int position, Direction8way direction)
        {
            this.position = position;
            this.direction = direction;

            models[0].Init(position, direction);
            models[1].Init(position, DirectionHelper.Opposite(direction));

            this.stations = new Dictionary<Transform, TrainStation>();
            foreach (var transform in stationTransforms)
            {
                stations.Add(transform, null);
            }
        }

        public RailModel GetModel(bool opposite)
        {
            if (opposite)
                return models[1];
            else
                return models[0];
        }

        public Transform GetClosestTrainSocket(Vector3 position)
        {
            float nearestDistance = float.PositiveInfinity;
            Transform nearestTransform = null;

            foreach (Transform currentTransform in stations.Keys)
            {
                Debug.Log(currentTransform);
                float currentDistance = Vector3.Distance(currentTransform.position, position);
                if (currentDistance < nearestDistance)
                {
                    if (stations[currentTransform] != null)
                    {
                        nearestDistance = currentDistance;
                        nearestTransform = currentTransform;
                    }
                }
            }
            return nearestTransform;
        }

        internal TrainStation AddStation(GameObject stationPrefab, Transform placementPosition)
        {
            /*
            if (this.stationSocket == null)
            {
                GameObject obj = Instantiate(stationPrefab, placementPosition.position,
                    Quaternion.Euler(DirectionHelper.ToEuler(direction))) as GameObject;
                TrainStation station = obj.GetComponent<TrainStation>();
                this.stationSocket = station;
                station.Position = this.Position;
                station.Direction = this.Direction;
                stations[placementPosition] = station;
                return station;
            }
            */
            return null;
        }
    }
}
