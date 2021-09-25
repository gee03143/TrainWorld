using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Source https://github.com/lordjesus/Packt-Introduction-to-graph-algorithms-for-game-developers
/// </summary>

namespace TrainWorld.Rails
{
    public static class RailGraphPathfinder
    {
        private const int MAXSEARCHLENGTH = 10;

        private static (Vector3Int, Direction8way) nullVertex = (Vector3Int.zero, Direction8way.DIRECTION_COUNT);

        internal static List<(Vector3Int, Direction8way)> AStarSearch(Vector3Int placementStartPosition, Direction8way placementStartDirection,
            Vector3Int endPosition, Direction8way endDirection = Direction8way.DIRECTION_COUNT, bool isAgent = false, Dictionary<(Vector3Int, Direction8way), Rail> railGraph = null)
        {
            // isAgent가 true 일 경우 Adjascent 알고리즘을 다른 걸 사용해야 함
            // agent는 이미 설치된 레일로만 이동함, agent가 아니라 레일 설치에 사용되는 경우 설치되지 않은 칸을 사용함
            List<(Vector3Int, Direction8way)> path = new List<(Vector3Int, Direction8way)>();

            List<(Vector3Int, Direction8way)> positionsTocheck = new List<(Vector3Int, Direction8way)>();
            Dictionary<(Vector3Int, Direction8way), float> costDictionary = new Dictionary<(Vector3Int, Direction8way), float>();
            Dictionary<(Vector3Int, Direction8way), float> priorityDictionary = new Dictionary<(Vector3Int, Direction8way), float>();
            Dictionary<(Vector3Int, Direction8way), (Vector3Int, Direction8way)> parentsDictionary = new Dictionary<(Vector3Int, Direction8way), (Vector3Int, Direction8way)>();

            positionsTocheck.Add((placementStartPosition, placementStartDirection));
            priorityDictionary.Add((placementStartPosition, placementStartDirection), 0);
            costDictionary.Add((placementStartPosition, placementStartDirection), 0);
            parentsDictionary.Add((placementStartPosition, placementStartDirection), nullVertex);

            while (positionsTocheck.Count > 0)
            {
                (Vector3Int, Direction8way) currentPos = GetClosestVertex(positionsTocheck, priorityDictionary);
                positionsTocheck.Remove(currentPos);
                if (currentPos.Item1.Equals(endPosition) && (currentPos.Item2.Equals(endDirection) || isAgent == false))
                {
                    path = GeneratePath(parentsDictionary, currentPos);
                    return path;
                }

                if(MathFunctions.ManhattanDiscance((currentPos.Item1), placementStartPosition) > MAXSEARCHLENGTH 
                    && isAgent == false)
                {
                    path = GeneratePath(parentsDictionary, currentPos);
                    return path;
                }

                List<(Vector3Int, Direction8way)> nextPositions = isAgent ? GetNeighbourCells(currentPos, railGraph) : GetAdjacentCells(currentPos);

                foreach ((Vector3Int, Direction8way) nextPos in nextPositions)
                {
                    float newCost = costDictionary[currentPos] + GetCostOfEnteringCell(nextPos, currentPos, isAgent);
                    if (!costDictionary.ContainsKey(nextPos) || newCost < costDictionary[nextPos])
                    {
                        costDictionary[nextPos] = newCost;

                        float priority = newCost + (float)MathFunctions.ManhattanDiscance(endPosition, nextPos.Item1);
                        positionsTocheck.Add(nextPos);
                        priorityDictionary[nextPos] = priority;

                        parentsDictionary[nextPos] = currentPos;
                    }
                }
            }
            return path;
        }

        private static float GetCostOfEnteringCell((Vector3Int, Direction8way) next, (Vector3Int, Direction8way) prev, bool isAgent)
        {
            float penalty = 0.0f;
            if (isAgent)    //  if it's agent  we should calculate penalty
            {
                if (PlacementManager.GetRailAt(next).myRailblock.hasAgent)
                {
                    penalty += 10;
                }
            }
            return (float)MathFunctions.ManhattanDiscance(Vector3Int.RoundToInt(next.Item1), Vector3Int.RoundToInt(prev.Item1)) + penalty;
        }

        private static List<(Vector3Int, Direction8way)> GetAdjacentCells((Vector3Int, Direction8way) current)
        {
            List<(Vector3Int, Direction8way)> AdjacentCells = new List<(Vector3Int, Direction8way)>();

            Vector3Int front =  current.Item2.ToDirectionalVector();
            Vector3Int left = current.Item2.Prev().ToDirectionalVector();
            Vector3Int right = current.Item2.Next().ToDirectionalVector();

            //check if selected position is empty
            if(PlacementManager.GetPlacementTypeAt(current.Item1 + front) != PlacementType.eBuilding)
                AdjacentCells.Add((current.Item1 + front, current.Item2));
            if (PlacementManager.GetPlacementTypeAt(current.Item1 + front + left) != PlacementType.eBuilding)
                AdjacentCells.Add((current.Item1 + front + left, current.Item2.Prev()));
            if (PlacementManager.GetPlacementTypeAt(current.Item1 + front + right) != PlacementType.eBuilding)
                AdjacentCells.Add((current.Item1 + front + right, current.Item2.Next()));

            return AdjacentCells;
        }

        private static List<(Vector3Int, Direction8way)> GetNeighbourCells((Vector3Int, Direction8way) current, 
            Dictionary<(Vector3Int, Direction8way), Rail> railGraph)
        {
            return railGraph[current].GetNeighbourTuples();
        }

        private static List<(Vector3Int, Direction8way)> GeneratePath(Dictionary<(Vector3Int, Direction8way),
            (Vector3Int, Direction8way)> parentsDictionary, (Vector3Int, Direction8way) endState)
        {
            List<(Vector3Int, Direction8way)> path = new List<(Vector3Int, Direction8way)>();
            (Vector3Int, Direction8way) parent = endState;
            while (parent != (Vector3Int.zero, Direction8way.DIRECTION_COUNT) && parentsDictionary.ContainsKey(parent))
            {
                path.Add(parent);
                parent = parentsDictionary[parent];
            }
            path.Reverse();
            return path;
        }

        private static (Vector3Int, Direction8way) GetClosestVertex(List<(Vector3Int, Direction8way)> positionsTocheck, Dictionary<(Vector3Int, Direction8way), float> priorityDictionary)
        {
            (Vector3Int, Direction8way) candidate = positionsTocheck[0];
            foreach ((Vector3Int, Direction8way) vertex in positionsTocheck)
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
