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
            InstantiateRailPrefab((placeAt.Item1, DirectionHelper.Opposite(placeAt.Item2)));

            Rail railAtPosition = PlacementManager.GetRailAt(placeAt);
            Rail railAtOpposite = PlacementManager.GetRailAt(placeAt.Item1, DirectionHelper.Opposite(placeAt.Item2));

            if (startFrom != placeAt){
                PlacementManager.GetRailAt(startFrom).AddNeighbour(placeAt);
                PlacementManager.GetRailAt((placeAt.Item1, DirectionHelper.Opposite(placeAt.Item2)))
                    .AddNeighbour((startFrom.Item1, DirectionHelper.Opposite(startFrom.Item2)));
            }

            List<RailBlock> adjascentRailBlocks = GetAllAdjascentRailBlocks(placeAt, railAtPosition, railAtOpposite);
            if(adjascentRailBlocks.Count == 0)
            {
                RailBlock newBlock = new RailBlock();
                newBlock.AddRail(placeAt);
                newBlock.AddRail((placeAt.Item1, DirectionHelper.Opposite(placeAt.Item2)));
                railAtPosition.myRailblock = newBlock;
                railAtOpposite.myRailblock = newBlock;
            }
            else if(adjascentRailBlocks.Count == 1)
            {
                RailBlock adjascentBlock = adjascentRailBlocks[0];
                adjascentBlock.AddRail(placeAt);
                adjascentBlock.AddRail((placeAt.Item1, DirectionHelper.Opposite(placeAt.Item2)));
                railAtPosition.myRailblock = adjascentBlock;
                railAtOpposite.myRailblock = adjascentBlock;
            }
            else
            {
                Debug.Log(adjascentRailBlocks.Count);
                //some merge functions
            }

            AddPositionToRailsToFix(placeAt.Item1, placeAt.Item2);
            AddAdjascentPositionsToRailsToFix(placeAt.Item1, placeAt.Item2);
            FixRails();
        }

        private List<RailBlock> GetAllAdjascentRailBlocks((Vector3Int, Direction8way) position, Rail railAtPosition, Rail railAtOpposite)
        {
            List<RailBlock> adjascentRailBlocks = new List<RailBlock>();
            List<(Vector3Int, Direction8way)> adjascentTuples = new List<(Vector3Int, Direction8way)>();

            adjascentTuples.AddRange(railAtPosition.GetNeighbourTuples().Select(x => (x.Item1, DirectionHelper.Opposite(x.Item2))).ToList());
            adjascentTuples.AddRange(railAtOpposite.GetNeighbourTuples().Select(x => (x.Item1, DirectionHelper.Opposite(x.Item2))).ToList());
            adjascentTuples.AddRange(PlacementManager.GetRailsAtPosition(position.Item1).Select(x => (x.Position, x.Direction)).ToList());

            foreach (var tuple in adjascentTuples)
            {
                RailBlock block = PlacementManager.GetRailAt(tuple).myRailblock;
                if (block != null)
                    adjascentRailBlocks.Add(block);
            }

            return adjascentRailBlocks;
        }

        private void InstantiateRailPrefab((Vector3Int, Direction8way) position, bool isTemp = false)
        {
            if (isTemp == false)
            {
                if (PlacementManager.IsEmpty(position))
                {
                    GameObject newObject = Instantiate(railPrefab, position.Item1, Quaternion.Euler(DirectionHelper.ToEuler(position.Item2)), railParent) as GameObject;
                    Rail newRail = newObject.GetComponent<Rail>();
                    newRail.Init(position.Item1, position.Item2);

                    (Vector3Int, Direction8way) frontPos = (position.Item1 + DirectionHelper.ToDirectionalVector(position.Item2), position.Item2);
                    (Vector3Int, Direction8way) rearPos = (position.Item1 - DirectionHelper.ToDirectionalVector(position.Item2), position.Item2);

                    //search nearby rails, if adjascent add to neighbour
                    if (PlacementManager.IsEmpty(frontPos) == false)
                    {
                        newRail.AddNeighbour(frontPos);
                    }
                    if (PlacementManager.IsEmpty(rearPos) == false)
                    {
                        PlacementManager.GetRailAt(rearPos).AddNeighbour(position);
                    }

                    PlacementManager.AddRailAt(position, newRail);
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
            railsToFix.UnionWith(PlacementManager.GetRailAt(position, direction).GetNeighbourTuples());
            railsToFix.UnionWith(PlacementManager.GetRailAt(position, DirectionHelper.Opposite(direction)).GetNeighbourTuples());
        }

        private void FixRails()
        {
            foreach (var railToFix in railsToFix)
            {
                PlacementManager.GetRailAt(railToFix).FixMyModel();
                PlacementManager.GetRailAt(railToFix.Item1, DirectionHelper.Opposite(railToFix.Item2)).FixMyModel();
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
            (Vector3Int, Direction8way) opposite = (railUnderCursor.Position, DirectionHelper.Opposite(railUnderCursor.Direction));

            // if selected position is out of border or empty do nothing
            if (IsPositionOutOfBorder(railUnderCursor.Position) || PlacementManager.IsEmpty(railUnderCursor.Position, railUnderCursor.Direction))
                return;

            AddAdjascentPositionsToRailsToFix(railUnderCursor.Position, railUnderCursor.Direction);


            if (PlacementManager.IsEmpty(cursorPos) == false || PlacementManager.IsEmpty(opposite) == false)
            {
                foreach (var neighbour in PlacementManager.GetRailAt(cursorPos).GetNeighbourTuples())
                {
                    PlacementManager.GetRailAt((neighbour.Item1, DirectionHelper.Opposite(neighbour.Item2))).RemoveNeighbourAt(opposite);
                }

                foreach (var neighbour in PlacementManager.GetRailAt(opposite).GetNeighbourTuples())
                {
                    PlacementManager.GetRailAt((neighbour.Item1, DirectionHelper.Opposite(neighbour.Item2))).RemoveNeighbourAt(cursorPos);
                }
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