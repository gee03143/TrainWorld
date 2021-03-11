using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TrainWorld.Rail
{
    public class RailGraph
    {
        private Dictionary<Vertex, List<Vertex>> graph;

        public RailGraph()
        {
            graph = new Dictionary<Vertex, List<Vertex>>();
        }

        internal Vertex GetVertexAt(Vector3 position, Direction8way direction)
        {
            return graph.Keys.FirstOrDefault(x => CompareVerticies(x, position, direction));
        }

        internal List<Vertex> GetNeighboursAt(Vector3 position, Direction8way direction)
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

        private bool CompareVerticies(Vertex x, Vector3 position, Direction8way direction)
        {
            return Vector3.SqrMagnitude(position - x.Position) < 0.0001f && direction == x.Direction;
        }

        private bool CompareVerticies(Vector3 position1, Direction8way direction1, Vector3 position2, Direction8way direction2)
        {
            return Vector3.SqrMagnitude(position2 - position1) < 0.0001f && direction1 == direction2;
        }

        public void AddVertexAtPosition(Vector3Int position, Direction8way direction)
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
            Vector3 adjascentPosition = v.Position + DirectionHelper.ToDirectionalVector(v.Direction);
            if (GetVertexAt(adjascentPosition, v.Direction) != null)
            {
                // if has adjascent vertex
                AddEdge(v.Position, adjascentPosition, v.Direction, v.Direction);
            }
             adjascentPosition = v.Position - DirectionHelper.ToDirectionalVector(v.Direction);
            if (GetVertexAt(adjascentPosition, v.Direction) != null)
            {
                // if has adjascent vertex
                AddEdge(v.Position, adjascentPosition, v.Direction, v.Direction);
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

        public void AddEdge(Vector3 position1, Vector3 position2, Direction8way direction1, Direction8way direction2)
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

        internal void DeleteVertexAtPosition(Vector3Int position, Direction8way direction)
        {
            DeleteVertex(new Vertex(position, direction));
            DeleteVertex(new Vertex(position, DirectionHelper.Opposite(direction)));
        }

        private void DeleteVertex(Vertex v)
        {
            if (graph.ContainsKey(v) == false) // if there's no such vertex
            {
                return; //  do nothing
            }

            foreach(var neighbour in graph[v])
            {
                graph[neighbour].Remove(v);
            }
            graph.Remove(v);
        }
    }
}