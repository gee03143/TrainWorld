using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Traffic
{
    public class TrafficSignal : MonoBehaviour, ISelectableObject
    {
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

        internal void Init(Vector3Int position, Direction8way direction)
        {
            this.position = position;
            this.direction = direction;
        }

        public SelectableObjectType GetSelectableObjectType()
        {
            return SelectableObjectType.Traffic;
        }

        public override string ToString()
        {
            return position.ToString() + " " + direction.ToString();
        }

        public void ShowMyUI()
        {
            Debug.Log(this.ToString());
        }
    }
}
