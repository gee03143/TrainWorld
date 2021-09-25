using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TrainWorld.Buildings;
using TrainWorld.Traffic;
using System;

namespace TrainWorld
{

    public class UiBuilding : MonoBehaviour
    {
        [SerializeField]
        Text PositionText;

        [SerializeField]
        Text StorageText;

        [SerializeField]
        Dropdown possibleStationDropdown;

        Building selectedBuilding;

        private void Awake()
        {
            possibleStationDropdown.onValueChanged.AddListener(delegate
            {
                ChangeSelectedBuilding(possibleStationDropdown);
            });
        }

        private void ChangeSelectedBuilding(Dropdown select)
        {
            string stationName = select.options[select.value].text;

            selectedBuilding.SetConnectedBuilding(stationName);
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetSelectedBuilding(ISelectableObject selectedObject)
        {
            selectedBuilding = (Building)selectedObject;
        }

        public void DisplaySelectedStationData()
        {
            PositionText.text = selectedBuilding.Position.ToString();
            StorageText.text = selectedBuilding.storage.CurrentStorage.ToString() +
                " / " + selectedBuilding.storage.maxStorage.ToString();
            
            List<TrainStation> possibleStations = selectedBuilding.GetAllPossibleStations();
            SetDropdownOptions(possibleStations);
            possibleStationDropdown.RefreshShownValue();
        }

        internal void SetDropdownOptions(List<TrainStation> possibleStations)
        {
            possibleStationDropdown.ClearOptions();
            foreach (var station in possibleStations)
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = station.StationName;
                possibleStationDropdown.options.Add(option);
            }
            Dropdown.OptionData emptyOption = new Dropdown.OptionData();
            emptyOption.text = "empty";
            possibleStationDropdown.options.Add(emptyOption);

            foreach (var item in possibleStationDropdown.options)
            {
                if(selectedBuilding.ConnectedStation != null && 
                    item.text == selectedBuilding.ConnectedStation.StationName)
                {
                    possibleStationDropdown.value = possibleStationDropdown.options.IndexOf(item);
                    return;
                }
            }

            possibleStationDropdown.value = possibleStationDropdown.options.IndexOf(emptyOption);

        }
    }
}
