using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public enum Direction8way
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
        public static Direction8way Next(Direction8way current)
        {
            return (Direction8way)(((int)current + 1) % (int)Direction8way.DIRECTION_COUNT);
        }

        public static Direction8way Prev(Direction8way current)
        {
            return (Direction8way)(((int)current - 1 + (int)Direction8way.DIRECTION_COUNT) % (int)Direction8way.DIRECTION_COUNT);
        }

        public static Direction8way Opposite(Direction8way current)
        {
            return (Direction8way)(((int)current + 4) % (int)Direction8way.DIRECTION_COUNT);
        }

        public static bool IsDiagonal(Direction8way direction)
        {
            return direction == Direction8way.NE || direction == Direction8way.SE || direction == Direction8way.SW || direction == Direction8way.NW;
        }

        public static Vector3Int ToDirectionalVector(Direction8way direction)
        {
            if(direction == Direction8way.N)
            {
                return new Vector3Int(0, 0, 1);
            }
            else if(direction == Direction8way.NE)
            {
                return new Vector3Int(1, 0, 1);
            }
            else if (direction == Direction8way.E)
            {
                return new Vector3Int(1, 0, 0);
            }
            else if (direction == Direction8way.SE)
            {
                return new Vector3Int(1, 0, -1);
            }else if (direction == Direction8way.S)
            {
                return new Vector3Int(0, 0, -1);
            }
            else if (direction == Direction8way.SW)
            {
                return new Vector3Int(-1, 0, -1);
            }
            else if (direction == Direction8way.W)
            {
                return new Vector3Int(-1, 0, 0);
            }
            else if (direction == Direction8way.NW)
            {
                return new Vector3Int(-1, 0, 1);
            }
            else
            {
                throw new Exception();
            }
        }

        public static Vector3 ToEuler(Direction8way direction)
        {
            return new Vector3(0, 45 * (int)direction, 0);
        }
    }
}