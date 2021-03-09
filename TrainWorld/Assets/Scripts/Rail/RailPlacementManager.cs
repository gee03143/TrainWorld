﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TrainWorld
{
    // Rail 배치에 대한 입력 정의
    // Dictionary에 새 Rail이 입력되도록 함
    // railGraph에 새로 입력된 Node, Edge를 전달함
    // railObjectManager에 새 railObject를 저장하도록 함
    // 


    // TODO : 복수의 레일을 한꺼번에 배치, a* 알고리즘을 사용할 것
    // placementmode에서 뒤 방향으로 커서가 이동할 경우 커서를 표시하지 않도록 만들기
    // 8방향 Direction을 4방향으로 줄이는 방법?
    public class RailPlacementManager : MonoBehaviour
    {

        [SerializeField]
        int maxWidth;
        [SerializeField]
        int maxHeight;

        [SerializeField]
        private RailModelManager railModelManager;
        [SerializeField]
        private RailCursor railCursor;

        private RailGraph railGraph;
        private RailGraphPathfinder railGraphPathfinder;

        private HashSet<Vertex> railsToFix;
        private List<Vertex> tempRailVertices;

        private bool placementMode = false;
        private Vector3Int placementStartPosition;
        private Direction8way placementStartDirection;

        private void Awake()
        {
            railGraph = new RailGraph();
            railGraphPathfinder = new RailGraphPathfinder();
            railsToFix = new HashSet<Vertex>();
            tempRailVertices = new List<Vertex>();
        }

        private bool isPositionEmpty(Vector3Int position)
        {
            return railGraph.GetVertexAt(position, railCursor.Direction) == null;
        }

        private bool IsPositionOutOfBorder(Vector3Int position)
        {
            return position.x < 0 || position.x > maxWidth || position.z < 0 || position.z > maxHeight;
        }

        internal void PlaceRail(Vector3Int position)
        {
            //해당 위치가 경계 바깥이면 종료 
            if (IsPositionOutOfBorder(position))
                return;

            if (placementMode)
            {
                CreateRailAtTempRailVertices();
                tempRailVertices.Clear();
                placementMode = false;
            }
            else
            {
                if (isPositionEmpty(position))
                {
                    railGraph.AddVertexAtPosition(position, railCursor.Direction);

                    railModelManager.AddModelAt(position, railCursor.Direction);
                    railModelManager.AddModelAt(position, DirectionHelper.Opposite(railCursor.Direction));
                    AddPositionToRailsToFix(position, railCursor.Direction);
                    AddAdjascentPositionsToRailsToFix(position, railCursor.Direction);

                    FixRails();
                }
                else
                {
                    placementMode = true;
                    placementStartPosition = position;
                    placementStartDirection = railCursor.Direction;
                }
            }
        }

        public void DestroyRail(Vector3Int position)
        {
            // if selected position is out of border or empty do nothing
            if (IsPositionOutOfBorder(position) || isPositionEmpty(position))
                return;

            if (railGraph.GetVertexAt(position, railCursor.Direction) == null)
                return;


            AddAdjascentPositionsToRailsToFix(position, railCursor.Direction);

            railGraph.DeleteVertexAtPosition(position, railCursor.Direction);
            railModelManager.RemoveModelAt(position, railCursor.Direction);
            railModelManager.RemoveModelAt(position, DirectionHelper.Opposite(railCursor.Direction));
            FixRails();
        }

        private void CreateRailAtTempRailVertices()
        {
            Vertex last = null;
            foreach (var pos in tempRailVertices)
            {
                if (last != null)
                    AddRailAt(last.direction, Vector3Int.RoundToInt(last.Position), pos.direction, Vector3Int.RoundToInt(pos.Position));
                last = pos;
            }
        }

        private void AddRailAt(Direction8way startDirection, Vector3Int startPosition, Direction8way nextDirection, Vector3Int nextPosition)
        {
            railGraph.AddVertexAtPosition(nextPosition, nextDirection);
            railGraph.AddEdge(startPosition, nextPosition, startDirection, nextDirection);
            railGraph.AddEdge(startPosition, nextPosition, DirectionHelper.Opposite(startDirection), DirectionHelper.Opposite(nextDirection));
            railModelManager.AddModelAt(nextPosition, nextDirection);
            railModelManager.AddModelAt(nextPosition, DirectionHelper.Opposite(nextDirection));

            AddPositionToRailsToFix(nextPosition, nextDirection);
            AddAdjascentPositionsToRailsToFix(nextPosition, nextDirection);
            FixRails();
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
                railModelManager.FixRailAtPosition(Vector3Int.RoundToInt(vertex.Position), vertex.direction,
                    railGraph.GetNeighboursAt(vertex.Position, vertex.direction));
            }

            railsToFix.Clear();
        }

        internal void MoveCursor(Vector3Int position)
        {
            if (placementMode)
            {
                railCursor.Move(placementStartPosition);
                railCursor.ToggleCursorObject(true);

                //clear temp rails
                tempRailVertices.Clear();
                tempRailVertices = railGraphPathfinder.AStarSearch(placementStartPosition, placementStartDirection, position);
                railModelManager.RemoveTempModels();

                //place temp rails
                for (int i = 0; i < tempRailVertices.Count; i++)
                {
                    List<Vertex> neighbour = new List<Vertex>();

                    if (i != 0)
                        neighbour.Add(tempRailVertices[i - 1]);
                    if (i != tempRailVertices.Count - 1)
                        neighbour.Add(tempRailVertices[i + 1]);
                    if(railGraph.GetVertexAt(tempRailVertices[i].Position, tempRailVertices[i].direction) != null)
                        neighbour.AddRange(railGraph.GetNeighboursAt(tempRailVertices[i].Position, tempRailVertices[i].direction));

                    railModelManager.AddTempModelAt(Vector3Int.RoundToInt(tempRailVertices[i].Position), tempRailVertices[i].direction);
                    railModelManager.AddTempModelAt(Vector3Int.RoundToInt(tempRailVertices[i].Position), 
                        DirectionHelper.Opposite(tempRailVertices[i].direction));

                    railModelManager.FixRailAtPosition(Vector3Int.RoundToInt(tempRailVertices[i].Position), tempRailVertices[i].direction, neighbour, true);
                    railModelManager.FixRailAtPosition(Vector3Int.RoundToInt(tempRailVertices[i].Position), 
                        DirectionHelper.Opposite(tempRailVertices[i].direction), neighbour, true);
                }
            }
            else
            {
                railCursor.Move(position);
                if (isPositionEmpty(position) == false)
                {
                    railCursor.ToggleCursorObject(true);
                }
                else
                {
                    railCursor.ToggleCursorObject(false);
                }
            }
        }

        internal void RotateCursor()
        {
            if(placementMode == false)
                railCursor.Rotate();
        }

        internal void RailPlacementEnter()
        {
        }
    }
}