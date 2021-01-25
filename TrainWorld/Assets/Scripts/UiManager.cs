using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TrainWorld
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField]
        Button railButton, stationButton, trafficButton;

        List<Button> buttons;

        Button selectedButton;

        GameManager gameManager;

        private void Awake()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }

        private void Start()
        {
            buttons = new List<Button> { railButton, stationButton, trafficButton };
            selectedButton = null;

            railButton.onClick.AddListener(() => {
                ModifyOutline(railButton);
                if (selectedButton != railButton)
                {
                    gameManager.RailButtonHandler();
                    selectedButton = railButton;
                }
                else
                {
                    gameManager.ClearButtonHandler();
                    selectedButton = null;
                }
            });

            stationButton.onClick.AddListener(() => {
                ModifyOutline(stationButton);
                if(selectedButton != stationButton)
                {
                    gameManager.StationButtonHandler();
                    selectedButton = stationButton;
                }
                else
                {
                    gameManager.ClearButtonHandler();
                    selectedButton = null;
                }
            });

            trafficButton.onClick.AddListener(() => {
                ModifyOutline(trafficButton);
                if (selectedButton != trafficButton)
                {
                    gameManager.TrafficButtonHandler();
                    selectedButton = trafficButton;
                }
                else
                {
                    gameManager.ClearButtonHandler();
                    selectedButton = null;
                }
            });
        }


        private void ModifyOutline(Button clickedButton)
        {
            if (selectedButton == clickedButton) {
                clickedButton.GetComponent<Outline>().enabled = false;
                gameManager.ClearButtonHandler();
            }
            else
            {
                foreach (var button in buttons)
                {
                    button.GetComponent<Outline>().enabled = false;
                }
                clickedButton.GetComponent<Outline>().enabled = true;
            }
        }
    }
}
