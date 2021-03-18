using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TrainWorld.Rail
{

    // Rail의 모양을 고치는 클래스 Rail GameObject의 생성
    // 생성된 GameObject들의 레퍼런스는 railObjectManager에 보관됨
    // RailGraph의 정보를 참고함(이웃 데이터)
    //

    public class RailModelManager : MonoBehaviour
    {
        private Dictionary<(Vector3Int, Direction8way), RailModel> railModels;
        private Dictionary<(Vector3Int, Direction8way), RailModel> tempRailModels;

        private void Awake()
        {
            railModels = new Dictionary<(Vector3Int, Direction8way), RailModel>();
            tempRailModels = new Dictionary<(Vector3Int, Direction8way), RailModel>();
        }

        public RailModel GetModelAt(Vector3Int position, Direction8way direction)
        {
            if (this.railModels.ContainsKey((position, direction)) == false)
            {
                Debug.Log("Don't have such key " + position.ToString() + direction.ToString());
                return null;
            }
            return railModels[(position, direction)];
        }

        public RailModel GetRailModelViaMousePosition(Vector3 mousePosition)
        {
            Vector3Int roundedPosition = Vector3Int.RoundToInt(mousePosition);
            List<(Vector3Int, Direction8way)> modelsAtPosition = railModels.Keys.Where(x => roundedPosition == x.Item1).ToList();

            if (modelsAtPosition.Count > 0)
            {
                Direction8way nearestDirection = modelsAtPosition[0].Item2;
                float shortestDistance = Vector3.Distance(modelsAtPosition[0].Item1 + Vector3.Normalize(DirectionHelper.ToDirectionalVector(modelsAtPosition[0].Item2)),
                    mousePosition);
                foreach (var model in modelsAtPosition)
                {
                    float newDistance = Vector3.Distance(model.Item1 + Vector3.Normalize(DirectionHelper.ToDirectionalVector(model.Item2)), mousePosition);
                    if (shortestDistance > newDistance)
                    {
                        shortestDistance = newDistance;
                        nearestDirection = model.Item2;
                    }
                }
                return railModels[(roundedPosition, nearestDirection)];
            }
            else
            {
                return null;
            }
        }

        public List<RailModel> GetRailModelsVisMousePosition(Vector3 mousePosition)
        {
            Vector3Int roundedPosition = Vector3Int.RoundToInt(mousePosition);
            List<(Vector3Int, Direction8way)> modelsAtPosition = railModels.Keys.Where(x => roundedPosition == x.Item1).ToList();

            List<RailModel> models = new List<RailModel>();
            foreach (var position in modelsAtPosition)
            {
                models.Add(railModels[position]);
            }

            return models;
        }

        public void AddModelAt(Vector3Int position, Direction8way direction, Rail newRail)
        {
            if (this.railModels.ContainsKey((position, direction)) == false)
            {
                this.railModels[(position, direction)] = newRail.GetModel(false);
            }
            if (this.railModels.ContainsKey((position, DirectionHelper.Opposite(direction))) == false)
            {
                this.railModels[(position, DirectionHelper.Opposite(direction))] = newRail.GetModel(true);
            }
        }

        public void RemoveModelAt(Vector3Int position, Direction8way direction)
        {
            if (this.railModels.ContainsKey((position, direction)))
            {
                this.railModels[(position, direction)].DestroyMyself();
                this.railModels.Remove((position, direction));
            }
            if (this.railModels.ContainsKey((position, DirectionHelper.Opposite(direction))))
            {
                this.railModels[(position, DirectionHelper.Opposite(direction))].DestroyMyself();
                this.railModels.Remove((position, DirectionHelper.Opposite(direction)));
            }
        }

        public void RemoveTempModels()
        {
            foreach (var pos in tempRailModels.Keys)
            {
                tempRailModels[pos].DestroyMyself();
            }
            tempRailModels.Clear();
        }


        internal void AddTempModelAt(Vector3Int position, Direction8way direction, Rail newRail)
        {
            if (this.tempRailModels.ContainsKey((position, direction)) == false)
            {
                this.tempRailModels[(position, direction)] = newRail.GetModel(false);
            }
            if (this.tempRailModels.ContainsKey((position, DirectionHelper.Opposite(direction))) == false)
            {
                this.tempRailModels[(position, DirectionHelper.Opposite(direction))] = newRail.GetModel(true);
            }
        }

        public void FixRailAtPosition(Vector3Int position, Direction8way direction, List<Vertex> neighbours, bool isTempObjects = false)
        {
            if (isTempObjects)
            {
                if (tempRailModels.ContainsKey((position, direction)) == false)
                {
                    return;
                }
                tempRailModels[(position, direction)].FixModel(neighbours);
            }
            else
            {
                if (railModels.ContainsKey((position, direction)) == false)
                {
                    return;
                }
                railModels[(position, direction)].FixModel(neighbours);
            }
        }
    }
}