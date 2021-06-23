using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Traffic
{
    [RequireComponent(typeof(Rigidbody))]
    public class TrainStation : MonoBehaviour, ISelectableObject
    {
        [SerializeField]
        private string stationName;

        public string StationName
        {
            get { return stationName; }
            set { stationName = value; }
        }

        private Vector3Int position;

        public Vector3Int Position
        {
            get { return position; }
            set { position = value; }
        }

        private Direction8way direction;

        public Direction8way Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        internal void Init(Vector3Int position, Direction8way direction, string name)
        {
            this.position = position;
            this.direction = direction;
            this.StationName = name;
        }

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
