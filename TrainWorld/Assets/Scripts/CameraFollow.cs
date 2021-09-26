using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class CameraFollow : MonoBehaviour
    {
        private Camera gameCamera;

        private Vector3 target;
        private bool hasTarget;

        public float smoothSpeed = 10.0f;
        public Vector3 offset;

        public void SetTarget(Vector3 newTarget)
        {
            target = newTarget;
            gameCamera.orthographicSize = 4.5f;
            hasTarget = true;
        }

        private void Awake()
        {
            gameCamera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (!hasTarget)
                return;

            Vector3 desiredPosition = target + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            if(Vector3.Distance(desiredPosition, smoothedPosition) < 1.0f)
            {
                hasTarget = false;
            }
        }
    }
}
