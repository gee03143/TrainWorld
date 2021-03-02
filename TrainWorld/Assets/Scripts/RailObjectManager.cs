using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    // 게임 오브젝트를 관리하는 클래스
    // 해당 (위치, 방향)튜플의 게임오브젝트들을 저장함
    // 게임오브젝트의 레퍼런스 저장

    // 도움을 주는 클래스 : RailFixer
    public class RailObjectManager : MonoBehaviour
    {
        private Dictionary<(Vector3Int, Direction), List<GameObject>> railObjects;

        private void Awake()
        {
            railObjects = new Dictionary<(Vector3Int, Direction), List<GameObject>>();
        }

        public void AddGameObjectAt(Vector3Int position, Direction direction, GameObject objects)
        {
            if (this.railObjects.ContainsKey((position, direction)) == false)
            {
                this.railObjects[(position, direction)] = new List<GameObject>(); ;
            }

            this.railObjects[(position, direction)].Add(objects);
        }

        public void AddGameObjectsAt(Vector3Int position, Direction direction, List<GameObject> objects)
        {
            if (this.railObjects.ContainsKey((position, direction)) == false)
            {
                this.railObjects[(position, direction)] = new List<GameObject>(); ;
            }

            this.railObjects[(position, direction)].AddRange(objects);
        }

        public List<GameObject> GetGameObjectsAt(Vector3Int position, Direction direction)
        {

            if (railObjects.ContainsKey((position, direction)))
            {
                return railObjects[(position, direction)];
            }
            else
            {
                return new List<GameObject>() ;
            }
        }

        public void ClearGameObjectsAt(Vector3Int position, Direction direction)
        {
            if(railObjects.ContainsKey((position, direction)))
            {
                foreach (GameObject obj in railObjects[(position, direction)])
                {
                    Destroy(obj);
                }
                railObjects[(position, direction)].Clear();
                railObjects.Remove((position, direction));
            }
        }

        public List<(Vector3Int, Direction)> GetNeighbours(Vector3Int position, Direction direction)
        {
            List<(Vector3Int, Direction)> neighbours = new List<(Vector3Int, Direction)>();
            Vector3Int frontPos = position + DirectionHelper.ToDirectionalVector(direction);
            Vector3Int leftPos = position + DirectionHelper.ToDirectionalVector(direction) + DirectionHelper.ToDirectionalVector(DirectionHelper.Prev(direction));
            Vector3Int rightPos = position + DirectionHelper.ToDirectionalVector(direction) + DirectionHelper.ToDirectionalVector(DirectionHelper.Next(direction));

            if (IsRailAtPosition(frontPos, DirectionHelper.Opposite(direction)))    // neighbour at front position
            {
                neighbours.Add((frontPos, DirectionHelper.Opposite(direction)));
            }
            if (IsRailAtPosition(leftPos, DirectionHelper.Opposite(DirectionHelper.Prev(direction))))
            {
                neighbours.Add((leftPos, DirectionHelper.Opposite(DirectionHelper.Prev(direction))));
            }
            if (IsRailAtPosition(rightPos, DirectionHelper.Opposite(DirectionHelper.Next(direction))))
            {
                neighbours.Add((rightPos, DirectionHelper.Opposite(DirectionHelper.Next(direction))));
            }
            return neighbours;
        }

        public bool IsRailAtPosition(Vector3Int position, Direction direction)
        {
            return GetGameObjectsAt(position, direction).Count > 0;
        }
    }
}
