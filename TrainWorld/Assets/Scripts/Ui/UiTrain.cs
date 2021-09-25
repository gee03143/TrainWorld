using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using TrainWorld.Traffic;
using TrainWorld.AI;
using System;

namespace TrainWorld
{
    public class UiTrain : MonoBehaviour
    {
        [SerializeField]
        private GameObject uiDestinationRow;
        [SerializeField]
        private Transform DestinationsParent;


        private List<UiDestination> uiDestinations;
        private List<string> stationNames;

        [SerializeField]
        Dropdown destinationOptions;

        [SerializeField]
        Image addDestinationRow;

        [SerializeField]
        Button copyButton;

        [SerializeField]
        Button pasteButton;

        [SerializeField]
        Button closeButton;

        [SerializeField]
        Button addDestinationButton;

        [SerializeField]
        Button changeScheduleButton;

        AiAgent selectedAi;

        private void Awake()
        {
            uiDestinations = new List<UiDestination>();
            stationNames = new List<string>();
            SetDropdownOptions();

            copyButton.onClick.AddListener(CopyHandler);
            pasteButton.onClick.AddListener(PasteHandler);
            closeButton.onClick.AddListener(CloseHandler);
            addDestinationButton.onClick.AddListener(AddRowHandler);
            changeScheduleButton.onClick.AddListener( ChangeScheduleHandler);
        }

        private void SetDropdownOptions()
        {
            destinationOptions.ClearOptions();

            foreach (string station in PlacementManager.GetStations().Keys)
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = station;
                destinationOptions.options.Add(option);
            }
        }

        private void CopyHandler()
        {
            if (selectedAi != null) {
                Clipboard.CopyToClipboard(selectedAi.schedules);
            }
        }

        private void PasteHandler()
        {
            if (selectedAi != null)
            {
                List<(TrainStation, DepartureConditionType)> temp = new List<(TrainStation, DepartureConditionType)>();
                Clipboard.PasteFromClipboard(ref temp);
                RefreshDestinationRows(temp);
            }
        }

        private void CloseHandler()
        {
            SetActive(false);
        }

        private void AddRowHandler()
        {
            GameObject newObject = Instantiate(uiDestinationRow, DestinationsParent);

            UiDestination destination = newObject.GetComponent<UiDestination>();
            uiDestinations.Add(destination);
            destination.onDestroy += RemoveRowFromList;

            destination.SetDropdownOptions();
            destination.SetDropdownSelected(destinationOptions.value);

            newObject.transform.SetAsLastSibling();
            addDestinationRow.transform.SetAsLastSibling();
            changeScheduleButton.transform.SetAsLastSibling();
        }

        private void RemoveRowFromList(UiDestination row)
        {
            uiDestinations.Remove(row);
        }

        private void ChangeScheduleHandler()
        {
            List<(TrainStation, DepartureConditionType)> schedule = new List<(TrainStation, DepartureConditionType)>();
            foreach (var row in uiDestinations)
            {
                schedule.Add((row.GetStationDropdownSelected(), row.GetDepartureConditionDropdownSelected()));
            }
            selectedAi.SetUpSchedule(schedule);
        }

        public void SetSelectedAi(ISelectableObject selected)
        {
            selectedAi = (AiAgent)selected;

            RefreshDestinationRows(selectedAi.schedules);
        }

        private void RefreshDestinationRows(List<(TrainStation, DepartureConditionType)> schedules)
        {
            ClearAllRows();
            if (schedules != null)
            {
                foreach (var schedule in schedules)
                {
                    GameObject newObject = Instantiate(uiDestinationRow, DestinationsParent);

                    UiDestination destination = newObject.GetComponent<UiDestination>();

                    uiDestinations.Add(destination);
                    destination.onDestroy += RemoveRowFromList;

                    destination.SetDropdownOptions();
                    Debug.Log("( " + schedule.Item1 + " , " + schedule.Item2 + " )");
                    destination.SetDropdownSelected(schedule.Item1.StationName, schedule.Item2);

                    newObject.transform.SetAsLastSibling();
                }
            }

            addDestinationRow.transform.SetAsLastSibling();
            changeScheduleButton.transform.SetAsLastSibling();
        }

        private void ClearAllRows()
        {
            int numberofRows = uiDestinations.Count; // 컨테이너의 멤버 수가 변하므로 미리 값을 따로 저장해야 함

            for (int i = 0; i < numberofRows; i++)
            {
                uiDestinations.First().DestroyMyself();
            }
            uiDestinations.Clear();
        }

        public void SetStationNameList(List<string> stationNameList)
        {
            stationNames = stationNameList;
            SetDropdownOptions();
        }

        internal void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}
