using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public static class Bezier
    {
        public static Vector3 EvaluateQuadratic(Vector3 point1, Vector3 point2, Vector3 point3, float t)
        {
            Vector3 p0 = Vector3.Lerp(point1, point2, t);
            Vector3 p1 = Vector3.Lerp(point2, point3, t);
            return Vector3.Lerp(p0, p1, t);
        }

        public static Vector3 EvaluateCubic(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, float t)
        {
            Vector3 p0 = EvaluateQuadratic(point1, point2, point3, t);
            Vector3 p1 = EvaluateQuadratic(point2, point3, point4, t);
            return Vector3.Lerp(p0, p1, t);
        }
    }
}
