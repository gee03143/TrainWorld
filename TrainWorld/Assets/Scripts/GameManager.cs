using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rail;
using TrainWorld.Station;

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
        private StationPlacementManager stationPlacementManager;
        [SerializeField]
        private UiController uiController;


        private void Start()
        {
            uiController.OnRailPlacement += RailButtonHandler;
            uiController.OnStationPlacement += StationButtonHandler;
            uiController.OnTrafficPlacement += TrafficButtonHandler;
            uiController.OnDestruction += DestructionButtonHandler;
            inputManager.onEscInput += HandleEscape;
        }

        private void HandleEscape()
        {
            ClearInputActions();
            uiController.ResetButtonColor();
        }

        void RailButtonHandler()
        {
            ClearInputActions();

            if (uiController.GetCurrentButton() != uiController.placeRailButton)
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

            inputManager.onMouseDown += stationPlacementManager.PlaceStation;
            inputManager.onMouseMove += stationPlacementManager.MoveCursor;
            inputManager.onRInput += stationPlacementManager.RotateCursor;
        }

        void TrafficButtonHandler()
        {
            ClearInputActions();
        }

        private void DestructionButtonHandler()
        {
            ClearInputActions();

            inputManager.onMouseDown += railPlacementManager.DestroyRail;
            inputManager.onRInput += railPlacementManager.RotateCursor;
        }
    }
}