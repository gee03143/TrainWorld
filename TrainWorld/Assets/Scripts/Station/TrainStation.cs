using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Station
{
    [RequireComponent(typeof(Rigidbody))]
    public class TrainStation : MonoBehaviour, ISelectableObject
    {
        public string stationName;
        public Vector3Int Position;
        public Direction8way Direction;

        public override string ToString()
        {
            return stationName + " " + Position.ToString() + " " + Direction.ToString();
        }

        public SelectableObjectType GetSelectableObjectType()
        {
            return SelectableObjectType.Station;
        }

        public void ShowMyUI()
        {
            Debug.Log(this.ToString());
        }

        internal void DestroyMyself()
        {
            Destroy(gameObject);
        }
    }
}
