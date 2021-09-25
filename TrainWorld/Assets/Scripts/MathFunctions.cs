using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class MathFunctions : MonoBehaviour
    {
        public static int ManhattanDiscance(Vector3Int position1, Vector3Int position2)
        {
            return Mathf.Abs(position2.x - position1.x) + Mathf.Abs(position2.z - position1.z);
        }
    }
}