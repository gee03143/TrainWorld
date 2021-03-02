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

        private HashSet<Vertex> railsToFix;

        private bool placementMode = false;
        private Vector3Int placementStartPosition;
        private Direction placementStartDirection;

        private void Awake()
        {
            railGraph = new RailGraph();
            railsToFix = new HashSet<Vertex>();
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
                    int cpSign = DecideRailDirection(placementStartPosition, position + DirectionHelper.ToDirectionalVector(placementStartDirection), position);
                    Direction nextDirection = placementStartDirection;
                    Vector3Int nextPosition = placementStartPosition + DirectionHelper.ToDirectionalVector(placementStartDirection);
                    if (cpSign == 0)    // 정면에 레일 배치
                    {
                        nextDirection = placementStartDirection;
                        nextPosition = placementStartPosition + DirectionHelper.ToDirectionalVector(placementStartDirection);
                    }
                    else if(cpSign == 1)    // 오른쪽에 레일 배치
                    {
                        nextDirection = DirectionHelper.Next(placementStartDirection);
                        nextPosition = placementStartPosition + DirectionHelper.ToDirectionalVector(placementStartDirection)
                            + DirectionHelper.ToDirectionalVector(nextDirection);
                    }
                    else if(cpSign == -1)  // 왼쪽에 레일 배치
                    {
                        nextDirection = DirectionHelper.Prev(placementStartDirection);
                        nextPosition = placementStartPosition + DirectionHelper.ToDirectionalVector(placementStartDirection)
                            + DirectionHelper.ToDirectionalVector(nextDirection);
                    }
                    AddRailAt(nextDirection, nextPosition);
                    placementMode = false;
                }
                else
                {

                }
            }
            else
            {
                if (isPositionEmpty(position))
                {
                    railGraph.AddVertexAt(position, railCursor.CursorDirection);

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

        private void AddRailAt(Direction nextDirection, Vector3Int nextPosition)
        {
            railGraph.AddVertexAt(nextPosition, nextDirection);
            if(placementStartDirection != nextDirection)
                railGraph.AddEdge(placementStartPosition, nextPosition, placementStartDirection, DirectionHelper.Opposite(nextDirection));

            railsToFix.Add(new Vertex(nextPosition, nextDirection));
            railsToFix.Add(new Vertex(nextPosition, DirectionHelper.Opposite(nextDirection)));
            railsToFix.UnionWith(railGraph.GetNeighboursAt(nextPosition, nextDirection));
            railsToFix.UnionWith(railGraph.GetNeighboursAt(nextPosition, DirectionHelper.Opposite(nextDirection)));
            FixRails();
        }

        private int DecideRailDirection(Vector3Int a, Vector3Int b, Vector3Int c)
        {
            // AB와 AC사이의 cross product를 이용해 점 C의 AB로부터의 상대적인 방향을 알 수 있다.
            // cross Product Between AB and AC
            Vector3 cp = Vector3.Normalize(Vector3.Cross(b - a, c - a));
            if (cp == Vector3.up)
            {
                return 1;   // C가 AB의 오른쪽에 존재
            }else if(cp == Vector3.down)
            {
                return -1; // C가 AB의 왼쪽에 존재
            }
            else if(cp == Vector3.zero)
            {
                return 0; // AC가 AB와 평행
            }
            else
            {
                // something wrong happen
                Debug.Log("something wrong happen at GetCrossProductSign");
                throw new ArithmeticException();
            }
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

            }
            else
            {
                railCursor.Move(position);
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