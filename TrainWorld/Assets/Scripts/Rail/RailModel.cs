using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Station;

namespace TrainWorld.Rail
{
    public class RailModel : MonoBehaviour
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
        private List<Vertex> neighbours;

        [SerializeField]
        private TrainStation stationSocket;

        public void FixModel(List<Vertex> neighbours)
        {
            DestroyAllChild();

            this.neighbours.Clear();
            this.neighbours.AddRange(neighbours);

            Vector3Int frontCandidatePos = position + DirectionHelper.ToDirectionalVector(direction);
            Vector3Int leftCandidatePos = frontCandidatePos + DirectionHelper.ToDirectionalVector(DirectionHelper.Prev(direction));
            Vector3Int rightCandidatePos = frontCandidatePos + DirectionHelper.ToDirectionalVector(DirectionHelper.Next(direction));

            bool railCreated = false;
            foreach (Vertex neighbour in neighbours)
            {
                if (Vector3Int.RoundToInt(neighbour.Position).Equals(leftCandidatePos))
                {
                    railCreated = true;
                    if (DirectionHelper.IsDiagonal(direction)) // if direction is diagonal
                    {
                        continue;   // do nothing
                    }
                    else
                    {
                        RailFactory.SpawnRail(RailType.Corner_Left, position, direction, transform);
                    }
                }
                else if (Vector3Int.RoundToInt(neighbour.Position).Equals(rightCandidatePos))
                {
                    railCreated = true;
                    if (DirectionHelper.IsDiagonal(direction)) // if direction is diagonal
                    {
                        continue;   // do nothing
                    }
                    else
                    {
                        RailFactory.SpawnRail(RailType.Corner_Right, position, direction, transform);
                    }
                }
                else if (Vector3Int.RoundToInt(neighbour.Position).Equals(frontCandidatePos))
                {
                    railCreated = true;
                    if (DirectionHelper.IsDiagonal(direction))// if direction is diagonal
                    {
                        // make diagonal rail
                        RailFactory.SpawnRail(RailType.Diagonal, position, direction, transform);
                    }
                    else
                    {
                        // make straight rail
                        RailFactory.SpawnRail(RailType.Straight, position, direction, transform);
                    }
                }
            }

            if (railCreated == false)
            {
                if (DirectionHelper.IsDiagonal(direction))
                {
                    RailFactory.SpawnRail(RailType.Diagonal_Deadend, position, direction, transform);
                }
                else
                {
                    RailFactory.SpawnRail(RailType.Straight_Deadend, position, direction, transform);
                }
            }
        }

        internal TrainStation AddStation(GameObject stationPrefab)
        {
            if (this.stationSocket == null)
            {
                GameObject obj = Instantiate(stationPrefab, position, Quaternion.Euler(DirectionHelper.ToEuler(direction))) as GameObject;
                TrainStation station = obj.GetComponent<TrainStation>();
                this.stationSocket = station;
                station.Position = this.Position;
                station.Direction = this.Direction;
                return station;
            }
            return null;
        }

        internal void Init(Vector3Int position, Direction8way direction)
        {
            this.position = position;
            this.direction = direction;
            this.neighbours = new List<Vertex>();
        }

        private void DestroyAllChild()
        {
            //clear my children
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void DestroyMyself()
        {
            if(stationSocket != null)
                stationSocket.DestroyMyself();
            DestroyAllChild();
            Destroy(gameObject);
        }

        public override string ToString()
        {
            return position.ToString() + " " + direction.ToString();
        }

        void OnDrawGizmosSelected()
        {
            foreach (var neighbour in neighbours)
            {
                if (this.Direction == Direction8way.N || this.Direction == Direction8way.NW ||
                    this.Direction == Direction8way.NE || this.Direction == Direction8way.E)
                    Debug.DrawLine(neighbour.Position, this.Position, Color.red);
                else
                    Debug.DrawLine(neighbour.Position, this.Position, Color.blue);
            }
        }
    }
}
