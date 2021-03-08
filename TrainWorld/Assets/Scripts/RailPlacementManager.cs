using System;
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
        private RailFixer railFixer;
        [SerializeField]
        private RailCursor railCursor;

        private RailGraph railGraph;
        private RailGraphPathfinder railGraphPathfinder;

        private HashSet<Vertex> railsToFix;
        private List<Vertex> tempRailPositions;

        private bool placementMode = false;
        private Vector3Int placementStartPosition;
        private Direction placementStartDirection;

        private void Awake()
        {
            railGraph = new RailGraph();
            railGraphPathfinder = new RailGraphPathfinder();
            railsToFix = new HashSet<Vertex>();
            tempRailPositions = new List<Vertex>();
        }

        private bool isPositionEmpty(Vector3Int position)
        {
            return railGraph.GetVertexAt(position, railCursor.CursorDirection) == null;
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
                if (isPositionEmpty(position))
                {
                    List<Vertex> path = railGraphPathfinder.AStarSearch(placementStartPosition, placementStartDirection, position);
                    Vertex last = null;
                    foreach (var pos in path)
                    {
                        if(last != null)
                            AddRailAt(last.direction, Vector3Int.RoundToInt(last.Position), pos.direction, Vector3Int.RoundToInt(pos.Position));
                        last = pos;
                    }
                    placementMode = false;
                }
            }
            else
            {
                if (isPositionEmpty(position))
                {
                    railGraph.AddVertexAtGridCell(position, railCursor.CursorDirection);

                    railsToFix.Add(new Vertex(position, railCursor.CursorDirection));
                    railsToFix.Add(new Vertex(position, DirectionHelper.Opposite(railCursor.CursorDirection)));
                    railsToFix.UnionWith(railGraph.GetNeighboursAt(position, railCursor.CursorDirection));
                    railsToFix.UnionWith(railGraph.GetNeighboursAt(position, DirectionHelper.Opposite(railCursor.CursorDirection)));

                    FixRails();
                }
                else
                {
                    placementMode = true;
                    placementStartPosition = position;
                    placementStartDirection = railCursor.CursorDirection;
                }
            }
        }

        private void AddRailAt(Direction startDirection, Vector3Int startPosition, Direction nextDirection, Vector3Int nextPosition)
        {
            railGraph.AddVertexAtGridCell(nextPosition, nextDirection);
            railGraph.AddEdge(startPosition, nextPosition, startDirection, nextDirection);
            railGraph.AddEdge(startPosition, nextPosition, DirectionHelper.Opposite(startDirection), DirectionHelper.Opposite(nextDirection));

            railsToFix.Add(new Vertex(nextPosition, nextDirection));
            railsToFix.Add(new Vertex(nextPosition, DirectionHelper.Opposite(nextDirection)));
            railsToFix.UnionWith(railGraph.GetNeighboursAt(nextPosition, nextDirection));
            railsToFix.UnionWith(railGraph.GetNeighboursAt(nextPosition, DirectionHelper.Opposite(nextDirection)));
            FixRails();
        }

        private void FixRails()
        {
            foreach (var vertex in railsToFix)
            {
                railFixer.FixRailAtPosition(Vector3Int.RoundToInt(vertex.Position), vertex.direction,
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