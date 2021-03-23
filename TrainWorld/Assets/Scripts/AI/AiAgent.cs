using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TrainWorld.Station;
using TrainWorld.Rail;

namespace TrainWorld.AI
{
    [RequireComponent(typeof(Rigidbody))]
    public class AiAgent : MonoBehaviour, ISelectableObject
    {
        public event Action OnDeath;

        [SerializeField]
        private float speed = 1.0f;
        [SerializeField]
        private float rotationSpeed = 10.0f;

        public bool loop;

        private bool move;

        private bool stop;

        List<Vertex> path;
        int currentIndex;
        Vertex currentTarget;

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

        /*
        internal void ChangeDestination(TrainStation dest1, TrainStation dest2, RailGraph railGraph)
        {
 
            Debug.Log("change destination called");
            path = RailGraphPathfinder.AStarSearch(dest1.Position, dest1.Direction, dest2.Position, true, railGraph);
            path.AddRange(RailGraphPathfinder.AStarSearch(dest2.Position, dest2.Direction, dest1.Position, true, railGraph));
            currentTarget = path[0];
            move = true;

        }
        */

        Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            path = new List<Vertex>();
            currentIndex = 0;
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
            if(path.Count > currentIndex)
            {
                float remainingDistance = MoveAgent();
                if(remainingDistance < 0.1f)
                {
                    // step to next position
                    currentIndex++;
                    if(currentIndex >= path.Count)
                    {
                        if (loop)
                        {
                            currentIndex = 0;
                        }
                        else
                        {
                            move = false;
                            return;
                        }
                    }
                    currentTarget = path[currentIndex];
                }
            }
        }

        private float MoveAgent()
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.Position, step);
            rb.velocity = transform.forward * speed;

            Vector3 lookDirection = currentTarget.Position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * rotationSpeed);
            return Vector3.Distance(transform.position, currentTarget.Position);
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
