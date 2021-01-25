using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public static class DirectionHelper
    {
        public static Direction Prev(Direction direction)
        {
            return (Direction)(((uint)direction - 1 + (uint)Direction.DIRECTION_COUNT) % (uint)Direction.DIRECTION_COUNT);
        }
        public static Direction Next(Direction direction)
        {
            return (Direction)(((uint)direction + 1) % (uint)Direction.DIRECTION_COUNT);
        }

        public static Direction Prev90(Direction direction)
        {
            return (Direction)(((uint)direction - 2 + (uint)Direction.DIRECTION_COUNT) % (uint)Direction.DIRECTION_COUNT);
        }
        public static Direction Next90(Direction direction)
        {
            return (Direction)(((uint)direction + 2) % (uint)Direction.DIRECTION_COUNT);
        }

        public static Direction Opposite(Direction direction)
        {
            return (Direction)(((uint)direction + ((uint)Direction.DIRECTION_COUNT / 2)) % (uint)Direction.DIRECTION_COUNT);
        }

        public static Vector3 ToEuler(Direction direction)
        {
            return new Vector3(0, (uint)direction * 45, 0);
        }

        public static Vector3Int ToDirectionVector(Direction direction)
        {
            if (direction == Direction.W)
            {
                return new Vector3Int(1, 0, 0);
            }
            else if (direction == Direction.SW)
            {
                return new Vector3Int(1, 0, -1);
            }
            else if (direction == Direction.S)
            {
                return new Vector3Int(0, 0, -1);
            }
            else if (direction == Direction.SE)
            {
                return new Vector3Int(-1, 0, -1);
            }
            else if (direction == Direction.E)
            {
                return new Vector3Int(-1, 0, 0);
            }
            else if (direction == Direction.NE)
            {
                return new Vector3Int(-1, 0, 1);
            }
            else if (direction == Direction.N)
            {
                return new Vector3Int(0, 0, 1);
            }
            else if (direction == Direction.NW)
            {
                return new Vector3Int(1, 0, 1);
            }
            else
            {
                Debug.Log("Invalid direction Data converted");
                return Vector3Int.zero;
            }
        }
    }
}
