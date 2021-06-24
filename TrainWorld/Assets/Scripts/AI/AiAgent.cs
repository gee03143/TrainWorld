using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TrainWorld.Traffic;

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

        public List<AgentTask> tasks;
        public List<TrainStation> trainStationsInSchedule;
        int taskIndex;

        List<(Vector3Int, Direction8way)> path;

        internal void Init(Vector3Int position, Direction8way direction)
        {
            this.position = position;
            this.direction = direction;
            PlacementManager.GetRailAt(position, direction).myRailblock.hasAgent = true;
        }

        public void SetUpSchedule(List<(AgentTaskType, string)> schedule)
        {
            taskIndex = 0;
            //trainStationsInSchedule = schedule;
            move = true;

            if (tasks != null)
                tasks.Clear();

            foreach (var task in schedule)
            {
                if (task.Item1 == AgentTaskType.Move)
                {
                    MoveToStationTask newTask = new MoveToStationTask(this, task.Item2);
                    newTask.onFinish += ChangeTask;
                    newTask.onNextRail += ChangePositionAndDirection;
                    tasks.Add(newTask);
                }else if(task.Item1 == AgentTaskType.Wait)
                {
                    float.TryParse(task.Item2, out float waitTime);
                    Debug.Log(waitTime);
                    WaitTask newTask = new WaitTask(this, waitTime);
                    newTask.onFinish += ChangeTask;
                    tasks.Add(newTask);
                }
            }
        }

        private void ChangeTask()
        {
            taskIndex = (taskIndex + 1) % tasks.Count;
            tasks[taskIndex].OnTaskEnter();
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            tasks = new List<AgentTask>();
        }

        private void Update()
        {
            if(tasks.Count != 0)
                tasks[taskIndex].DoTask();
        }

        public float MoveAgent((Vector3Int, Direction8way) target)
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
