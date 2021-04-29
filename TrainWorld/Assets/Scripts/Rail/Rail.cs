using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

using TrainWorld.Station;
using TrainWorld.Traffic;

namespace TrainWorld.Rails
{
    [RequireComponent(typeof(Rigidbody))]
    public class Rail : MonoBehaviour, ISelectableObject
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
            private set { 
                direction = value; 
            }
        }

        private HashSet<(Vector3Int, Direction8way)> neighbourPositions;

        [SerializeField]
        private RailModel railModel;

        [SerializeField]
        public Transform trafficSocketTransform;

        [SerializeField]
        private TrafficSocket trafficSocket;

        [SerializeField]
        public Renderer railGroupDisplay;

        public RailBlock myRailblock;

        internal void Init(Vector3Int position, Direction8way direction)
        {
            this.position = position;
            this.direction = direction;
            this.myRailblock = null;
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

        public bool HasTraffic()
        {
            return trafficSocket.Type == TrafficSocketType.Traffic;
        }

        public bool IsTrafficSocketEmpty()
        {
            return trafficSocket.Type == TrafficSocketType.Empty;
        }

        internal TrainStation AddStation(string name)
        {
            trafficSocket.Type = TrafficSocketType.Station;
            TrainStation station = trafficSocket.GetStation();
            station.Init(this.Position, this.Direction, name);
            return trafficSocket.GetStation();
        }

        public TrainStation AddTempStation()
        {
            trafficSocket.SetMyGameObject(TrafficSocketType.Station);
            return trafficSocket.GetStation();
        }

        internal TrafficSignal AddTrafficSignal()
        {
            trafficSocket.Type = TrafficSocketType.Traffic;
            TrafficSignal traffic = trafficSocket.GetTraffic();
            traffic.Init(this.Position, this.Direction);

            return trafficSocket.GetTraffic();
        }

        public TrafficSignal AddTempTrafficSignal()
        {
            trafficSocket.SetMyGameObject(TrafficSocketType.Traffic);
            return trafficSocket.GetTraffic();
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
            foreach (var neighbour in neighbourPositions)
            {
                Debug.DrawLine(neighbour.Item1, this.Position, Color.red);
            }
        }

        public SelectableObjectType GetSelectableObjectType()
        {
            return SelectableObjectType.Rail;
        }

        public override string ToString()
        {
            return position.ToString() + " " + direction.ToString();
        }

        public void ShowMyUI()
        {
            Debug.Log(this.ToString());
        }
    }
}
