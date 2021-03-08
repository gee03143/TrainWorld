using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class RailGizmo : MonoBehaviour
    {
        List<Vertex> neighbours;

        private void Awake()
        {
            neighbours = new List<Vertex>();
        }

        public void SetNeighbours(List<Vertex> value)
        {
            neighbours = value;
        }

        private void OnDrawGizmosSelected()
        {
            foreach (Vertex n in neighbours)
            {
                Debug.DrawLine(transform.position, n.Position, Color.red);
            }
        }
    }
}