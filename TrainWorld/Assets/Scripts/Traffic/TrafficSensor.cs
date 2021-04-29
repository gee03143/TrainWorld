using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.AI;

namespace TrainWorld.Traffic
{
    public class TrafficSensor : MonoBehaviour
    {
        TrafficSignal signal;

        private void Awake()
        {
            signal = GetComponentInParent<TrafficSignal>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            /*
            Debug.Log("TriggerEnter called");
            AiAgent currentAgnet = collider.gameObject.GetComponent<AiAgent>();
            if (currentAgnet != null)
            {
                Debug.Log(currentAgnet.ToString());
                signal.agentEnter.Invoke();
            }
            */
        }
    }
}
