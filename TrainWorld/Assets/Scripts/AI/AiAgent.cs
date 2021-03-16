using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.AI
{
    [RequireComponent(typeof(Rigidbody))]
    public class AiAgent : MonoBehaviour
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

        Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            path = new List<Vertex>();
            currentIndex = 0;
        }

        public void Initialize(List<Vertex> path, bool isLoop)
        {
            this.path = path;
            currentIndex = 1;
            currentTarget = path[currentIndex];
            move = true;
            stop = false;
            loop = isLoop;
        }

        private void Update()
        {
            if(move && stop == false)
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
    }
}
