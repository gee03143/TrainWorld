using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using TrainWorld.Station;
using TrainWorld.Rails;
using TrainWorld.Traffic;

namespace TrainWorld.AI
{
    public class AgentTask
    {
        public Action onFinish;
        public AiAgent responsible;
        public AgentTaskType taskType;

        public virtual void DoTask()
        {

        }

        public virtual void OnTaskEnter()
        {

        }
    }

    public class MoveToStationTask : AgentTask
    {
        public Action<Vector3Int, Direction8way> onNextRail;

        public TrainStation targetStation;
        RailBlock lastBlock;

        List<(Vector3Int, Direction8way)> path;

        int pathIndex = 0;
        (Vector3Int, Direction8way) currentTarget;

        public MoveToStationTask(AiAgent responsible, TrainStation targetStation)
        {
            this.taskType = AgentTaskType.Move;
            this.responsible = responsible;
            this.targetStation = targetStation;
        }

        public override void OnTaskEnter()
        {
            SetUpPath();
        }

        private void SetUpPath()
        {
            path = PlacementManager.GetRailPathForAgent(responsible.Position, responsible.Direction,
                targetStation.Position, targetStation.Direction);
            if (path == null)
            {
                Debug.Log("No Path");
            }
            pathIndex = 0;
            currentTarget = path[pathIndex];
            Debug.Log(currentTarget.ToString());
        }

        public override void DoTask()
        {
            if (path == null)
                SetUpPath();


            Rail rail = PlacementManager.GetRailAt(responsible.Position, responsible.Direction);
            if (lastBlock != rail.myRailblock && lastBlock != null) // if it moved to next block
            {
                lastBlock.hasAgent = false;
            }
            lastBlock = rail.myRailblock;
            lastBlock.hasAgent = true;

            TimeStepping();
        }

        private void TimeStepping()
        {
            float remainingDistance = float.MaxValue;
            if (path.Count > pathIndex)
            {
                Rail agentRail = PlacementManager.GetRailAt(responsible.Position, responsible.Direction); // 이 agent가 올라가 있는 rail
                Rail targetRail = PlacementManager.GetRailAt(currentTarget.Item1, currentTarget.Item2); // agent가 이동할 예정인 rail
                if (targetRail.myRailblock.hasAgent && agentRail.myRailblock != targetRail.myRailblock)
                //다음 railblock으로 이동할 때 해당 railblock이 agent가 존재한다면
                {
                    return;
                }
                else
                {
                    remainingDistance = responsible.MoveAgent(currentTarget);
                }
                if (remainingDistance < 0.1f)
                {
                    onNextRail.Invoke(currentTarget.Item1, currentTarget.Item2);
                    // step to next position
                    pathIndex++;
                    if (pathIndex >= path.Count)
                    {
                        onFinish.Invoke();
                        pathIndex = 0;
                        return;
                    }
                    currentTarget = path[pathIndex];
                }
            }
        }
    }

    public class WaitTask : AgentTask
    {
        public float waitTime;
        float deltaTime;

        public override void OnTaskEnter()
        {
            deltaTime = 0;
        }
        public WaitTask(AiAgent responsible, float waitTime)
        {
            this.taskType = AgentTaskType.Wait;
            this.responsible = responsible;
            this.waitTime = waitTime;
            deltaTime = 0.0f;
        }

        public override void DoTask()
        {
            deltaTime += Time.deltaTime;

            if(deltaTime > waitTime)
            {
                deltaTime = 0.0f;
                onFinish.Invoke();
            }
        }
    }
}
