using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rails;
using System.Linq;

namespace TrainWorld.Traffic
{
    public static class BFSSearcher
    {
        public static HashSet<(Vector3Int, Direction8way)> BFSSearch((Vector3Int, Direction8way) startRail, RailBlock block)
        {
            Queue<(Vector3Int, Direction8way)> railsToCheck = new Queue<(Vector3Int, Direction8way)>();

            HashSet<(Vector3Int, Direction8way)> visited = new HashSet<(Vector3Int, Direction8way)>();

            railsToCheck.Enqueue(startRail);
            while(railsToCheck.Count > 0)
            {
                (Vector3Int, Direction8way) current = railsToCheck.Dequeue();
                visited.Add(current);

                List<(Vector3Int, Direction8way)> neighbourTuples = PlacementManager.GetRailAt(current).GetNeighbourTuples();

                List<(Vector3Int, Direction8way)> adjascentPositions = neighbourTuples.Select(x => (x.Item1, DirectionHelper.Opposite(x.Item2))).ToList();
                foreach (var adjascent in adjascentPositions)
                {
                    visited.Add(adjascent);
                }
                
                foreach ((Vector3Int, Direction8way) neighbour in neighbourTuples)
                {
                    if (visited.Contains(neighbour) == false) // on unvisited neighbour
                    {
                        if (PlacementManager.GetRailAt(neighbour).HasTraffic() == false &&
                        PlacementManager.GetRailAt((neighbour.Item1, DirectionHelper.Opposite(neighbour.Item2))).HasTraffic() == false)
                        // if neighbour dont has traffic
                        {
                            List<(Vector3Int, Direction8way)> railsAtPos = 
                                PlacementManager.GetRailsAtPosition(neighbour.Item1).Select(x => (x.Position, x.Direction)).ToList();
                            foreach (var railAtPos in railsAtPos)
                            {
                                railsToCheck.Enqueue(railAtPos);
                            }
                        }
                    }
                }
            }

            return visited;
        }
    }
}
