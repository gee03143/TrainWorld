using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.AI;

namespace TrainWorld
{
    public class PathVisualizer : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
        }

        public void ShowPath(List<(Vector3Int, Direction8way)> path)
        {
            ResetPath();
            lineRenderer.positionCount = path.Count;
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
            for (int i = 0; i < path.Count; i++)
            {
                lineRenderer.SetPosition(i, path[i].Item1);
            }
        }

        public void ResetPath()
        {
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
            }
        }
    }
}
