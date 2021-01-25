using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public enum Direction // ↕ ⤢ ↔ ⤡, 8 way direction
    {
        W, SW, S, SE, E, NE, N, NW, DIRECTION_COUNT
    }

    public enum Type
    {
        Empty, Rail, Station, TYPE_COUNT
    }
}
