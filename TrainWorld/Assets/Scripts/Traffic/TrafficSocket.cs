using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Traffic;

namespace TrainWorld.Traffic
{
    public enum TrafficSocketType
    {
        Empty,
        Traffic,
        Station
    }

    public class TrafficSocket : MonoBehaviour
    {
        private TrafficSocketType socketType = TrafficSocketType.Empty;

        public TrafficSocketType Type
        {
            get { return socketType; }
            set
            {
                socketType = value;
                SetMyGameObject(value);
            }
        }

        [SerializeField]
        private GameObject stationObject;
        [SerializeField]
        private GameObject trafficObject;

        private List<GameObject> objectList;


        private void Start()
        {
            objectList = new List<GameObject> { stationObject, trafficObject };
        }

        public TrainStation GetStation()
        {
            return stationObject.GetComponent<TrainStation>();
        }

        public TrafficSignal GetTraffic()
        {
            return trafficObject.GetComponent<TrafficSignal>();
        }

        public void SetMyGameObject(TrafficSocketType value)
        {
            ClearAllObjects();
            if (value == TrafficSocketType.Station)
                stationObject.SetActive(true);
            else if (value == TrafficSocketType.Traffic)
                trafficObject.SetActive(true);
        }

        private void ClearAllObjects()
        {
            foreach (var obj in objectList)
            {
                obj.SetActive(false);
            }
        }
    }
}
