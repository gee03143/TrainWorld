using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace TrainWorld
{
    // 3가지 UI버튼에 대한 참조와 Action을 정의
    // 해당 버튼 클릭 시 Action을 Invoke 시킴
    public class UiController : MonoBehaviour
    {
        public Action OnRailPlacement, OnStationPlacement, OnTrafficPlacement, OnTrainPlacement, OnDestruction;
        public Button placeRailButton, placeStationButton, placeTrafficButton, placeTrainButton, destructionButton;

        public Color outlineColor;
        List<Button> buttonList;

        private Button currentButton;

        public Button CurrentButton
        {
            get { return currentButton; }
            private set { currentButton = value; }
        }


        private void Start()
        {
            buttonList = new List<Button> { placeStationButton, placeRailButton, placeTrafficButton, placeTrainButton, destructionButton };

            placeRailButton.onClick.AddListener(() =>
            {
                ResetButtonColor();
                ModifyOutline(placeRailButton);
                OnRailPlacement?.Invoke();

            });
            placeStationButton.onClick.AddListener(() =>
            {
                ResetButtonColor();
                ModifyOutline(placeStationButton);
                OnStationPlacement?.Invoke();

            });
            placeTrafficButton.onClick.AddListener(() =>
            {
                ResetButtonColor();
                ModifyOutline(placeTrafficButton);
                OnTrafficPlacement?.Invoke();

            });

            placeTrainButton.onClick.AddListener(() =>
            {
                ResetButtonColor();
                ModifyOutline(placeTrainButton);
                OnTrainPlacement?.Invoke();
            });

            destructionButton.onClick.AddListener(() =>
            {
                ResetButtonColor();
                ModifyOutline(destructionButton);
                OnDestruction?.Invoke();

            });
        }

        private void ModifyOutline(Button button)
        {
            var outline = button.GetComponent<Outline>();
            outline.effectColor = outlineColor;
            outline.enabled = true;
        }

        public void ResetButtonColor()
        {
            foreach (Button button in buttonList)
            {
                button.GetComponent<Outline>().enabled = false;
            }
        }

        public Button GetCurrentButton()
        {
            foreach (Button button in buttonList)
            {
                var outline = button.GetComponent<Outline>();
                if (outline.enabled)
                {
                    return button;
                }
            }
            return null;
        }
    }
}

