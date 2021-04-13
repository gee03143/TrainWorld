using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TrainWorld.Station;
using TrainWorld.AI;
using System;

namespace TrainWorld
{
    public class UiTrain : MonoBehaviour
    {
        [SerializeField]
        StationPlacementManager stationPlacementManager;

        [SerializeField]
        private GameObject uiTrainRow;

        private List<UiTrainRow> uiTrainRows;
        private List<string> stationNames;

        [SerializeField]
        Button addRowButton;

        [SerializeField]
        Button changeDestinationButton;

        AiAgent selectedAi;

        private void Awake()
        {
            uiTrainRows = new List<UiTrainRow>();
            stationNames = new List<string>();
            addRowButton.onClick.AddListener(AddRowHandler);
            changeDestinationButton.onClick.AddListener( ChangeDestinationHandler);
        }

        private void AddRowHandler()
        {
            GameObject newObject = Instantiate(uiTrainRow, gameObject.transform);
            UiTrainRow row = newObject.GetComponent<UiTrainRow>();
            newObject.transform.SetAsLastSibling();
            uiTrainRows.Add(row);
            row.SetDropdownOptions(stationNames);
            row.onDestroy += RemoveRowFromList;

            addRowButton.transform.SetAsLastSibling();
            changeDestinationButton.transform.SetAsLastSibling();

        }

        private void RemoveRowFromList(UiTrainRow row)
        {
            uiTrainRows.Remove(row);
        }

        private void ChangeDestinationHandler()
        {
            List<string> destinations = new List<string>();
            foreach (var rows in uiTrainRows)
            {
                destinations.Add(rows.GetDropdownSelected());
            }

            stationPlacementManager.SetDestinationToAgent(destinations, selectedAi);
        }

        public void SetSelectedAi(ISelectableObject selected)
        {
            selectedAi = (AiAgent)selected;

            ClearAllRows();

            var stationsInSchedule = selectedAi.trainStationsInSchedule;

            if (stationsInSchedule != null)
            {
                foreach (var item in selectedAi.trainStationsInSchedule)
                {
                    GameObject newObject = Instantiate(uiTrainRow, gameObject.transform);
                    UiTrainRow row = newObject.GetComponent<UiTrainRow>();
                    uiTrainRows.Add(row);
                }
            }
            addRowButton.transform.SetAsLastSibling();
            changeDestinationButton.transform.SetAsLastSibling();
            SetUpDropdown();
            int index = 0;
            if (stationsInSchedule != null)
            {
                foreach (var row in uiTrainRows)
                {
                    row.SetDropdownSelected(selectedAi.trainStationsInSchedule[index].StationName);
                    index++;
                }
            }
        }

        private void ClearAllRows()
        {
            foreach (var row in uiTrainRows)
            {
                row.DestroyMyself();
            }
            uiTrainRows.Clear();
        }

        public void SetStationNameList(List<string> stationNameList)
        {
            stationNames = stationNameList;
            SetUpDropdown();
        }

        private void SetUpDropdown()
        {
            foreach (var item in uiTrainRows)
            {
                item.SetDropdownOptions(stationNames);
            }
        }

        internal void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}
