using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class Vertex : IEquatable<Vertex>
    {
        public Vector3 Position { get; set; }
        public Direction8way direction { get; set; }

        public Vertex(Vector3 position, Direction8way direction)
        {
            this.Position = position;
            this.direction = direction;
        }

        public bool Equals(Vertex other)
        {
            return Vector3.SqrMagnitude(Position - other.Position) < 0.001f && direction == other.direction;
        }

        public override string ToString()
        {
            return Position.ToString() + "  " + direction.ToString();
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ direction.GetHashCode();
        }
    }
}