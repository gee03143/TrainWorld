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

        [SerializeField]
        private Transform railFolder;

        public void FixRailAtPosition(Vector3Int position, Direction direction, List<Vertex> neighbours, bool isTempObjects = false)
        {
            railObjectManager.ClearGameObjectsAt(position, direction);

            Vector3Int frontCandidatePos = position + DirectionHelper.ToDirectionalVector(direction);
            Vector3Int leftCandidatePos = frontCandidatePos + DirectionHelper.ToDirectionalVector(DirectionHelper.Prev(direction));
            Vector3Int rightCandidatePos = frontCandidatePos + DirectionHelper.ToDirectionalVector(DirectionHelper.Next(direction));

            if(neighbours.Count == 0)
            {
                if (DirectionHelper.IsDiagonal(direction))
                {
                    InstantiateRail(position, direction, deadend_diagonal, neighbours, isTempObjects);
                }
                else
                {
                    InstantiateRail(position, direction, deadend_straight, neighbours, isTempObjects);
                }
                return;
            }

            bool railCreated = false;
            foreach (Vertex neighbour in neighbours)
            {
                if (Vector3Int.RoundToInt(neighbour.Position).Equals(leftCandidatePos))
                {
                    railCreated = true;
                    if (DirectionHelper.IsDiagonal(direction)) // if direction is diagonal
                    {
                        continue;   // do nothing
                    }
                    else
                    {
                        InstantiateRail(position, direction, corner_left, neighbours, isTempObjects);
                    }
                }
                else if (Vector3Int.RoundToInt(neighbour.Position).Equals(rightCandidatePos))
                {
                    railCreated = true;
                    if (DirectionHelper.IsDiagonal(direction)) // if direction is diagonal
                    {
                        continue;   // do nothing
                    }
                    else
                    {
                        InstantiateRail(position, direction, corner_right, neighbours, isTempObjects);
                    }
                }
                else if (Vector3Int.RoundToInt(neighbour.Position).Equals(frontCandidatePos))
                {
                    railCreated = true;
                    if (DirectionHelper.IsDiagonal(direction))// if direction is diagonal
                    {
                        // make diagonal rail
                        InstantiateRail(position, direction, diagonal, neighbours, isTempObjects);
                    }
                    else
                    {
                        // make straight rail
                        InstantiateRail(position, direction, straight, neighbours, isTempObjects);
                    }
                }
            }

            if(railCreated == false)
            {
                if (DirectionHelper.IsDiagonal(direction))
                {
                    InstantiateRail(position, direction, deadend_diagonal, neighbours, isTempObjects);
                }
                else
                {
                    InstantiateRail(position, direction, deadend_straight, neighbours, isTempObjects);
                }
            }
        }

        private void InstantiateRail(Vector3Int position, Direction direction, GameObject railPrefab, List<Vertex> neighbours, bool isTempObjects)
        {
            GameObject newRail = Instantiate(railPrefab, position, Quaternion.Euler(DirectionHelper.ToEuler(direction)), railFolder.transform) as GameObject;

            newRail.AddComponent<RailGizmo>();
            newRail.GetComponent<RailGizmo>().SetNeighbours(neighbours);

            if (isTempObjects)
                railObjectManager.AddTempObjectAt(position, direction, newRail);
            else
                railObjectManager.AddGameObjectAt(position, direction, newRail);
        }
    }
}