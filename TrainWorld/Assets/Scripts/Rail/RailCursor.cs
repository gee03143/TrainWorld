using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Rails
{
    public class RailCursor : MonoBehaviour
    {
        [SerializeField]
        private GameObject deadend_straight, deadend_diagonal;
        [SerializeField]
        private GameObject arrowPrefab;

        private GameObject arrow;

        private Vector3Int position;
            
        public Vector3Int Position
        {
            get { return position; }
            private set { 
                position = value;
                MoveTempRailObjects();
            }
        }

        private Direction8way direction;

        public Direction8way Direction
        {
            get { return direction; }
            private set { 
                direction = value;
            }
        }
        private void Awake()
        {
            arrow = Instantiate(arrowPrefab, position, Quaternion.Euler(direction.ToEuler())) as GameObject;
            arrow.SetActive(false);
        }

        internal void Move(Vector3Int position)
        {
            Position = position;
        }

        internal void Rotate()
        {
            direction = direction.Next();
            arrow.transform.rotation = Quaternion.Euler(direction.ToEuler());
        }

        private void MoveTempRailObjects()
        {
            arrow.transform.position = position;
        }

        public void ToggleCursorObject(bool toArrow)
        {
            if (toArrow)
            {
                arrow.SetActive(true);
            }
            else
            {
                arrow.SetActive(false);
            }
        }
    }
}
