using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TrainWorld
{
    [CustomEditor(typeof(CurveCreator))]
    public class CurveEditor : Editor
    {

        CurveCreator creator;
        Curve curve
        {
            get
            {
                return creator.curve;
            }
        }

        const float segmentSelectDistanceThreshold = .1f;
        int selectedSegmentIndex = -1;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Create new"))
            {
                Undo.RecordObject(creator, "Create new");
                creator.CreateCurve();
            }

            bool isClosed = GUILayout.Toggle(curve.IsClosed, "Closed");
            if (isClosed != curve.IsClosed)
            {
                Undo.RecordObject(creator, "Toggle closed");
                curve.IsClosed = isClosed;
            }

            bool autoSetControlPoints = GUILayout.Toggle(curve.AutoSetControlPoints, "Auto Set Control Points");
            if (autoSetControlPoints != curve.AutoSetControlPoints)
            {
                Undo.RecordObject(creator, "Toggle auto set controls");
                curve.AutoSetControlPoints = autoSetControlPoints;
            }

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }

        void OnSceneGUI()
        {
            Input();
            Draw();
        }

        void Input()
        {
            Event guiEvent = Event.current;
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
            {
                if (selectedSegmentIndex != -1)
                {
                    Undo.RecordObject(creator, "Split segment");
                    curve.SplitSegment(mousePos, selectedSegmentIndex);
                }
                else if (!curve.IsClosed)
                {
                    Undo.RecordObject(creator, "Add segment");
                    curve.AddSegment(mousePos);
                }
            }


            HandleUtility.AddDefaultControl(0);
        }

        void Draw()
        {
            for (int i = 0; i < curve.NumSegments; i++)
            {
                Vector3[] points = curve.GetPointsInSegment(i);
                if (creator.displayControlPoints)
                {
                    Handles.color = Color.black;
                    Handles.DrawLine(points[1], points[0]);
                    Handles.DrawLine(points[2], points[3]);
                }
                Color segmentCol = (i == selectedSegmentIndex && Event.current.shift) ? creator.selectedSegmentCol : creator.segmentCol;
                Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentCol, null, 2);
            }


            for (int i = 0; i < curve.NumPoints; i++)
            {
                if (i % 3 == 0 || creator.displayControlPoints)
                {
                    Handles.color = (i % 3 == 0) ? creator.anchorCol : creator.controlCol;
                    float handleSize = (i % 3 == 0) ? creator.anchorDiameter : creator.controlDiameter;
                    Vector3 newPos = Handles.FreeMoveHandle(curve[i], Quaternion.identity, handleSize, Vector3.zero, Handles.CylinderHandleCap);
                    if (curve[i] != newPos)
                    {
                        Undo.RecordObject(creator, "Move point");
                        curve.MovePoint(i, newPos);
                    }
                }
            }
        }

        void OnEnable()
        {
            creator = (CurveCreator)target;
            if (creator.curve == null)
            {
                creator.CreateCurve();
            }
        }
    }
}
