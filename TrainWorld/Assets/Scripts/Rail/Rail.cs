using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Station;
using System;
using System.Linq;

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

        private HashSet<(Vector3Int, Direction8way)> neighbourPositions;

        [SerializeField]
        private RailModel railModel;

        [SerializeField]
        private Transform positiveStationTransform;
        [SerializeField]
        private Transform negativeStationTransform;

        [SerializeField]
        private TrainStation positiveStation;
        [SerializeField]
        private TrainStation negativeStation;

        internal void Init(Vector3Int position, Direction8way direction)
        {
            this.position = position;
            this.direction = direction;
            neighbourPositions = new HashSet<(Vector3Int, Direction8way)>();

            railModel.Init(position, direction);
        }

        public (Vector3Int, Direction8way) GetPosAndDirection()
        {
            return (Position, Direction);
        }

        public List<(Vector3Int, Direction8way)> GetNeighbourTuples()
        {
            return neighbourPositions.ToList();
        }

        public void RemoveNeighbourAt((Vector3Int, Direction8way) pos)
        {
            neighbourPositions.Remove(pos);
        }

        public void AddNeighbour((Vector3Int, Direction8way) neighbourPosition)
        {
            if(neighbourPositions.Contains(neighbourPosition) == false)
            {
                neighbourPositions.Add(neighbourPosition);
            }            
        }

        public void AddNeighbours(List<(Vector3Int, Direction8way)> neighbourPositions)
        {
            neighbourPositions.Union(neighbourPositions);
        }

        public RailModel GetModel(bool opposite)
        {
            return railModel;
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

                    station.Direction = (Direction8way)this.Direction + 4;

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

        public void FixMyModel()
        {
            railModel.FixModel(neighbourPositions.ToList());
        }

        public void DestroyMyself()
        {
            neighbourPositions.Clear();
            Destroy(gameObject);
        }

        void OnDrawGizmosSelected()
        {
            Debug.Log(neighbourPositions.Count);
            foreach (var neighbour in neighbourPositions)
            {
                if (this.Direction == Direction8way.N || this.Direction == Direction8way.NW ||
                    this.Direction == Direction8way.NE || this.Direction == Direction8way.E)
                    Debug.DrawLine(neighbour.Item1, this.Position, Color.red);
                else
                    Debug.DrawLine(neighbour.Item1, this.Position, Color.blue);
            }
        }
    }
}
