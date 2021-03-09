using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
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

        public void FixModel(List<Vertex> neighbours)
        {
            DestroyAllChild();

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
                        RailFactory.SpawnRail(RailType.Straight, position, direction, transform);
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

        internal void Init(Vector3Int position, Direction8way direction)
        {
            this.position = position;
            this.direction = direction;
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
            Destroy(gameObject);
        }
    }
}
