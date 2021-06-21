using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace TrainWorld.Rails
{
    public class PlacementManager
    {
        private static Dictionary<(Vector3Int, Direction8way), Rail> placementData;

        static PlacementManager()
        {
            placementData = new Dictionary<(Vector3Int, Direction8way), Rail>();
        }

        public static bool IsEmpty(Vector3Int position, Direction8way direction)
        {
            return placementData.ContainsKey((position, direction)) == false;
        }

        public static bool IsEmpty((Vector3Int, Direction8way) position)
        {
            return placementData.ContainsKey(position) == false;
        }

        public static void AddRailAt((Vector3Int, Direction8way) position, Rail newRail)
        {
            placementData.Add((position), newRail);
        }

        public static Rail GetRailAt(Vector3Int position, Direction8way direction)
        {
            if (placementData.ContainsKey((position, direction)) == false)
                return null;

            return placementData[(position, direction)];
        }

        public static Rail GetRailAt((Vector3Int, Direction8way) position)
        {
            if (placementData.ContainsKey((position)) == false)
                return null;

            return placementData[(position)];
        }

        public static void RemoveRailAt((Vector3Int, Direction8way) position)
        {
            placementData.Remove(position);
        }


        public static List<Rail> GetRailsAtPosition(Vector3Int position)
        {
            List<Rail> railsAtPosition = placementData.Where(x => position == x.Key.Item1).Select(x => x.Value).ToList();

            return railsAtPosition;
        }

        public static Rail GetRailViaMousePosition(Vector3 mousePosition, bool findByTrafficSocket = false)
        {
            // railModel 의 위치를 기준으로 찾는 방법과 trafficsocket의 위치를 기준으로 찾는 두 가지 모드 존재

            Vector3Int roundedPosition = Vector3Int.RoundToInt(mousePosition);
            List<Rail> railsAtPosition = placementData.Values.Where(x => roundedPosition == x.Position).ToList();

            if (railsAtPosition.Count > 0)
            {
                if (findByTrafficSocket)
                {
                    if (railsAtPosition.Count != 2)
                        return null;
                    if (Vector3.Distance(railsAtPosition[0].trafficSocketTransform.position, mousePosition) >
                        Vector3.Distance(railsAtPosition[1].trafficSocketTransform.position, mousePosition))
                        return railsAtPosition[1];
                    else
                        return railsAtPosition[0];
                }
                else
                {
                    Direction8way nearestDirection = railsAtPosition[0].Direction;
                    float shortestDistance = Vector3.Distance(railsAtPosition[0].Position + 
                        Vector3.Normalize(railsAtPosition[0].Direction.ToDirectionalVector()), mousePosition);
                    foreach (var model in railsAtPosition)
                    {
                        float newDistance = Vector3.Distance(model.Position + Vector3.Normalize(model.Direction.ToDirectionalVector()),
                            mousePosition);
                        if (shortestDistance > newDistance)
                        {
                            shortestDistance = newDistance;
                            nearestDirection = model.Direction;
                        }
                    }
                    return placementData[(roundedPosition, nearestDirection)];
                }
            }
            return null;
        }

        public static List<(Vector3Int, Direction8way)> GetRailPathForAgent(Vector3Int startPosition, Direction8way startDirection, Vector3Int endPosition, Direction8way endDirection)
        {
            return RailGraphPathfinder.AStarSearch(startPosition, startDirection, endPosition, endDirection, true, placementData);
        }
    }
}
