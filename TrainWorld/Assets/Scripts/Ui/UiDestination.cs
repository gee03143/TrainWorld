using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;

using TrainWorld.Traffic;

namespace TrainWorld
{
    public enum DepartureConditionType
    {
        Wait,
        Load,
        Unload
    }

    public class UiDestination : MonoBehaviour
    {
        [SerializeField]
        Dropdown stationDropdown;
        [SerializeField]
        Dropdown departureConditionDropdown;
        [SerializeField]
        Button locationButton;
        [SerializeField]
        Button deleteButton;

        public Action<UiDestination> onDestroy;


        private TrainStation destination;

        public TrainStation Destination
        {
            get { return destination; }
            private set { destination = value; }
        }

        private DepartureConditionType departureCondition;

        public DepartureConditionType DepartureCondition
        {
            get { return departureCondition; }
            private set { departureCondition = value; }
        }

        private void Awake()
        {
            stationDropdown.onValueChanged.AddListener(ChangeStation);
            departureConditionDropdown.onValueChanged.AddListener(ChangeDepartureCondition);

            locationButton.onClick.AddListener(FindStation);
            deleteButton.onClick.AddListener(DestroyMyself);
        }

        private void ChangeDepartureCondition(int value)
        {
            DepartureCondition = (DepartureConditionType)value;
        }

        private void ChangeStation(int value)
        {
            Destination = PlacementManager.GetStationOfName(stationDropdown.options[value].text);
        }

        private void FindStation()
        {
            TrainStation targetStation = GetStationDropdownSelected();

            // TODO : focus camera on target station
        }

        internal TrainStation GetStationDropdownSelected()
        {
            return PlacementManager.GetStationOfName(stationDropdown.options[stationDropdown.value].text);
        }

        internal DepartureConditionType GetDepartureConditionDropdownSelected()
        {
            return (DepartureConditionType)departureConditionDropdown.value;
        }

        public void SetDropdownOptions()
        {
            stationDropdown.ClearOptions();
            foreach (string station in PlacementManager.GetStations().Keys)
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = station;
                stationDropdown.options.Add(option);
            }

            departureConditionDropdown.ClearOptions();
            foreach (var name in Enum.GetValues(typeof(DepartureConditionType)))
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = name.ToString();
                departureConditionDropdown.options.Add(option);
            }
        }

        internal void SetDropdownSelected(string value, DepartureConditionType departureCondition)
        {
            List<string> options = stationDropdown.options.Select(option => option.text).ToList();
            stationDropdown.value = options.IndexOf(value);

            options = departureConditionDropdown.options.Select(option => option.text).ToList();
            departureConditionDropdown.value = (int)departureCondition;
        }

        internal void SetDropdownSelected(int value)
        {
            stationDropdown.value = value; // option starts from 0
        }

        public void DestroyMyself()
        {
            Debug.Log("on destroy is called");
            onDestroy?.Invoke(this);
            Destroy(gameObject);
        }
    }
}