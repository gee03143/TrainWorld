using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Station
{
    public class TrainStation : MonoBehaviour
    {
        public Vector3Int Position;
        public Direction8way Direction;

        internal void DestroyMyself()
        {
            Destroy(gameObject);
        }
    }
}
