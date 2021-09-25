using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TrainWorld.Traffic;

namespace TrainWorld.Rails
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
        private RailArrowUI railArrowUI;
        [SerializeField]
        private GameObject railPrefab;
        [SerializeField]
        private Transform railParent;

        private HashSet<(Vector3Int, Direction8way)> railsToFix;

        private List<(Vector3Int, Direction8way)> tempRailPositions;

        [SerializeField]
        private RailBlockManager railBlockManager;

        private Dictionary<(Vector3Int, Direction8way), Rail> tempRails;

        private bool placementMode = false;
        private Vector3Int placementStartPosition;
        private Direction8way placementStartDirection;

        private Rail railUnderCursor;

        private void Awake()
        {
            railsToFix = new HashSet<(Vector3Int, Direction8way)>();
            tempRailPositions = new List<(Vector3Int, Direction8way)>();
            tempRails = new Dictionary<(Vector3Int, Direction8way), Rail>();
            placementStartDirection = Direction8way.N;
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
                    AddRailAt((roundedPosition, placementStartDirection), (roundedPosition, placementStartDirection));
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

        private void AddRailAt((Vector3Int, Direction8way) startFrom, (Vector3Int, Direction8way) placeAt)
        {
            InstantiateRailPrefab(placeAt);
            InstantiateRailPrefab((placeAt.Item1, placeAt.Item2.Opposite()));

            Rail railAtPosition = PlacementManager.GetRailAt(placeAt);
            Rail railAtOpposite = PlacementManager.GetRailAt(placeAt.Item1, placeAt.Item2.Opposite());

            if (startFrom != placeAt){
                PlacementManager.GetRailAt(startFrom).AddNeighbour(placeAt);
                PlacementManager.GetRailAt((placeAt.Item1, placeAt.Item2.Opposite()))
                    .AddNeighbour((startFrom.Item1, startFrom.Item2.Opposite()));
            }

            List<RailBlock> adjascentRailBlocks = railBlockManager.GetAllAdjascentRailBlocks(placeAt.Item1, placeAt.Item2);
            Debug.Log("adjascentRailBlocks.Count : " + adjascentRailBlocks.Count);
            if (adjascentRailBlocks.Count == 0) // 근처 railblock 이 없다면 새 railblock을 만듬
            {
                RailBlock newBlock = railBlockManager.MakeNewBlock(placeAt.Item1, placeAt.Item2);
                newBlock.UpdateRailsBlockReference();
            }
            else // 근처 railblock 존재할 경우 모든 railBlock을 합침
            {
                RailBlock newBlock = railBlockManager.MakeNewBlock(placeAt.Item1, placeAt.Item2);
                railBlockManager.Unite(newBlock, adjascentRailBlocks);
                newBlock.UpdateRailsBlockReference();
            }

            AddPositionToRailsToFix(placeAt.Item1, placeAt.Item2);
            AddAdjascentPositionsToRailsToFix(placeAt.Item1, placeAt.Item2);
            FixRails();
        }

        private void InstantiateRailPrefab((Vector3Int, Direction8way) position, bool isTemp = false)
        {
            if (isTemp == false)
            {
                if (PlacementManager.GetRailAt(position) == null)
                {
                    GameObject newObject = Instantiate(railPrefab, position.Item1, Quaternion.Euler(position.Item2.ToEuler()), railParent) as GameObject;
                    Rail newRail = newObject.GetComponent<Rail>();
                    newRail.Init(position.Item1, position.Item2);

                    (Vector3Int, Direction8way) frontPos = (position.Item1 + position.Item2.ToDirectionalVector(), position.Item2);
                    (Vector3Int, Direction8way) rearPos = (position.Item1 - position.Item2.ToDirectionalVector(), position.Item2);

                    //search nearby rails, if adjascent add to neighbour
                    if (PlacementManager.IsRailAtPosition(frontPos))
                    {
                        newRail.AddNeighbour(frontPos);
                    }
                    if (PlacementManager.IsRailAtPosition(rearPos))
                    {
                        PlacementManager.GetRailAt(rearPos).AddNeighbour(position);
                    }

                    PlacementManager.AddRailAt(position, newRail);
                }
            }
            else
            {
                GameObject newObject = Instantiate(railPrefab, position.Item1, Quaternion.Euler(position.Item2.ToEuler()), railParent) as GameObject;
                Rail newRail = newObject.GetComponent<Rail>();
                newRail.Init(position.Item1, position.Item2);
                tempRails.Add(position, newRail);

                newRail.AddNeighbours(tempRailPositions);
            }
        }

        private void AddPositionToRailsToFix(Vector3Int position, Direction8way direction)
        {
            railsToFix.Add((position, direction));
            railsToFix.Add((position, direction.Opposite()));
        }

        private void AddAdjascentPositionsToRailsToFix(Vector3Int position, Direction8way direction)
        {
            if(PlacementManager.IsRailAtPosition((position, direction)))
                railsToFix.UnionWith(PlacementManager.GetRailAt(position, direction).GetNeighbourTuples());
            if (PlacementManager.IsRailAtPosition((position, direction.Opposite())))
                railsToFix.UnionWith(PlacementManager.GetRailAt(position, direction.Opposite()).GetNeighbourTuples());
        }

        private void FixRails()
        {
            foreach (var railToFix in railsToFix)
            {
                if (PlacementManager.IsRailAtPosition(railToFix))
                    PlacementManager.GetRailAt(railToFix).FixMyModel();
                if (PlacementManager.IsRailAtPosition((railToFix.Item1, railToFix.Item2.Opposite())))
                    PlacementManager.GetRailAt(railToFix.Item1, railToFix.Item2.Opposite()).FixMyModel();
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
            railUnderCursor = PlacementManager.GetRailViaMousePosition(cursorPosition);
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

        public void RemoveRail(Vector3 position)
        {
            (Vector3Int, Direction8way) cursorPos = (railUnderCursor.Position, railUnderCursor.Direction);
            (Vector3Int, Direction8way) opposite = (railUnderCursor.Position, railUnderCursor.Direction.Opposite());

            // if selected position is out of border or empty do nothing
            if (IsPositionOutOfBorder(railUnderCursor.Position) || PlacementManager.IsRailAtPosition(cursorPos) == false)
                return;

            AddAdjascentPositionsToRailsToFix(railUnderCursor.Position, railUnderCursor.Direction);


            if (PlacementManager.IsRailAtPosition(cursorPos) == true || PlacementManager.IsRailAtPosition(opposite) == true)
            {
                foreach (var neighbour in PlacementManager.GetRailAt(cursorPos).GetNeighbourTuples())
                {
                    PlacementManager.GetRailAt((neighbour.Item1, neighbour.Item2.Opposite())).RemoveNeighbourAt(opposite);
                }

                foreach (var neighbour in PlacementManager.GetRailAt(opposite).GetNeighbourTuples())
                {
                    PlacementManager.GetRailAt((neighbour.Item1, neighbour.Item2.Opposite())).RemoveNeighbourAt(cursorPos);
                }
                railBlockManager.Split(PlacementManager.GetRailAt(cursorPos).myRailblock, cursorPos.Item1, cursorPos.Item2);
                railBlockManager.Split(PlacementManager.GetRailAt(opposite).myRailblock, opposite.Item1, opposite.Item2);
               
                PlacementManager.GetRailAt(cursorPos).DestroyMyself();
                PlacementManager.RemoveRailAt(cursorPos);
               
                PlacementManager.GetRailAt(opposite).DestroyMyself();
                PlacementManager.RemoveRailAt(opposite);
            }

            FixRails();
        }

        internal void MoveCursorAtDestruction(Vector3 cursorPosition)
        {
            railUnderCursor = PlacementManager.GetRailViaMousePosition(cursorPosition);
            ShowRailUI(cursorPosition);
        }

        private void ShowRailUI(Vector3 cursorPosition)
        {
            if (railUnderCursor != null)
            {
                railArrowUI.arrow.SetActive(true);
                railArrowUI.arrow.transform.position = railUnderCursor.Position;

                railArrowUI.arrow.transform.rotation = Quaternion.Euler(railUnderCursor.Direction.ToEuler());
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
                InstantiateRailPrefab((tempRailPositions[i].Item1, tempRailPositions[i].Item2.Opposite()), true);
                if (i != 0)
                {
                    tempRails[tempRailPositions[i - 1]].AddNeighbour(tempRailPositions[i]);
                    tempRails[(tempRailPositions[i ].Item1, tempRailPositions[i].Item2.Opposite())]
                        .AddNeighbour((tempRailPositions[i - 1].Item1, tempRailPositions[i - 1].Item2.Opposite()));
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
                placementStartDirection = placementStartDirection.Next();
                RemoveTempRails();
                tempRailPositions.Clear();
                tempRailPositions.Add((placementStartPosition, placementStartDirection));
                CreateTempRailModel();
            }
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