using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public enum Direction
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW,
        DIRECTION_COUNT // 8 way direction ststem
    }

    public static class DirectionHelper
    {
        public static Direction Next(Direction current)
        {
            return (Direction)(((int)current + 1) % (int)Direction.DIRECTION_COUNT);
        }

        public static Direction Prev(Direction current)
        {
            return (Direction)(((int)current - 1 + (int)Direction.DIRECTION_COUNT) % (int)Direction.DIRECTION_COUNT);
        }

        public static Direction Opposite(Direction current)
        {
            return (Direction)(((int)current + (int)Direction.DIRECTION_COUNT / 2) % (int)Direction.DIRECTION_COUNT);
        }

        public static bool IsDiagonal(Direction direction)
        {
            return direction == Direction.NW || direction == Direction.NE || direction == Direction.SE || direction == Direction.SW;
        }

        public static Vector3Int ToDirectionalVector(Direction direction)
        {
            if(direction == Direction.N)
            {
                return new Vector3Int(0, 0, 1);
            }
            else if(direction == Direction.NE)
            {
                return new Vector3Int(1, 0, 1);
            }
            else if (direction == Direction.E)
            {
                return new Vector3Int(1, 0, 0);
            }
            else if (direction == Direction.SE)
            {
                return new Vector3Int(1, 0, -1);
            }
            else if (direction == Direction.S)
            {
                return new Vector3Int(0, 0, -1);
            }
            else if (direction == Direction.SW)
            {
                return new Vector3Int(-1, 0, -1);
            }
            else if (direction == Direction.W)
            {
                return new Vector3Int(-1, 0, 0);
            }
            else // NW
            {
                return new Vector3Int(-1, 0, 1);
            }
        }

        public static Vector3 ToEuler(Direction direction)
        {
            return new Vector3(0, 45 * (int)direction, 0);
        }
    }
}