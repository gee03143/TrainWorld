using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Source https://github.com/lordjesus/Packt-Introduction-to-graph-algorithms-for-game-developers
/// </summary>

namespace TrainWorld
{
    public class RailGraphPathfinder
    {
        internal List<Vertex> AStarSearch(Vector3Int placementStartPosition, Direction8way placementStartDirection, Vector3Int endPosition)
        {
            List<Vertex> path = new List<Vertex>();

            List<Vertex> positionsTocheck = new List<Vertex>();
            Dictionary<Vertex, float> costDictionary = new Dictionary<Vertex, float>();
            Dictionary<Vertex, float> priorityDictionary = new Dictionary<Vertex, float>();
            Dictionary<Vertex, Vertex> parentsDictionary = new Dictionary<Vertex, Vertex>();

            positionsTocheck.Add(new Vertex(placementStartPosition, placementStartDirection));
            priorityDictionary.Add(new Vertex(placementStartPosition, placementStartDirection), 0);
            costDictionary.Add(new Vertex(placementStartPosition, placementStartDirection), 0);
            parentsDictionary.Add(new Vertex(placementStartPosition, placementStartDirection), null);

            while (positionsTocheck.Count > 0)
            {
                Vertex current = GetClosestVertex(positionsTocheck, priorityDictionary);
                positionsTocheck.Remove(current);
                if (current.Position.Equals(endPosition))
                {
                    path = GeneratePath(parentsDictionary, current);
                    return path;
                }

                if(ManhattanDiscance(Vector3Int.RoundToInt(current.Position), placementStartPosition) > 5)
                {
                    path = GeneratePath(parentsDictionary, current);
                    return path;
                }

                foreach (Vertex neighbour in GetAdjacentCells(current))
                {
                    float newCost = costDictionary[current] + GetCostOfEnteringCell(neighbour, current);
                    if (!costDictionary.ContainsKey(neighbour) || newCost < costDictionary[neighbour])
                    {
                        costDictionary[neighbour] = newCost;

                        float priority = newCost + ManhattanDiscance(endPosition, Vector3Int.RoundToInt(neighbour.Position));
                        positionsTocheck.Add(neighbour);
                        priorityDictionary[neighbour] = priority;

                        parentsDictionary[neighbour] = current;
                    }
                }
            }
            return path;
        }

        private float ManhattanDiscance(Vector3Int endPosition, Vector3Int startPosition)
        {
            return Mathf.Abs(endPosition.x - startPosition.x) + Mathf.Abs(endPosition.z - startPosition.z);
        }

        private float GetCostOfEnteringCell(Vertex neighbour, Vertex current)
        {
            return ManhattanDiscance(Vector3Int.RoundToInt(neighbour.Position), Vector3Int.RoundToInt(current.Position));
        }

        private List<Vertex> GetAdjacentCells(Vertex current)
        {
            List<Vertex> AdjacentCells = new List<Vertex>();

            Vector3Int front = DirectionHelper.ToDirectionalVector(current.direction);
            Vector3Int left = DirectionHelper.ToDirectionalVector(DirectionHelper.Prev(current.direction));
            Vector3Int right = DirectionHelper.ToDirectionalVector(DirectionHelper.Next(current.direction));

            AdjacentCells.Add(new Vertex(current.Position + front, current.direction));
            AdjacentCells.Add(new Vertex(current.Position + front + left, DirectionHelper.Prev(current.direction)));
            AdjacentCells.Add(new Vertex(current.Position + front + right, DirectionHelper.Next(current.direction)));

            return AdjacentCells;
        }

        private List<Vertex> GeneratePath(Dictionary<Vertex, Vertex> parentsDictionary, Vertex endState)
        {
            List<Vertex> path = new List<Vertex>();
            Vertex parent = endState;
            while (parent != null && parentsDictionary.ContainsKey(parent))
            {
                path.Add(parent);
                parent = parentsDictionary[parent];
            }
            return path;
        }

        private Vertex GetClosestVertex(List<Vertex> positionsTocheck, Dictionary<Vertex, float> priorityDictionary)
        {
            Vertex candidate = positionsTocheck[0];
            foreach (Vertex vertex in positionsTocheck)
            {
                if (priorityDictionary[vertex] < priorityDictionary[candidate])
                {
                    candidate = vertex;
                }
            }
            return candidate;
        }
    }
}
