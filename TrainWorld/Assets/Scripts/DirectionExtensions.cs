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
        DIRECTION_COUNT // 8 way direction system
    }

    public static class DirectionExtensions
    {
        public static Direction8way Next(this Direction8way current)
        {
            return (Direction8way)(((int)current + 1) % (int)Direction8way.DIRECTION_COUNT);
        }

        public static Direction8way Prev(this Direction8way current)
        {
            return (Direction8way)(((int)current - 1 + (int)Direction8way.DIRECTION_COUNT) % (int)Direction8way.DIRECTION_COUNT);
        }

        public static Direction8way Opposite(this Direction8way current)
        {
            return (Direction8way)(((int)current + 4) % (int)Direction8way.DIRECTION_COUNT);
        }

        public static bool IsDiagonal(this Direction8way direction)
        {
            return direction == Direction8way.NE || direction == Direction8way.SE || direction == Direction8way.SW || direction == Direction8way.NW;
        }

        public static Vector3Int ToDirectionalVector(this Direction8way direction)
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

        public static Vector3 ToEuler(this Direction8way direction)
        {
            return new Vector3(0, 45 * (int)direction, 0);
        }

        public static Direction8way ToDirection4Way(this Direction8way direction)
        {
            if((int)direction > 3)
            {
                direction = direction - 4;
            }
            return direction;
        }
    }
}