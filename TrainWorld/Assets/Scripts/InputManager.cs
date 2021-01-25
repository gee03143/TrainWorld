using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainWorld
{
    public class InputManager : MonoBehaviour
    {
        public Action<Vector3Int> OnMouseUp;
        public Action<Vector3> OnMouseDown, OnMouseMove;
        public Action<Vector3Int> OnRInput;
        public Action OnArrowInput;

        [SerializeField]
        private LayerMask layerMask;

        private Vector2 cameraMovementVector;

        public Vector2 CameraMovementVector
        {
            get { return cameraMovementVector; }
        }

        private void Update()
        {
            GetMouseDown();
            GetMouseMove();
            GetMouseUp();
            GetRInput();
            GetArrowInput();
        }

        private void GetArrowInput()
        {
            cameraMovementVector.x = Input.GetAxis("Horizontal");
            cameraMovementVector.y = Input.GetAxis("Vertical");
        }

        private void GetRInput()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Vector3? mousePosition = RayCastToGround();
                if(mousePosition != null)
                    OnRInput?.Invoke(Vector3Int.RoundToInt((Vector3)mousePosition));
            }
        }

        private void GetMouseUp()
        {
            if (Input.GetMouseButtonUp(0) && EventSystem.current.IsPointerOverGameObject() == false)
            {
                Vector3? mousePosition = RayCastToGround();
                if(mousePosition != null)
                    OnMouseUp?.Invoke(Vector3Int.RoundToInt((Vector3)mousePosition));
            }
        }

        private void GetMouseMove()
        {
            if((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0) && EventSystem.current.IsPointerOverGameObject() == false)
            {
                Vector3? mousePosition = RayCastToGround();
                if (mousePosition != null)
                    OnMouseMove?.Invoke((Vector3)mousePosition);
            }
        }

        private void GetMouseDown()
        {
            if(Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
            {
                Vector3? mousePosition = RayCastToGround();
                if(mousePosition != null)
                    OnMouseDown?.Invoke((Vector3)mousePosition);
            }
        }

        private Vector3? RayCastToGround()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
                return hit.point;
            }
            return null;
        }
    }
}
