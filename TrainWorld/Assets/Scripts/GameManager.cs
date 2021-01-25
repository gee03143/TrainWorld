using SVS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private InputManager inputManager;
        [SerializeField]
        private RailManager railManager;
        [SerializeField]
        private CameraMovement cameraMovement;

        internal void RailButtonHandler()
        {
            ClearButtonHandler();

            inputManager.OnMouseMove += railManager.DisplayTempObjects;
            inputManager.OnMouseDown += railManager.PlaceRail;
            inputManager.OnRInput += railManager.RotateTempRail;
        }

        internal void StationButtonHandler()
        {
            ClearButtonHandler();
        }

        internal void TrafficButtonHandler()
        {
            ClearButtonHandler();
        }

        internal void ClearButtonHandler()
        {
            inputManager.OnMouseUp = null;
            inputManager.OnMouseMove = null;
            inputManager.OnMouseDown = null;
            inputManager.OnRInput = null;
        }

        public void PrintPosition(Vector3Int position)
        {
            Debug.Log(position.ToString());
        }

        private void Update()
        {
            cameraMovement.MoveCamera(inputManager.CameraMovementVector);
        }
    }
}