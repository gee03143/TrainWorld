﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Traffic;

namespace TrainWorld.Rails
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
        private List<(Vector3Int, Direction8way)> neighbours;

        public void FixModel(List<(Vector3Int, Direction8way)> neighbours)
        {
            DestroyAllChild();

            this.neighbours.Clear();
            this.neighbours.AddRange(neighbours);

            Vector3Int frontCandidatePos = position + direction.ToDirectionalVector();
            Vector3Int leftCandidatePos = frontCandidatePos + direction.Prev().ToDirectionalVector();
            Vector3Int rightCandidatePos = frontCandidatePos + direction.Next().ToDirectionalVector();

            bool railCreated = false;
            foreach ((Vector3Int, Direction8way) neighbour in neighbours)
            {
                if (neighbour.Item1.Equals(leftCandidatePos))
                {
                    railCreated = true;
                    if (direction.IsDiagonal()) // if direction is diagonal
                    {
                        continue;   // do nothing
                    }
                    else
                    {
                        RailFactory.SpawnRail(RailType.Corner_Left, position, direction, transform);
                    }
                }
                else if (neighbour.Item1.Equals(rightCandidatePos))
                {
                    railCreated = true;
                    if (direction.IsDiagonal()) // if direction is diagonal
                    {
                        continue;   // do nothing
                    }
                    else
                    {
                        RailFactory.SpawnRail(RailType.Corner_Right, position, direction, transform);
                    }
                }
                else if (neighbour.Item1.Equals(frontCandidatePos))
                {
                    railCreated = true;
                    if (direction.IsDiagonal())// if direction is diagonal
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
                if (direction.IsDiagonal())
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
            this.neighbours = new List<(Vector3Int, Direction8way)>();
        }

        private void DestroyAllChild()
        {
            //clear my children
            foreach (Transform child in transform)
            {
                if (child.gameObject.GetComponent<RailTypeHolder>() != null)
                    GameObject.Destroy(child.gameObject);
            }
        }

        public void DestroyMyself()
        {
            DestroyAllChild();
            Destroy(gameObject);
        }
    }
}
