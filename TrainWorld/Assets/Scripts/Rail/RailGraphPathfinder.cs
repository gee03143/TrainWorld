using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Source https://github.com/lordjesus/Packt-Introduction-to-graph-algorithms-for-game-developers
/// </summary>

namespace TrainWorld.Rail
{
    public static class RailGraphPathfinder
    {
        internal static List<Vertex> AStarSearch(Vector3Int placementStartPosition, Direction8way placementStartDirection, Vector3Int endPosition, bool isAgent = false, RailGraph railGraph = null)
        {
            // isAgent가 true 일 경우 Adjascent 알고리즘을 다른 걸 사용해야 함
            // agent는 이미 설치된 레일로만 이동함, agent가 아니라 레일 설치에 사용되는 경우 설치되지 않은 칸을 사용함
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

                if(ManhattanDiscance(Vector3Int.RoundToInt(current.Position), placementStartPosition) > 5 && isAgent == false)
                {
                    path = GeneratePath(parentsDictionary, current);
                    return path;
                }

                List<Vertex> nextPositions = isAgent ? GetNeighbourCells(current, railGraph) : GetAdjacentCells(current);

                foreach (Vertex nextPos in nextPositions)
                {
                    float newCost = costDictionary[current] + GetCostOfEnteringCell(nextPos, current);
                    if (!costDictionary.ContainsKey(nextPos) || newCost < costDictionary[nextPos])
                    {
                        costDictionary[nextPos] = newCost;

                        float priority = newCost + ManhattanDiscance(endPosition, Vector3Int.RoundToInt(nextPos.Position));
                        positionsTocheck.Add(nextPos);
                        priorityDictionary[nextPos] = priority;

                        parentsDictionary[nextPos] = current;
                    }
                }
            }
            return path;
        }

        private static float ManhattanDiscance(Vector3Int endPosition, Vector3Int startPosition)
        {
            return Mathf.Abs(endPosition.x - startPosition.x) + Mathf.Abs(endPosition.z - startPosition.z);
        }

        private static float GetCostOfEnteringCell(Vertex neighbour, Vertex current)
        {
            return ManhattanDiscance(Vector3Int.RoundToInt(neighbour.Position), Vector3Int.RoundToInt(current.Position));
        }

        private static List<Vertex> GetAdjacentCells(Vertex current)
        {
            List<Vertex> AdjacentCells = new List<Vertex>();

            Vector3Int front = DirectionHelper.ToDirectionalVector(current.Direction);
            Vector3Int left = DirectionHelper.ToDirectionalVector(DirectionHelper.Prev(current.Direction));
            Vector3Int right = DirectionHelper.ToDirectionalVector(DirectionHelper.Next(current.Direction));

            AdjacentCells.Add(new Vertex(current.Position + front, current.Direction));
            AdjacentCells.Add(new Vertex(current.Position + front + left, DirectionHelper.Prev(current.Direction)));
            AdjacentCells.Add(new Vertex(current.Position + front + right, DirectionHelper.Next(current.Direction)));

            return AdjacentCells;
        }

        private static List<Vertex> GetNeighbourCells(Vertex current, RailGraph railGraph)
        {
            return railGraph.GetNeighboursAt(current.Position, current.Direction);
        }

        private static List<Vertex> GeneratePath(Dictionary<Vertex, Vertex> parentsDictionary, Vertex endState)
        {
            List<Vertex> path = new List<Vertex>();
            Vertex parent = endState;
            while (parent != null && parentsDictionary.ContainsKey(parent))
            {
                path.Add(parent);
                parent = parentsDictionary[parent];
            }
            path.Reverse();
            return path;
        }

        private static Vertex GetClosestVertex(List<Vertex> positionsTocheck, Dictionary<Vertex, float> priorityDictionary)
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
