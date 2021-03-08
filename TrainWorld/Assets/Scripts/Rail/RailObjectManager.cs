using System;
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
        private Dictionary<(Vector3Int, Direction), List<GameObject>> tempRailObjects;

        private void Awake()
        {
            railObjects = new Dictionary<(Vector3Int, Direction), List<GameObject>>();
            tempRailObjects = new Dictionary<(Vector3Int, Direction), List<GameObject>>();
        }

        public void AddGameObjectAt(Vector3Int position, Direction direction, GameObject newObject)
        {
            if (this.railObjects.ContainsKey((position, direction)) == false)
            {
                this.railObjects[(position, direction)] = new List<GameObject>(); ;
            }

            this.railObjects[(position, direction)].Add(newObject);
        }

        internal void AddTempObjectAt(Vector3Int position, Direction direction, GameObject newObject)
        {
            if (this.tempRailObjects.ContainsKey((position, direction)) == false)
            {
                this.tempRailObjects[(position, direction)] = new List<GameObject>(); ;
            }

            this.tempRailObjects[(position, direction)].Add(newObject);
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

        public void ClearTempObjects()
        {
            foreach (var pos in tempRailObjects.Keys)
            {
                foreach (GameObject obj in tempRailObjects[pos])
                {
                    Destroy(obj);
                }
            }
            tempRailObjects.Clear();
        }

        public void ClearGameObjectsAt(Vector3Int position, Direction direction)
        {
            if (railObjects.ContainsKey((position, direction)))
            {
                foreach (GameObject obj in railObjects[(position, direction)])
                {
                    Destroy(obj);
                }
                railObjects[(position, direction)].Clear();
                railObjects.Remove((position, direction));
            }
        }
    }
}
