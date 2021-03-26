using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TrainWorld.Rail
{
    // Rail 배치에 대한 입력 정의
    // Dictionary에 새 Rail이 입력되도록 함
    // railGraph에 새로 입력된 Node, Edge를 전달함
    // railModelManager 새 railModel 저장하도록 함
    // 

    // 언젠가 맵 위에 레일 외의 오브젝트가 배치될 경우 변경될 가능성 있음

    public class RailPlacementManager : MonoBehaviour, InputHandler
    {

        [SerializeField]
        int maxWidth;
        [SerializeField]
        int maxHeight;

        [SerializeField]
        RailArrowUI railArrowUI;

        [SerializeField]
        private GameObject railPrefab;
        [SerializeField]
        private Transform railParent;

        private HashSet<(Vector3Int, Direction8way)> railsToFix;

        private List<(Vector3Int, Direction8way)> tempRailPositions;

        private Dictionary<(Vector3Int, Direction8way), Rail> rails;
        private Dictionary<(Vector3Int, Direction8way), Rail> tempRails;

        private bool placementMode = false;
        private Vector3Int placementStartPosition;
        private Direction8way placementStartDirection;

        private Rail railUnderCursor;

        private void Awake()
        {
            railsToFix = new HashSet<(Vector3Int, Direction8way)>();
            tempRailPositions = new List<(Vector3Int, Direction8way)>();
            rails = new Dictionary<(Vector3Int, Direction8way), Rail>();
            tempRails = new Dictionary<(Vector3Int, Direction8way), Rail>();
            placementStartDirection = Direction8way.N;
        }

        public Dictionary<(Vector3Int, Direction8way), Rail> GetRails()
        {
            return rails;
        }

        internal List<Rail> GetRailsAtPosition(Vector3Int position)
        {
            List<Rail> railsAtPosition = rails.Where(x => position == x.Key.Item1).Select(x => x.Value).ToList();

            return railsAtPosition;
        }

        private bool isPositionEmpty(Vector3Int position, Direction8way direction)
        {
            return rails.ContainsKey((position, direction)) == false;
        }

        private bool IsPositionOutOfBorder(Vector3Int position)
        {
            return position.x < 0 || position.x > maxWidth || position.z < 0 || position.z > maxHeight;
        }

        public void OnMouseDown(Vector3 mousePosition)
        {
            PlaceRail(mousePosition);
        }

        private void PlaceRail(Vector3 position)
        {
            Vector3Int roundedPosition = Vector3Int.RoundToInt(position);

            //해당 위치가 경계 바깥이면 종료 
            if (IsPositionOutOfBorder(roundedPosition))
                return;

            if (placementMode)  //  confirm rail placement, place rail at temp rail positions
            {
                CreateRailAtTempRailPositions();
                RemoveTempRails();
                tempRailPositions.Clear();
                placementMode = false;
            }
            else
            {
                if (railUnderCursor == null)
                {
                    InstantiateRailPrefab((roundedPosition, placementStartDirection));
                    InstantiateRailPrefab((roundedPosition, DirectionHelper.Opposite(placementStartDirection)));

                    AddPositionToRailsToFix(roundedPosition, placementStartDirection);
                    AddAdjascentPositionsToRailsToFix(roundedPosition, placementStartDirection);

                    FixRails();
                }
                else
                {
                    placementMode = true;
                    placementStartPosition = railUnderCursor.Position;
                    placementStartDirection = railUnderCursor.Direction;
                }
            }
        }

        private void CreateRailAtTempRailPositions()
        {
            (Vector3Int, Direction8way) lastTuple = (Vector3Int.zero, Direction8way.DIRECTION_COUNT);
            foreach (var pos in tempRailPositions)
            {
                if (lastTuple != (Vector3Int.zero, Direction8way.DIRECTION_COUNT))
                    AddRailAt(lastTuple, pos);
                else
                    AddRailAt(pos, pos);
                lastTuple = pos;
            }
        }

        private void AddRailAt((Vector3Int, Direction8way) start, (Vector3Int, Direction8way) next)
        {
            InstantiateRailPrefab(next);
            InstantiateRailPrefab((next.Item1, DirectionHelper.Opposite(next.Item2)));
            if(start != next){
                rails[start].AddNeighbour(next);
                rails[(next.Item1, DirectionHelper.Opposite(next.Item2))].AddNeighbour((start.Item1, DirectionHelper.Opposite(start.Item2)));
            }

            AddPositionToRailsToFix(next.Item1, next.Item2);
            AddAdjascentPositionsToRailsToFix(next.Item1, next.Item2);
            FixRails();
        }

        private void InstantiateRailPrefab((Vector3Int, Direction8way) position, bool isTemp = false)
        {
            if (isTemp == false)
            {
                if (rails.ContainsKey(position) == false)
                {
                    GameObject newObject = Instantiate(railPrefab, position.Item1, Quaternion.Euler(DirectionHelper.ToEuler(position.Item2)), railParent) as GameObject;
                    Rail newRail = newObject.GetComponent<Rail>();
                    newRail.Init(position.Item1, position.Item2);
                    rails.Add(position, newRail);

                    (Vector3Int, Direction8way) frontPos = (position.Item1 + DirectionHelper.ToDirectionalVector(position.Item2), position.Item2);
                    if (rails.ContainsKey(frontPos)){
                        newRail.AddNeighbour(frontPos);
                    }
                    (Vector3Int, Direction8way) rearPos = (position.Item1 - DirectionHelper.ToDirectionalVector(position.Item2), position.Item2);
                    if (rails.ContainsKey(rearPos))
                    {
                        rails[rearPos].AddNeighbour(position);
                    }
                }
            }
            else
            {
                GameObject newObject = Instantiate(railPrefab, position.Item1, Quaternion.Euler(DirectionHelper.ToEuler(position.Item2)), railParent) as GameObject;
                Rail newRail = newObject.GetComponent<Rail>();
                newRail.Init(position.Item1, position.Item2);
                tempRails.Add(position, newRail);

                newRail.AddNeighbours(tempRailPositions);
            }
        }

        private void AddPositionToRailsToFix(Vector3Int position, Direction8way direction)
        {
            railsToFix.Add((position, direction));
            railsToFix.Add((position, DirectionHelper.Opposite(direction)));
        }

        private void AddAdjascentPositionsToRailsToFix(Vector3Int position, Direction8way direction)
        {
            railsToFix.UnionWith(rails[(position, direction)].GetNeighbourTuples());
            railsToFix.UnionWith(rails[(position, DirectionHelper.Opposite(direction))].GetNeighbourTuples());
        }

        private void FixRails()
        {
            foreach (var railToFix in railsToFix)
            {
                rails[railToFix].FixMyModel();
                rails[(railToFix.Item1, DirectionHelper.Opposite(railToFix.Item2))].FixMyModel();
            }

            railsToFix.Clear();
        }

        private void RemoveTempRails()
        {
            foreach (var tempRail in tempRails)
            {
                Destroy(tempRail.Value.gameObject);
            }
            tempRails.Clear();
        }

        public void OnMouseMove(Vector3 mousePosition)
        {
            MoveCursorAtRailPlacement(mousePosition);
        }

        private void MoveCursorAtRailPlacement(Vector3 cursorPosition)
        {
            Vector3Int roundedPosition = Vector3Int.RoundToInt(cursorPosition);
            railUnderCursor = GetRailViaMousePosition(cursorPosition);
            ShowRailUI(cursorPosition);
            if (placementMode)
            {
                //clear temp rails
                RemoveTempRails();
                tempRailPositions.Clear();
                tempRailPositions = RailGraphPathfinder.AStarSearch(placementStartPosition, placementStartDirection, roundedPosition);

                //place temp rails
                CreateTempRailModel();
            }
            else
            {
                placementStartPosition = roundedPosition;

                RemoveTempRails();
                tempRailPositions.Clear();
                if (railUnderCursor == null)
                {
                    tempRailPositions.Add((placementStartPosition, placementStartDirection));

                    CreateTempRailModel();
                }
            }
        }

        public Rail GetRailViaMousePosition(Vector3 mousePosition, bool findByTrafficSocket = false)
        {
            // railModel 의 위치를 기준으로 찾는 방법과 trafficsocket의 위치를 기준으로 찾는 두 가지 모드 존재

            Vector3Int roundedPosition = Vector3Int.RoundToInt(mousePosition);
            List<Rail> railsAtPosition = rails.Values.Where(x => roundedPosition == x.Position).ToList();

            if (railsAtPosition.Count > 0)
            {
                if (findByTrafficSocket)
                {
                    if(railsAtPosition.Count != 2)
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
                    float shortestDistance = Vector3.Distance(railsAtPosition[0].Position + Vector3.Normalize(DirectionHelper.ToDirectionalVector(railsAtPosition[0].Direction)),
                        mousePosition);
                    foreach (var model in railsAtPosition)
                    {
                        float newDistance = Vector3.Distance(model.Position + Vector3.Normalize(DirectionHelper.ToDirectionalVector(model.Direction)),
                            mousePosition);
                        if (shortestDistance > newDistance)
                        {
                            shortestDistance = newDistance;
                            nearestDirection = model.Direction;
                        }
                    }
                    return rails[(roundedPosition, nearestDirection)];
                }
            }
            return null;
        }

        public void RemoveRail(Vector3 position)
        {
            (Vector3Int, Direction8way) cursorPos = (railUnderCursor.Position, railUnderCursor.Direction);
            (Vector3Int, Direction8way) opposite = (railUnderCursor.Position, DirectionHelper.Opposite(railUnderCursor.Direction));

            // if selected position is out of border or empty do nothing
            if (IsPositionOutOfBorder(railUnderCursor.Position) || isPositionEmpty(railUnderCursor.Position, railUnderCursor.Direction))
                return;

            AddAdjascentPositionsToRailsToFix(railUnderCursor.Position, railUnderCursor.Direction);


            if (rails.ContainsKey(cursorPos) || rails.ContainsKey(opposite))
            {
                foreach (var neighbour in rails[cursorPos].GetNeighbourTuples())
                {
                    rails[(neighbour.Item1, DirectionHelper.Opposite(neighbour.Item2))].RemoveNeighbourAt(opposite);
                }

                foreach (var neighbour in rails[opposite].GetNeighbourTuples())
                {
                    rails[(neighbour.Item1, DirectionHelper.Opposite(neighbour.Item2))].RemoveNeighbourAt(cursorPos);
                }
                rails[cursorPos].DestroyMyself();
                rails.Remove(cursorPos);
                rails[opposite].DestroyMyself();
                rails.Remove(opposite);
            }

            FixRails();
        }

        internal void MoveCursorAtDestruction(Vector3 cursorPosition)
        {
            railUnderCursor = GetRailViaMousePosition(cursorPosition);
            ShowRailUI(cursorPosition);
        }

        private void ShowRailUI(Vector3 cursorPosition)
        {
            if (railUnderCursor != null)
            {
                railArrowUI.arrow.SetActive(true);
                railArrowUI.arrow.transform.position = railUnderCursor.Position;

                railArrowUI.arrow.transform.rotation = Quaternion.Euler(DirectionHelper.ToEuler(railUnderCursor.Direction));
            }
            else
            {
                HideRailArrowUI();
            }
        }

        public void HideRailArrowUI()
        {
            railArrowUI.arrow.SetActive(false);
        }

        private void CreateTempRailModel()
        {
            for (int i = 0; i < tempRailPositions.Count; i++)
            {
                InstantiateRailPrefab((tempRailPositions[i].Item1, tempRailPositions[i].Item2), true);
                InstantiateRailPrefab((tempRailPositions[i].Item1, DirectionHelper.Opposite(tempRailPositions[i].Item2)), true);
                if (i != 0)
                {
                    tempRails[tempRailPositions[i - 1]].AddNeighbour(tempRailPositions[i]);
                    tempRails[(tempRailPositions[i ].Item1, DirectionHelper.Opposite(tempRailPositions[i].Item2))]
                        .AddNeighbour((tempRailPositions[i - 1].Item1, DirectionHelper.Opposite(tempRailPositions[i - 1].Item2)));
                }
            }
            foreach (var item in tempRails)
            {
                item.Value.FixMyModel();
            }
        }

        public void OnRInput()
        {
            RotateCursor();
        }
        private void RotateCursor()
        {
            if (placementMode == false)
            {
                placementStartDirection = DirectionHelper.Next(placementStartDirection);
                RemoveTempRails();
                tempRailPositions.Clear();
                tempRailPositions.Add((placementStartPosition, placementStartDirection));
                CreateTempRailModel();
            }
        }

        internal List<(Vector3Int, Direction8way)> GetRailPath(Vector3Int startPosition, Direction8way startDirection, Vector3Int endPosition, Direction8way endDirection)
        {
            return RailGraphPathfinder.AStarSearch(startPosition, startDirection, endPosition, endDirection, true, rails);
        }

        internal void RailPlacementEnter()
        {
            placementMode = false;
        }

        public void OnEnter()
        {
            Debug.Log("Rail Placement Enter");
        }

        public void OnExit()
        {
            RemoveTempRails();
            tempRailPositions.Clear();
            railArrowUI.arrow.SetActive(false);
            placementMode = false;
            Debug.Log("Rail Placement Exit");
        }
    }
}