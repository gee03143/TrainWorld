using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rails;
using TrainWorld.Station;
using TrainWorld.AI;
using TrainWorld.Traffic;

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
        private TrainPlacementManager trainPlacementManager;
        [SerializeField]
        private TrafficPlacementManager trafficPlacementManager;
        [SerializeField]
        private DestructionManager destructionManager;
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
            uiController.OnTrainPlacement += TrainButtonHandler;
            uiController.OnDestruction += DestructionButtonHandler;

            inputManager.onEscInput = HandleEscape;
            inputManager.onAxisInput += cameraMovement.MoveCamera;
            inputManager.onMouseScroll += cameraMovement.ChangeOrthoSize;
            SwitchHandler(neuturalStateManager);
        }

        private void HandleEscape()
        {
            SwitchHandler(neuturalStateManager);
            inputManager.onMouseDown += neuturalStateManager.OnMouseDown;
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
                inputManager.onMouseDown += currentHandler.OnMouseDown;
                inputManager.onMouseMove += currentHandler.OnMouseMove;
                inputManager.onRInput += currentHandler.OnRInput;
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
        }
        private void StationButtonHandler()
        {
            SwitchHandler(stationPlacementManager);
        }

        private void TrafficButtonHandler()
        {
            SwitchHandler(trafficPlacementManager);
        }

        private void TrainButtonHandler()
        {
            SwitchHandler(trainPlacementManager);
        }

        private void DestructionButtonHandler()
        {
            SwitchHandler(destructionManager);
        }
    }

    public interface InputHandler{

        void OnMouseDown(Vector3 mousePosition);

        void OnMouseMove(Vector3 mousePosition);

        void OnRInput();

        void OnEnter();

        void OnExit();
    }
}