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
                this.railObjects[(position, direction)] = new List<GameObject>(); 
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
    }
}
