using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class RailCursor : MonoBehaviour
    {
        [SerializeField]
        private GameObject deadend_straight, deadend_diagonal;
        [SerializeField]
        private GameObject arrowPrefab;

        private GameObject rail1;
        private GameObject rail2;
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
            rail1 = Instantiate(deadend_straight, position, Quaternion.Euler(DirectionHelper.ToEuler(direction))) as GameObject;
            rail2 = Instantiate(deadend_straight, position,
                Quaternion.Euler(DirectionHelper.ToEuler(direction) + new Vector3(0,180,0))) as GameObject;
            arrow = Instantiate(arrowPrefab, position, Quaternion.Euler(DirectionHelper.ToEuler(direction))) as GameObject;
            arrow.SetActive(false);
        }

        internal void Move(Vector3Int position)
        {
            Position = position;
        }

        internal void Rotate()
        {
            direction = DirectionHelper.Next(direction);
            arrow.transform.rotation = Quaternion.Euler(DirectionHelper.ToEuler(direction));
            ChangeCursorRailObjects();
        }

        private void MoveTempRailObjects()
        {
            rail1.transform.position = position;
            rail2.transform.position = position;
            arrow.transform.position = position;
        }

        private void ChangeCursorRailObjects()
        {
            Destroy(rail1);
            Destroy(rail2);

            if (DirectionHelper.IsDiagonal(direction)) {
                rail1 = Instantiate(deadend_diagonal, position, Quaternion.Euler(DirectionHelper.ToEuler(direction))) as GameObject;
                rail2 = Instantiate(deadend_diagonal, position,
                    Quaternion.Euler(DirectionHelper.ToEuler(direction) + new Vector3(0, 180, 0))) as GameObject;
            }
            else
            {
                rail1 = Instantiate(deadend_straight, position, Quaternion.Euler(DirectionHelper.ToEuler(direction))) as GameObject;
                rail2 = Instantiate(deadend_straight, position,
                    Quaternion.Euler(DirectionHelper.ToEuler(direction) + new Vector3(0, 180, 0))) as GameObject;
            }
        }

        public void ToggleCursorObject(bool toArrow)
        {
            if (toArrow)
            {
                rail1.SetActive(false);
                rail2.SetActive(false);
                arrow.SetActive(true);
            }
            else
            {
                rail1.SetActive(true);
                rail2.SetActive(true);
                arrow.SetActive(false);
            }
        }
    }
}
