using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TrainWorld.Traffic;

namespace TrainWorld
{
    public class UiStationData : MonoBehaviour
    {
        [SerializeField]
        StationPlacementManager stationPlacementManager;

        [SerializeField]
        InputField nameInputField;

        [SerializeField]
        Text PositionText;

        [SerializeField]
        Text DirectionText;

        TrainStation selectedStation;

        public void Awake()
        {
            nameInputField.onEndEdit.AddListener(TryChangeName);
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetSelectedStation(ISelectableObject selectedObject)
        {
            selectedStation = (TrainStation)selectedObject;
        }

        public void DisplaySelectedStationData()
        {
            nameInputField.text = selectedStation.StationName;
            PositionText.text = selectedStation.Position.ToString();
            DirectionText.text = selectedStation.Direction.ToString();
        }

        private void TryChangeName(string input)
        {
            if (selectedStation.StationName == input)
                return;

            bool isSuccessed = stationPlacementManager.TryChangeName(selectedStation.StationName, input, selectedStation);

            if (isSuccessed)
            {
                selectedStation.StationName = input;
            }
        }
    }
}
