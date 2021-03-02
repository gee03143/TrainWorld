using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrainWorld {

    // Rail의 모양을 고치는 클래스 Rail GameObject의 생성
    // 생성된 GameObject들의 레퍼런스는 railObjectManager에 보관됨
    // RailGraph의 정보를 참고함(이웃 데이터)
    //

    public class RailFixer : MonoBehaviour
    {
        public GameObject straight, diagonal, deadend_straight, deadend_diagonal, corner_left, corner_right;

        [SerializeField]
        private RailObjectManager railObjectManager;

        public void FixRailAtPosition(Vector3Int position, Direction direction, List<Vertex> neighbours)
        {
            railObjectManager.ClearGameObjectsAt(position, direction);

            if (neighbours.Count == 1)
            {
                if (DirectionHelper.IsDiagonal(direction))
                    InstantiateRail(railObjectManager, position, direction, deadend_diagonal);
                else
                    InstantiateRail(railObjectManager, position, direction, deadend_straight);
            }
            else
            {
                foreach (Vertex neighbour in neighbours)
                {
                    int manhattanDistance = ManhattanDistance(Vector3Int.RoundToInt(neighbour.Position), position);
                    if (manhattanDistance == 3)
                    {
                        if (DirectionHelper.IsDiagonal(direction)) // if direction is diagonal
                        {
                            continue;   // do nothing
                        }
                        else
                        {
                            if (NeighbourIsAtRight(position, direction, neighbour.Position))
                            {
                                InstantiateRail(railObjectManager, position, direction, corner_right);
                            }
                            else
                            {
                                InstantiateRail(railObjectManager, position, direction, corner_left);
                            }
                        }
                    }
                    else if (manhattanDistance == 2)
                    {
                        if (DirectionHelper.IsDiagonal(direction))// if direction is diagonal
                        {
                            // make diagonal rail
                            InstantiateRail(railObjectManager, position, direction, diagonal);
                        }
                        else
                        {
                            // something gone wrong, do nothing
                            Debug.Log("Something gone wrong on FixRailAtPosition");
                            throw new Exception();
                        }
                    }
                    else if (manhattanDistance == 1)
                    {
                        if (DirectionHelper.IsDiagonal(direction))// if direction is diagonal
                        {
                            // something gone wrong, do nothing
                            Debug.Log("Something gone wrong on FixRailAtPosition");
                            throw new Exception();
                        }
                        else
                        {
                            // make straight rail
                            InstantiateRail(railObjectManager, position, direction, straight);
                        }
                    }
                }
            }

        }

        private bool NeighbourIsAtRight(Vector3Int position1, Direction direction1, Vector3 position2)
        {
            Vector3 straight = DirectionHelper.ToDirectionalVector(direction1);
            Vector3 target = position2 - position1;
            Vector3 crossProduct = Vector3.Cross(straight, target);
            
            return Vector3.Normalize(crossProduct) == Vector3.up;         
        }

        private void InstantiateRail(RailObjectManager railObjectManager, Vector3Int position, Direction direction, GameObject railPrefab)
        {
            GameObject newRail = Instantiate(railPrefab, position, Quaternion.Euler(DirectionHelper.ToEuler(direction))) as GameObject;

           railObjectManager.AddGameObjectAt(position, direction, newRail);
        }

        private int ManhattanDistance(Vector3Int position1, Vector3Int position2)
        {
            return Mathf.Abs(position1.x - position2.x) + Mathf.Abs(position1.z - position2.z);
        }
    }
}