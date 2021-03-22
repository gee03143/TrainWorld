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
        private NeuturalStateManager neuturalStateManager;
        [SerializeField]
        private RailPlacementManager railPlacementManager;
        [SerializeField]
        private StationPlacementManager stationPlacementManager;
        [SerializeField]
        private UiController uiController;
        [SerializeField]
        private CameraMovement cameraMovement;

        InputHandler currentHandler;

        private void Start()
        {
            uiController.OnRailPlacement += RailButtonHandler;
            uiController.OnStationPlacement += StationButtonHandler;
            uiController.OnTrafficPlacement += TrafficButtonHandler;
            uiController.OnDestruction += DestructionButtonHandler;
            inputManager.onEscInput = HandleEscape;
            inputManager.onAxisInput += cameraMovement.MoveCamera;
            inputManager.onMouseScroll += cameraMovement.ChangeOrthoSize;
            SwitchHandler(neuturalStateManager);
        }

        private void HandleEscape()
        {
            SwitchHandler(neuturalStateManager);
            inputManager.onMouseDown += neuturalStateManager.OnClick;
            inputManager.onEscInput += neuturalStateManager.CloseUI;
            uiController.ResetButtonColor();
        }

        private void SwitchHandler(InputHandler nextHandler)
        {
            ClearInputActions();
            neuturalStateManager.CloseUI();

            if (currentHandler != nextHandler)
            {
                currentHandler?.OnExit();
                nextHandler?.OnEnter();

                currentHandler = nextHandler;
            }
        }

        void ClearInputActions()
        {
            inputManager.onMouseDown = null;
            inputManager.onMouseMove = null;
            inputManager.onRInput = null;
            inputManager.onEscInput = HandleEscape;
        }
         
        void RailButtonHandler()
        {
            SwitchHandler(railPlacementManager);

            inputManager.onMouseDown += railPlacementManager.PlaceRail;
            inputManager.onMouseMove += railPlacementManager.MoveCursorAtRailPlacement;
            inputManager.onRInput += railPlacementManager.RotateCursor;
        }
        void StationButtonHandler()
        {
            SwitchHandler(stationPlacementManager);

            inputManager.onMouseDown += stationPlacementManager.PlaceStation;
            inputManager.onMouseMove += stationPlacementManager.MoveCursor;
        }

        void TrafficButtonHandler()
        {
            SwitchHandler(null);
        }

        private void DestructionButtonHandler()
        {
            SwitchHandler(null);

            inputManager.onMouseDown += railPlacementManager.DestroyRail;
            inputManager.onRInput += railPlacementManager.RotateCursor;
            inputManager.onMouseMove += railPlacementManager.MoveCursorAtDestruction;
        }
    }

    public interface InputHandler{

        void OnEnter();

        void OnExit();
    }
}