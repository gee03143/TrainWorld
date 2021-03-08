using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TrainWorld
{
    public class RailGraph
    {
        private Dictionary<Vertex, List<Vertex>> graph;

        public RailGraph()
        {
            graph = new Dictionary<Vertex, List<Vertex>>();
        }

        internal Vertex GetVertexAt(Vector3 position, Direction direction)
        {
            return graph.Keys.FirstOrDefault(x => CompareVerticies(x, position, direction));
        }

        internal List<Vertex> GetNeighboursAt(Vector3 position, Direction direction)
        {
            Vertex targetVertex = GetVertexAt(position, direction);
            if (targetVertex == null)
            {
                return null;
            }
            else
            {
                return graph[targetVertex];
            }
        }

        private bool CompareVerticies(Vertex x, Vector3 position, Direction direction)
        {
            return Vector3.SqrMagnitude(position - x.Position) < 0.0001f && direction == x.direction;
        }

        private bool CompareVerticies(Vector3 position1, Direction direction1, Vector3 position2, Direction direction2)
        {
            return Vector3.SqrMagnitude(position2 - position1) < 0.0001f && direction1 == direction2;
        }

        public void AddVertexAtPosition(Vector3Int position, Direction direction)
        {
            AddVertex(new Vertex(position, direction));
            AddVertex(new Vertex(position, DirectionHelper.Opposite(direction)));

            //DrawGraph();
        }

        public void AddVertex(Vertex v)
        {
            if (graph.ContainsKey(v))
                return;

            graph.Add(v, new List<Vertex>());

            //check adjascent
            Vector3 adjascentPosition = v.Position + DirectionHelper.ToDirectionalVector(v.direction);
            if (GetVertexAt(adjascentPosition, v.direction) != null)
            {
                // if has adjascent vertex
                AddEdge(v.Position, adjascentPosition, v.direction, v.direction);
            }
             adjascentPosition = v.Position - DirectionHelper.ToDirectionalVector(v.direction);
            if (GetVertexAt(adjascentPosition, v.direction) != null)
            {
                // if has adjascent vertex
                AddEdge(v.Position, adjascentPosition, v.direction, v.direction);
            }
        }

        public void RemoveVertex(Vertex v)
        {
            if (graph.ContainsKey(v) == false)
                return;

            foreach (var neighbour in graph[v])
            {
                graph[neighbour].Remove(v);
            }

            graph.Remove(v);
        }

        public void AddEdge(Vector3 position1, Vector3 position2, Direction direction1, Direction direction2)
        {
            if(CompareVerticies(position1, direction1, position2, direction2))
            {
                return; //don't add edges between the same vertex
            }

            var v1 = GetVertexAt(position1, direction1);
            var v2 = GetVertexAt(position2, direction2);
            if (v1 == null)
            {
                v1 = new Vertex(position1, direction1);
            }
            if(v2 == null)
            {
                v2 = new Vertex(position2, direction2);
            }
            AddEdgeBetween(v1, v2);
            AddEdgeBetween(v2, v1); //  this graph is non-directional
        }

        private void AddEdgeBetween(Vertex v1, Vertex v2)
        {
            if (v1 == v2)
                return;

            if (graph.ContainsKey(v1))
            {
                if (graph[v1].FirstOrDefault(x => x == v2) == null)
                {
                    graph[v1].Add(v2);
                }
            }
            else
            {
                AddVertex(v1);
                graph[v1].Add(v2);
            }
        }

        private void DrawGraph()
        {
            foreach(Vertex v in graph.Keys)
            {
                foreach(Vertex neighbours in graph[v])
                {
                    Debug.DrawLine(v.Position, neighbours.Position, Color.red, 10.0f);
                }
            }
        }
    }
}