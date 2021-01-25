using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class RailNode
    {
        public Vector3Int position;

        public Dictionary<Direction, RailNode> connectedRails;

        public RailNode(Vector3Int position, Direction direction)
        {
            this.position = position;
            connectedRails = new Dictionary<Direction, RailNode>();
            this.connectedRails.Add(direction, this);
            this.connectedRails.Add(DirectionHelper.Opposite(direction), this);
        }

        public Direction GetClosestExtendDirection(Vector3 mousePosition)
        {
            double minDist = double.PositiveInfinity;
            Direction returnValue = Direction.N;

            foreach (var direction in connectedRails.Keys)
            {
                double distance = Vector3.Distance(mousePosition, position + DirectionHelper.ToDirectionVector(direction));
                if (minDist > distance)
                {
                    minDist = distance;
                    returnValue = direction;
                }
            }

            return returnValue;
        }
    }
}