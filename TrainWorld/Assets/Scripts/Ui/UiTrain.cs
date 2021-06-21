using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TrainWorld.AI;
using System;

namespace TrainWorld
{
    public class UiTrain : MonoBehaviour
    {
        [SerializeField]
        private GameObject uiMoveRow;
        [SerializeField]
        private GameObject uiWaitRow;

        private Dictionary<AgentTaskType, GameObject> taskRowPrefabs;

        private List<UiTrainRow> uiTrainRows;
        private List<string> stationNames;

        [SerializeField]
        Dropdown taskOptions;

        [SerializeField]
        Image addTaskRow;

        [SerializeField]
        Button addTaskButton;

        [SerializeField]
        Button changeScheduleButton;

        AiAgent selectedAi;

        private void Awake()
        {
            uiTrainRows = new List<UiTrainRow>();
            stationNames = new List<string>();
            taskRowPrefabs = new Dictionary<AgentTaskType, GameObject>();
            InitTaskRowPrefabs();
            SetDropdownOptions();

            addTaskButton.onClick.AddListener(AddRowHandler);
            changeScheduleButton.onClick.AddListener( ChangeScheduleHandler);
        }

        private void InitTaskRowPrefabs()
        {
            taskRowPrefabs.Add(AgentTaskType.Move, uiMoveRow);
            taskRowPrefabs.Add(AgentTaskType.Wait, uiWaitRow);
        }

        private void SetDropdownOptions()
        {
            taskOptions.ClearOptions();
            foreach (AgentTaskType name in Enum.GetValues(typeof(AgentTaskType)))
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = name.ToString();
                taskOptions.options.Add(option);
            }
        }

        private void AddRowHandler()
        {
            AgentTaskType type = (AgentTaskType)taskOptions.value;
            GameObject newObject = Instantiate(taskRowPrefabs[type], gameObject.transform);

            UiTrainRow row = newObject.GetComponent<UiTrainRow>();
            uiTrainRows.Add(row);
            row.onDestroy += RemoveRowFromList;
            if (type == AgentTaskType.Move) // Move타입일 경우 dropdown을 station의 이름들로 채움
                row.SetDropdownOptions(stationNames);

            newObject.transform.SetAsLastSibling();
            addTaskRow.transform.SetAsLastSibling();
            changeScheduleButton.transform.SetAsLastSibling();
        }

        private void RemoveRowFromList(UiTrainRow row)
        {
            uiTrainRows.Remove(row);
        }

        private void ChangeScheduleHandler()
        {
            List<(AgentTaskType, string)> tasks = new List<(AgentTaskType, string)>();
            foreach (var row in uiTrainRows)
            {
                tasks.Add((row.GetComponent<AgentTaskTypeHolder>().taskType, row.GetDropdownSelected()));
            }

            //stationPlacementManager.SetDestinationToAgent(destinations, selectedAi);
            selectedAi.SetUpSchedule(tasks);
        }

        public void SetSelectedAi(ISelectableObject selected)
        {
            selectedAi = (AiAgent)selected;

            ClearAllRows();

            var tasks = selectedAi.tasks;

            if (tasks != null)
            {
                foreach (var task in tasks)
                {
                    GameObject newObject = Instantiate(taskRowPrefabs[task.taskType], gameObject.transform);
                    UiTrainRow row = newObject.GetComponent<UiTrainRow>();
                    uiTrainRows.Add(row);

                    /*
                    if (task.taskType == AgentTaskType.Move)
                    {
                        MoveToStationTask moveTask = (MoveToStationTask)task;
                        row.SetDropdownSelected(moveTask.targetStation.name);
                    }
                    else if (task.taskType == AgentTaskType.Wait)
                    {
                        WaitTask waitTask = (WaitTask)task;
                        int waitTime = (int)waitTask.waitTime;
                        row.SetDropdownSelected(waitTime);
                    }
                    */
                }
            }
            addTaskRow.transform.SetAsLastSibling();
            changeScheduleButton.transform.SetAsLastSibling();
            SetUpDropdown();
            int index = 0;
            if (tasks != null)
            {
                foreach (var row in uiTrainRows)
                {
                    if(tasks[index].taskType == AgentTaskType.Move)
                    {
                        MoveToStationTask moveTask = (MoveToStationTask)tasks[index];
                        row.SetDropdownSelected(moveTask.targetStation.StationName);
                    }
                    else if(tasks[index].taskType == AgentTaskType.Wait)
                    {
                        WaitTask waitTask = (WaitTask)tasks[index];
                        int waitTime = (int)waitTask.waitTime;
                        row.SetDropdownSelected(waitTime);
                    }
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
                if(item.GetComponent<AgentTaskTypeHolder>().taskType == AgentTaskType.Move)
                    item.SetDropdownOptions(stationNames);
            }
        }

        internal void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}
