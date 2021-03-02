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

        private GameObject rail1;
        private GameObject rail2;

        private Vector3Int position;
            
        public Vector3Int CursorPosition
        {
            get { return position; }
            private set { 
                position = value;
                MoveTempRailObjects();
            }
        }

        private Direction direction;

        public Direction CursorDirection
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
                Quaternion.Euler(DirectionHelper.ToEuler(DirectionHelper.Opposite(direction)))) as GameObject;
        }

        internal void Move(Vector3Int position)
        {
            CursorPosition = position;
        }

        internal void Rotate()
        {
            direction = DirectionHelper.Next(direction);
            ChangeCursorRailObjects();
        }

        private void MoveTempRailObjects()
        {
            rail1.transform.position = position;
            rail2.transform.position = position;
        }

        private void ChangeCursorRailObjects()
        {
            Destroy(rail1);
            Destroy(rail2);

            if (DirectionHelper.IsDiagonal(direction)) {
                rail1 = Instantiate(deadend_diagonal, position, Quaternion.Euler(DirectionHelper.ToEuler(direction))) as GameObject;
                rail2 = Instantiate(deadend_diagonal, position,
                    Quaternion.Euler(DirectionHelper.ToEuler(DirectionHelper.Opposite(direction)))) as GameObject;
            }
            else
            {
                rail1 = Instantiate(deadend_straight, position, Quaternion.Euler(DirectionHelper.ToEuler(direction))) as GameObject;
                rail2 = Instantiate(deadend_straight, position,
                    Quaternion.Euler(DirectionHelper.ToEuler(DirectionHelper.Opposite(direction)))) as GameObject;
            }
        }
    }
}
