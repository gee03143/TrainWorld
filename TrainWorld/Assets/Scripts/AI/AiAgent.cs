using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TrainWorld.Station;
using TrainWorld.Rails;

namespace TrainWorld.AI
{
    [RequireComponent(typeof(Rigidbody))]
    public class AiAgent : MonoBehaviour, ISelectableObject
    {
        [SerializeField]
        private Vector3Int position;

        public Vector3Int Position
        {
            get { return position; }
            private set { position = value; }
        }

        [SerializeField]
        private Direction8way direction;

        public Direction8way Direction
        {
            get { return direction; }
            private set { direction = value; }
        }

        public event Action OnDeath;
        Rigidbody rb;

        [SerializeField]
        private float speed = 1.0f;
        [SerializeField]
        private float rotationSpeed = 10.0f;

        public bool loop;

        private bool move;

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

        public List<TrainStation> trainStationsInSchedule;
        int stationIndex;

        List<(Vector3Int, Direction8way)> path;

        int pathIndex;
        (Vector3Int, Direction8way) currentTarget;

        internal void Init(Vector3Int position, Direction8way direction)
        {
            this.position = position;
            this.direction = direction;
        }

        public void SetUpSchedule(List<TrainStation> schedule)
        {

            pathIndex = 0;
            stationIndex = 0;
            trainStationsInSchedule = schedule;
            move = true;
            if(trainStationsInSchedule.Count > 1)
            {
                SetUpPath(stationIndex);
            }
        }

        private void SetUpPath(int nextStationIndex)
        {
            path = PlacementManager.GetRailPathForAgent(this.position, this.direction,
                trainStationsInSchedule[stationIndex].Position, trainStationsInSchedule[stationIndex].Direction);
            if(path == null)
            {
                Debug.Log("No Path");
            }
            pathIndex = 0;
            currentTarget = path[pathIndex];
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            path = new List<(Vector3Int, Direction8way)>();
            trainStationsInSchedule = new List<TrainStation>();
            stationIndex = 0;
            pathIndex = 0;
        }

        private void Update()
        {
            if(move)
            {
                TimeStepping();
            }
        }

        private void TimeStepping()
        {
            if(path.Count > pathIndex)
            {
                float remainingDistance = MoveAgent();
                if(remainingDistance < 0.1f)
                {
                    this.Position = currentTarget.Item1;
                    this.Direction = currentTarget.Item2;
                    // step to next position
                    pathIndex++;
                    if(pathIndex >= path.Count)
                    {
                        //search new path
                        int nextStationIndex = (stationIndex + 1) % trainStationsInSchedule.Count;
                        move = false;
                        SetUpPath(nextStationIndex);
                        move = true;
                        stationIndex = nextStationIndex;
                    }
                    currentTarget = path[pathIndex];
                }
            }
        }

        private float MoveAgent()
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.Item1, step);
            rb.velocity = transform.forward * speed;

            Vector3 lookDirection = currentTarget.Item1 - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * rotationSpeed);
            return Vector3.Distance(transform.position, currentTarget.Item1);
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
