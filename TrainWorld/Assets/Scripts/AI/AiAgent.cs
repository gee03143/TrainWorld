using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TrainWorld.Rails;
using TrainWorld.Traffic;
using TrainWorld.Buildings;

namespace TrainWorld.AI
{
    [RequireComponent(typeof(Rigidbody))]
    public class AiAgent : MonoBehaviour, ISelectableObject
    {
        public Storage storage;

        private TrainStation currentStation;

        public event Action OnDeath;
        Rigidbody rb;

        [SerializeField]
        private float speed = 1.0f;
        [SerializeField]
        private float rotationSpeed = 10.0f;


        [SerializeField]
        private Vector3Int position;

        public Vector3Int Position
        {
            get { return position; }
            private set {          
                position = value;
            }
        }

        [SerializeField]
        private Direction8way direction;

        public Direction8way Direction
        {
            get { return direction; }
            private set {
                direction = value;
            }
        }

        private bool stop;

        public bool Stop
        {
            get { return stop; }
            set
            {
                stop = value;
                if (stop)
                {
                    rb.velocity = Vector3.zero;
                }
            }
        }

        public List<(TrainStation, DepartureConditionType)> schedules;

        //for move status
        int scheduleIndex;
        int pathIndex;
        (Vector3Int, Direction8way) currentTarget;
        List<(Vector3Int, Direction8way)> path;
        RailBlock lastBlock;

        //for wait status(waiting until departure condition accomplished

        internal void Init(Vector3Int position, Direction8way direction)
        {
            this.Position = position;
            this.Direction = direction;
            PlacementManager.GetRailAt(position, direction).myRailblock.hasAgent = true;
            storage = gameObject.GetComponent<Storage>();
            schedules = new List<(TrainStation, DepartureConditionType)>();
        }



        public void SetUpSchedule(List<(TrainStation, DepartureConditionType)> newSchedule)
        {
            scheduleIndex = 0;

            if (schedules != null)
                schedules.Clear();

            schedules = newSchedule;
            Stop = false;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void SetUpPath()
        {
            TrainStation destination = schedules[scheduleIndex].Item1;
            if (destination == null)
            {
                Debug.Log("target station : " + destination.StationName + " is missing");
                path = new List<(Vector3Int, Direction8way)>();
                return;
            }
            path = PlacementManager.GetRailPathForAgent(Position, Direction,
                destination.Position, destination.Direction);
            if (path == null)
            {
                Debug.Log("No Path");
            }
            pathIndex = 0;
            currentTarget = path[pathIndex];
        }

        void Update()
        {
            if (schedules.Count == 0)
                return;

            if (Stop)
            {
                if (IsDepartureConditionAccomplished())
                {
                    SetToNextSchedule();
                    currentStation.inserter.StopInserterCoroutine();
                    Stop = false;
                }
    
            }
            else
            {
                if (path == null)
                    SetUpPath();

                Rail rail = PlacementManager.GetRailAt(Position, Direction);
                if (lastBlock != rail.myRailblock && lastBlock != null) // if it moved to next block
                {
                    lastBlock.hasAgent = false;
                }
                lastBlock = rail.myRailblock;
                lastBlock.hasAgent = true;

                TimeStepping();
            }
        }

        private void SetToNextSchedule()
        {
            scheduleIndex = (scheduleIndex + 1) % schedules.Count;
            SetUpPath();
        }

        //for waiting departure condition
        private bool IsDepartureConditionAccomplished()
        {
            DepartureConditionType departureCondition = schedules[scheduleIndex].Item2;

            if(departureCondition == DepartureConditionType.Load)
            {
                return storage.IsFull();
            }else if(departureCondition == DepartureConditionType.Unload)
            {
                return storage.IsEmpty();
            }
            else
            {
                return true;
            }
        }

        private void TimeStepping()
        {
            float remainingDistance = float.MaxValue;
            if (path.Count > pathIndex)
            {
                Rail agentRail = PlacementManager.GetRailAt(Position, Direction); // 이 agent가 올라가 있는 rail
                Rail targetRail = PlacementManager.GetRailAt(currentTarget.Item1, currentTarget.Item2); // agent가 이동할 예정인 rail
                if (targetRail.myRailblock.hasAgent && agentRail.myRailblock != targetRail.myRailblock)
                //다음 railblock으로 이동할 때 해당 railblock이 agent가 존재한다면
                {
                    return;
                }
                else
                {
                    remainingDistance = MoveAgent(currentTarget);
                }
                if (remainingDistance < 0.1f)
                {
                    ChangePositionAndDirection(currentTarget.Item1, currentTarget.Item2);
                    // step to next position
                    pathIndex++;
                    if (pathIndex >= path.Count)
                    {
                        SwitchToStopStatus();
                        stop = true;

                        pathIndex = 0;
                        return;
                    }
                    currentTarget = path[pathIndex];
                }
            }
        }

        private void SwitchToStopStatus()
        {
            currentStation = schedules[scheduleIndex].Item1;
            if(schedules[scheduleIndex].Item2 == DepartureConditionType.Load)
            {
                currentStation.inserter.AddReceiver(storage);
                currentStation.inserter.AddProviders(currentStation.connectedStorage);
                currentStation.inserter.StartInserterCoroutine();
            }else if (schedules[scheduleIndex].Item2 == DepartureConditionType.Unload)
            {
                currentStation.inserter.AddProvider(storage);
                currentStation.inserter.AddReceivers(currentStation.connectedStorage);
                currentStation.inserter.StartInserterCoroutine();
            }
        }

        private float MoveAgent((Vector3Int, Direction8way) target)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.Item1, step);
            rb.velocity = transform.forward * speed;

            Vector3 lookDirection = target.Item1 - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * rotationSpeed);
            return Vector3.Distance(transform.position, target.Item1);
        }

        private void ChangePositionAndDirection(Vector3Int position, Direction8way direction)
        {
            Position = position;
            Direction = direction;
        }


        private void OnDrawGizmosSelected()
        {
            if (path == null)
                return;

            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i].Item1 + new Vector3(0,1,0), path[i + 1].Item1 + new Vector3(0, 1, 0), Color.red);
            }

        }

        private void OnDestroy()
        {
            PlacementManager.GetRailAt((Position, Direction)).myRailblock.hasAgent = false;
            OnDeath?.Invoke();
        }

        public SelectableObjectType GetSelectableObjectType()
        {
            return SelectableObjectType.Agent;
        }

        public void ShowMyUI()
        {
            Debug.Log(transform.gameObject.name);
        }
    }
}
