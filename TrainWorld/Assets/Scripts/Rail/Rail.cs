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
        private Direction4way direction;

        public Direction4way Direction
        {
            get { return direction; }
            private set { direction = value; }
        }

        [SerializeField]
        private List<RailModel> models;

        [SerializeField]
        private Transform positiveStationTransform;
        [SerializeField]
        private Transform negativeStationTransform;

        [SerializeField]
        private TrainStation positiveStation;
        [SerializeField]
        private TrainStation negativeStation;

        internal void Init(Vector3Int position, Direction4way direction)
        {
            this.position = position;
            this.direction = direction;

            models[0].Init(position, (Direction8way)direction);
            models[1].Init(position, (Direction8way)(direction + 4));
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
            float positiveDistance = Vector3.Distance(position, positiveStationTransform.position);
            float negativeDistance = Vector3.Distance(position, negativeStationTransform.position);
            if (positiveDistance < negativeDistance)
            {
                if (positiveStation == null)
                {
                    return positiveStationTransform;
                }
                else if(negativeStation == null)
                {
                    return negativeStationTransform;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (negativeStation == null)
                {
                    return negativeStationTransform;
                }
                else if (positiveStation == null)
                {
                    return positiveStationTransform;
                }
                else
                {
                    return null;
                }
            }

        }

        internal TrainStation AddStation(GameObject stationPrefab, Transform placementPosition)
        {
            if(positiveStationTransform == placementPosition)
            {
                if(positiveStation == null)
                {
                    GameObject obj = Instantiate(stationPrefab, placementPosition.position,
                    Quaternion.identity) as GameObject;
                    TrainStation station = obj.GetComponent<TrainStation>();
                    positiveStation = station;
                    station.Position = Vector3Int.RoundToInt(this.position);

                    station.Direction = (Direction8way)this.Direction;

                    return station;
                }
                else
                {
                    return null;
                }
            }else if(negativeStationTransform == placementPosition)
            {
                if(negativeStation == null)
                {
                    GameObject obj = Instantiate(stationPrefab, placementPosition.position,
                    Quaternion.identity) as GameObject;
                    TrainStation station = obj.GetComponent<TrainStation>();
                    negativeStation = station;
                    station.Position = Vector3Int.RoundToInt(this.position);

                    station.Direction = (Direction8way)this.Direction;

                    return station;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void DestroyMyself()
        {
            Destroy(gameObject);
        }
    }
}
