using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Rails {
    public enum RailType
    {
        Corner_Left,
        Corner_Right,
        Straight,
        Diagonal,
        Straight_Deadend,
        Diagonal_Deadend
    }


    public class RailTypeHolder : MonoBehaviour
    {
        [SerializeField]
        public RailType type;
    }
}
