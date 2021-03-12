using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainWorld
{
    //유저 입력을 체크하는 클래스
    // 마우스 키보드 입력을 인식하고 해당 Action을 Invoke 시킴
    // 추후 다른 입력 방식이 생길 경우 GameManager참조를 인터페이스로 옮기고 
    // 이 클래스가 해당 인터페이스를 구현하게 할 것
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private float minMouseMovement;
        [SerializeField]
        private float minAxis;
        [SerializeField]
        private float minMouseScroll;

        public Action<Vector3Int> onMouseDown;
        public Action<Vector3> onMouseMove, onAxisInput;
        public Action<float> onMouseScroll;
        public Action onRInput, onEscInput;

        private LayerMask layerMask;

        private void Awake()
        {
            layerMask = 1 << LayerMask.NameToLayer("Plane");
        }

        private void Update()
        {
            CheckMouseDown();
            CheckMouseMove();
            CheckRInput();
            CheckAxisInput();
            CheckMouseScrollInput();
            CheckEscInput();
        }

        private void CheckMouseScrollInput()
        {
            float scrollDelta = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scrollDelta) > minMouseScroll)
            {
                onMouseScroll?.Invoke(scrollDelta);
            }
        }

        private void CheckAxisInput()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (Mathf.Abs(horizontal) > minAxis || Mathf.Abs(vertical) > minAxis)
            {
                onAxisInput?.Invoke(new Vector3(horizontal, 0, vertical));
            }
        }

        private void CheckEscInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                onEscInput?.Invoke();
            }
        }

        private void CheckRInput()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                onRInput?.Invoke();
            }
        }

        private void CheckMouseMove()
        {
            if ( Mathf.Abs(Input.GetAxis("Mouse X")) > minMouseMovement || Mathf.Abs(Input.GetAxis("Mouse Y")) > minMouseMovement)
            {
                Vector3? hitPosition = RaycastToGround();
                if (hitPosition != null)
                    onMouseMove?.Invoke((Vector3)hitPosition);
            }
        }

        private void CheckMouseDown()
        {
            if(Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
            {
                Vector3? hitPosition = RaycastToGround();
                if (hitPosition != null)
                {
                    onMouseDown?.Invoke(Vector3Int.RoundToInt((Vector3)hitPosition));
                }
            }
        }

        private Vector3? RaycastToGround()
        {
            Vector3 hitPosition;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                hitPosition = hit.point;
                return hitPosition;
            }
            return null;
        }
    }
}