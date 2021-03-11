using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class Vertex : IEquatable<Vertex>
    {
        public Vector3 Position { get; set; }
        public Direction8way Direction { get; set; }

        public Vertex(Vector3 position, Direction8way direction)
        {
            this.Position = position;
            this.Direction = direction;
        }

        public bool Equals(Vertex other)
        {
            return Vector3.SqrMagnitude(Position - other.Position) < 0.001f && Direction == other.Direction;
        }

        public override string ToString()
        {
            return Position.ToString() + "  " + Direction.ToString();
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Direction.GetHashCode();
        }
    }
}