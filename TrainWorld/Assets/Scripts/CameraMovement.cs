using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class CameraMovement : MonoBehaviour
    {
        private Camera gameCamera;
        [Range(0.1f,10.0f)]
        public float cameraMovementSpeed = 5;

        public float minOrthoSize = 3.0f;
        public float maxOrthoSize = 15.0f;
        [Range(0.1f, 2f)]
        public float scrollSensitivity = 0.5f;

        private void Start()
        {
            gameCamera = GetComponent<Camera>();
        }
        public void MoveCamera(Vector3 inputVector)
        {
            var movementVector = Quaternion.Euler(0, 30, 0) * inputVector;
            gameCamera.transform.position += movementVector * Time.deltaTime * cameraMovementSpeed;
        }

        internal void ChangeOrthoSize(float delta)
        {
            // delta is positive = zoom in, negative = zoom out

            gameCamera.orthographicSize = Mathf.Clamp(gameCamera.orthographicSize - delta * scrollSensitivity, minOrthoSize, maxOrthoSize);
        }
    }
}
