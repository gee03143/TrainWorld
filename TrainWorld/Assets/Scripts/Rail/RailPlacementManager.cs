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
        private RailModelManager railModelManager;

        [SerializeField]
        RailArrowUI railArrowUI;

        [SerializeField]
        private GameObject railPrefab;
        [SerializeField]
        private Transform railParent;

        public RailGraph railGraph;

        private HashSet<Vertex> railsToFix;

        private List<Vertex> tempRailVertices;

        private Dictionary<(Vector3Int, Direction8way), Rail> rails;
        private List<Rail> tempRails;

        private bool placementMode = false;
        private Vector3Int placementStartPosition;
        private Direction8way placementStartDirection;

        private RailModel modelUnderCursor;

        private void Awake()
        {
            railGraph = new RailGraph();
            railsToFix = new HashSet<Vertex>();
            tempRailVertices = new List<Vertex>();
            rails = new Dictionary<(Vector3Int, Direction8way), Rail>();
            tempRails = new List<Rail>();
            placementStartDirection = Direction8way.N;
        }

        internal List<Rail> GetRailsAtPosition(Vector3Int position)
        {
            List<Rail> railsAtPosition = rails.Where(x => position == x.Key.Item1).Select(x => x.Value).ToList();

            return railsAtPosition;
        }

        private bool isPositionEmpty(Vector3Int position, Direction8way direction)
        {
            return railGraph.GetVertexAt(position, direction) == null;
        }

        private bool IsPositionOutOfBorder(Vector3Int position)
        {
            return position.x < 0 || position.x > maxWidth || position.z < 0 || position.z > maxHeight;
        }

        internal void PlaceRail(Vector3 position)
        {
            Vector3Int roundedPosition = Vector3Int.RoundToInt(position);

            //해당 위치가 경계 바깥이면 종료 
            if (IsPositionOutOfBorder(roundedPosition))
                return;

            if (placementMode)
            {
                CreateRailAtTempRailVertices();
                RemoveTempRails();
                tempRailVertices.Clear();
                placementMode = false;
            }
            else
            {
                if (modelUnderCursor == null)
                {
                    railGraph.AddVertexAtPosition(roundedPosition, placementStartDirection);

                    InstantiateRailPrefab(roundedPosition, placementStartDirection);

                    AddPositionToRailsToFix(roundedPosition, placementStartDirection);
                    AddAdjascentPositionsToRailsToFix(roundedPosition, placementStartDirection);

                    FixRails();
                }
                else
                {
                    placementMode = true;
                    placementStartPosition = modelUnderCursor.Position;
                    placementStartDirection = modelUnderCursor.Direction;
                }
            }
        }

        public void DestroyRail(Vector3 position)
        {
            // if selected position is out of border or empty do nothing
            if (IsPositionOutOfBorder(modelUnderCursor.Position) || isPositionEmpty(modelUnderCursor.Position, modelUnderCursor.Direction))
                return;
            if (railGraph.GetVertexAt(modelUnderCursor.Position, modelUnderCursor.Direction) == null)
                return;

            AddAdjascentPositionsToRailsToFix(modelUnderCursor.Position, modelUnderCursor.Direction);
            railGraph.DeleteVertexAtPosition(modelUnderCursor.Position, modelUnderCursor.Direction);

            Direction8way direction4 = DirectionHelper.ToDirection4Way(modelUnderCursor.Direction);
            Direction8way opposite = DirectionHelper.Opposite(direction4);
            if (rails.ContainsKey((modelUnderCursor.Position, modelUnderCursor.Direction)) || rails.ContainsKey((modelUnderCursor.Position, opposite)))
            {
                railModelManager.RemoveModelAt(modelUnderCursor.Position, modelUnderCursor.Direction);
                rails[(modelUnderCursor.Position, direction4)].DestroyMyself();
                rails.Remove((modelUnderCursor.Position, direction4));
            }

            FixRails();
        }

        private void CreateRailAtTempRailVertices()
        {
            Vertex last = null;
            foreach (var pos in tempRailVertices)
            {
                if (last != null)
                    AddRailAt(last.Direction, Vector3Int.RoundToInt(last.Position), pos.Direction, Vector3Int.RoundToInt(pos.Position));
                else
                    AddRailAt(pos.Direction, Vector3Int.RoundToInt(pos.Position), pos.Direction, Vector3Int.RoundToInt(pos.Position));
                last = pos;
            }
        }

        private void AddRailAt(Direction8way startDirection, Vector3Int startPosition, Direction8way nextDirection, Vector3Int nextPosition)
        {
            railGraph.AddVertexAtPosition(nextPosition, nextDirection);
            railGraph.AddEdge(startPosition, nextPosition, startDirection, nextDirection);
            railGraph.AddEdge(startPosition, nextPosition, DirectionHelper.Opposite(startDirection), DirectionHelper.Opposite(nextDirection));
            InstantiateRailPrefab(nextPosition, nextDirection);

            AddPositionToRailsToFix(nextPosition, nextDirection);
            AddAdjascentPositionsToRailsToFix(nextPosition, nextDirection);
            FixRails();
        }

        private void InstantiateRailPrefab(Vector3Int position, Direction8way direction, bool isTemp = false)
        {
            Direction8way direction4 = DirectionHelper.ToDirection4Way(direction);
            if (isTemp == false)
            {
                if (rails.ContainsKey((position, direction4)) == false)
                {
                    GameObject newObject = Instantiate(railPrefab, position, Quaternion.Euler(DirectionHelper.ToEuler(direction4)), railParent) as GameObject;
                    Rail newRail = newObject.GetComponent<Rail>();
                    newRail.Init(position, direction4);
                    rails.Add((position, direction4), newRail);
                    railModelManager.AddModelAt(position, direction4, newRail);
                }
            }
            else
            {
                GameObject newObject = Instantiate(railPrefab, position, Quaternion.Euler(DirectionHelper.ToEuler(direction4)), railParent) as GameObject;
                Rail newRail = newObject.GetComponent<Rail>();
                newRail.Init(position, direction4);
                tempRails.Add(newRail);
                railModelManager.AddTempModelAt(position, direction4, newRail);
            }
        }

        private void AddPositionToRailsToFix(Vector3Int position, Direction8way direction)
        {
            railsToFix.Add(new Vertex(position, direction));
            railsToFix.Add(new Vertex(position, DirectionHelper.Opposite(direction)));
        }

        private void AddAdjascentPositionsToRailsToFix(Vector3Int position, Direction8way direction)
        {
            railsToFix.UnionWith(railGraph.GetNeighboursAt(position, direction));
            railsToFix.UnionWith(railGraph.GetNeighboursAt(position, DirectionHelper.Opposite(direction)));
        }

        private void FixRails()
        {
            foreach (var vertex in railsToFix)
            {
                railModelManager.FixRailAtPosition(Vector3Int.RoundToInt(vertex.Position), vertex.Direction,
                    railGraph.GetNeighboursAt(vertex.Position, vertex.Direction));
            }

            railsToFix.Clear();
        }

        private void RemoveTempRails()
        {
            foreach (var tempRail in tempRails)
            {
                Destroy(tempRail.gameObject);
            }
            tempRails.Clear();
            railModelManager.RemoveTempModels();
        }

        internal void MoveCursorAtRailPlacement(Vector3 cursorPosition)
        {
            Vector3Int roundedPosition = Vector3Int.RoundToInt(cursorPosition);
            modelUnderCursor = railModelManager.GetRailModelViaMousePosition(cursorPosition);
            ShowRailUI(cursorPosition);
            if (placementMode)
            {
                //clear temp rails
                RemoveTempRails();
                tempRailVertices.Clear();
                tempRailVertices = RailGraphPathfinder.AStarSearch(placementStartPosition, placementStartDirection, roundedPosition);

                //place temp rails
                CreateTempRailModel();
            }
            else
            {
                placementStartPosition = roundedPosition;

                RemoveTempRails();
                tempRailVertices.Clear();
                if (modelUnderCursor == null)
                {
                    tempRailVertices.Add(new Vertex(placementStartPosition, placementStartDirection));

                    CreateTempRailModel();
                }
            }
        }

        internal void MoveCursorAtDestruction(Vector3 cursorPosition)
        {
            modelUnderCursor = railModelManager.GetRailModelViaMousePosition(cursorPosition);
            ShowRailUI(cursorPosition);
        }

        private void ShowRailUI(Vector3 cursorPosition)
        {
            if (modelUnderCursor != null)
            {
                railArrowUI.arrow.SetActive(true);
                railArrowUI.arrow.transform.position = modelUnderCursor.Position;

                railArrowUI.arrow.transform.rotation = Quaternion.Euler(DirectionHelper.ToEuler(modelUnderCursor.Direction));
            }
            else
            {
                railArrowUI.arrow.SetActive(false);
            }
        }

        private void CreateTempRailModel()
        {
            for (int i = 0; i < tempRailVertices.Count; i++)
            {
                List<Vertex> neighbour = new List<Vertex>();

                if (i != 0)
                    neighbour.Add(tempRailVertices[i - 1]);
                if (i != tempRailVertices.Count - 1)
                    neighbour.Add(tempRailVertices[i + 1]);
                if (railGraph.GetVertexAt(tempRailVertices[i].Position, tempRailVertices[i].Direction) != null)
                    neighbour.AddRange(railGraph.GetNeighboursAt(tempRailVertices[i].Position, tempRailVertices[i].Direction));

                InstantiateRailPrefab(Vector3Int.RoundToInt(tempRailVertices[i].Position), tempRailVertices[i].Direction, true);

                railModelManager.FixRailAtPosition(Vector3Int.RoundToInt(tempRailVertices[i].Position), tempRailVertices[i].Direction, neighbour, true);
                railModelManager.FixRailAtPosition(Vector3Int.RoundToInt(tempRailVertices[i].Position),
                    DirectionHelper.Opposite(tempRailVertices[i].Direction), neighbour, true);
            }
        }

        internal void RotateCursor()
        {
            if (placementMode == false)
            {
                placementStartDirection = DirectionHelper.Next(placementStartDirection);
                RemoveTempRails();
                tempRailVertices.Clear();
                tempRailVertices.Add(new Vertex(placementStartPosition, placementStartDirection));
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
            tempRailVertices.Clear();
            railArrowUI.arrow.SetActive(false);
            placementMode = false;
            Debug.Log("Rail Placement Exit");
        }
    }
}