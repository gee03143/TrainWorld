using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    // 각 Manager들을 통합하는 역할
    // UI 입력에 따른 행동 설정
    // 
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private InputManager inputManager;
        [SerializeField]
        private RailPlacementManager railPlacementManager;
        [SerializeField]
        private UiController uiController;


        private void Start()
        {
            uiController.OnRailPlacement += RailButtonHandler;
            uiController.OnStationPlacement += StationButtonHandler;
            uiController.OnTrafficPlacement += TrafficButtonHandler;
            inputManager.onEscInput += HandleEscape;

            inputManager.onMouseDown = TempFunction2;
            inputManager.onMouseMove = TempFunction2;
            inputManager.onRInput = TempFunction;
        }

        private void HandleEscape()
        {
            ClearInputActions();
            uiController.ResetButtonColor();
        }

        void RailButtonHandler()
        {
            ClearInputActions();

            if(uiController.GetCurrentButton() != uiController.placeRailButton)
            {
                railPlacementManager.RailPlacementEnter();
            }

            inputManager.onMouseDown += railPlacementManager.PlaceRail;
            inputManager.onMouseMove += railPlacementManager.MoveCursor;
            inputManager.onRInput += railPlacementManager.RotateCursor;
        }

        void ClearInputActions()
        {
            inputManager.onMouseDown = null;
            inputManager.onMouseMove = null;
            inputManager.onRInput = null;
        }

        void StationButtonHandler()
        {
            ClearInputActions();
            inputManager.onMouseDown = TempFunction2;
            inputManager.onMouseMove = TempFunction2;
            inputManager.onRInput = TempFunction;
        }

        void TrafficButtonHandler()
        {
            ClearInputActions();
            inputManager.onMouseDown = TempFunction2;
            inputManager.onMouseMove = TempFunction2;
            inputManager.onRInput = TempFunction;
        }

        void TempFunction()
        {
            //temp function to avoid nullReferenceException
        }

        void TempFunction2(Vector3Int vector)
        {
            //temp function to avoid nullReferenceException
        }

    }
}