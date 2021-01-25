using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class CurveCreator : MonoBehaviour
    {
        [HideInInspector]
        public Curve curve;

        public Color anchorCol = Color.red;
        public Color controlCol = Color.white;
        public Color segmentCol = Color.green;
        public Color selectedSegmentCol = Color.yellow;
        public float anchorDiameter = .1f;
        public float controlDiameter = .075f;
        public bool displayControlPoints = true;

        public void CreateCurve()
        {
            curve = new Curve(transform.position);
        }

        public void CreateCurve(Vector3 position1, Vector3 position2, Direction direction1, Direction direction2)
        {
            curve = new Curve(position1, position2, direction1, direction2);
        }

        void Reset()
        {
            CreateCurve();
        }
    }

}