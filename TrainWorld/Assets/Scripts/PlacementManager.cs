using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TrainWorld.Rails;
using TrainWorld.Traffic;
using TrainWorld.Audio;
using System;

namespace TrainWorld
{
    public enum PlacementType
    {
        eRail,
        eBuilding,
        eEmpty
    }

    public class PlacementManager
    {
        private static Dictionary<Vector3Int, PlacementType> placementData;

        private static Dictionary<(Vector3Int, Direction8way), Rail> railData;
        private static Dictionary<string, TrainStation> stationData;

        static PlacementManager()
        {
            placementData = new Dictionary<Vector3Int, PlacementType>();
            railData = new Dictionary<(Vector3Int, Direction8way), Rail>();
            stationData = new Dictionary<string, TrainStation>();
        }

        public static PlacementType GetPlacementTypeAt(Vector3Int position)
        {
            if (placementData.ContainsKey(position) == false)
                return PlacementType.eEmpty;

            return placementData[position];
        }

        internal static void AddBuildingAt(Vector3Int roundedPosition)
        {
            placementData.Add(roundedPosition, PlacementType.eBuilding);
            AudioManager.instance.PlaySound("Placement_Building");
        }

        public static bool IsRailAtPosition((Vector3Int, Direction8way) position)
        {
            return railData.ContainsKey(position) == true;
        }

        public static void AddRailAt((Vector3Int, Direction8way) position, Rail newRail)
        {
            placementData[position.Item1] = PlacementType.eRail;
            railData.Add(position, newRail);
            AudioManager.instance.PlaySound("Placement_Rail");
        }

        public static Rail GetRailAt(Vector3Int position, Direction8way direction)
        {
            if (railData.ContainsKey((position, direction)) == false)
                return null;

            return railData[(position, direction)];
        }

        public static Rail GetRailAt((Vector3Int, Direction8way) position)
        {
            if (railData.ContainsKey(position) == false)
                return null;

            return railData[position];
        }

        public static void RemoveRailAt((Vector3Int, Direction8way) position)
        {
            // TODO : remove station/traffic of rail, 함께 삭제되어야 함
            railData.Remove(position);

            // 만약 해당 position에 더 이상 rail이 남아 있지 않다면 placementData에서 key 삭제
            if (GetRailsAtPosition(position.Item1).Count == 0)
                placementData.Remove(position.Item1);
            AudioManager.instance.PlaySound("Destruction");
        }

        internal static void RemoveStationOfName(string stationName)
        {
            stationData.Remove(stationName);
            AudioManager.instance.PlaySound("Destruction");
        }

        public static List<Rail> GetRailsAtPosition(Vector3Int position)
        {
            List<Rail> railsAtPosition = railData.Where(x => position == x.Key.Item1).Select(x => x.Value).ToList();

            return railsAtPosition;
        }

        public static Rail GetRailViaMousePosition(Vector3 mousePosition, bool findByTrafficSocket = false)
        {
            // railModel 의 위치를 기준으로 찾는 방법과 trafficsocket의 위치를 기준으로 찾는 두 가지 모드 존재

            Vector3Int roundedPosition = Vector3Int.RoundToInt(mousePosition);
            List<Rail> railsAtPosition = railData.Values.Where(x => roundedPosition == x.Position).ToList();

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
                    return railData[(roundedPosition, nearestDirection)];
                }
            }
            return null;
        }

        public static List<(Vector3Int, Direction8way)> GetRailPathForAgent(Vector3Int startPosition, Direction8way startDirection, Vector3Int endPosition, Direction8way endDirection)
        {
            return RailGraphPathfinder.AStarSearch(startPosition, startDirection, endPosition, endDirection, true, railData);
        }

        public static void AddStationOfName(string name, TrainStation newStation)
        {
            stationData.Add(name, newStation);
            AudioManager.instance.PlaySound("Placement_Building");
        }

        public static TrainStation GetStationOfName(string name)
        {
            if (stationData.ContainsKey(name) == false)
                return null;

            return stationData[name];
        }

        public static Dictionary<string, TrainStation> GetStations()
        {
            return stationData;
        }
    }
}
