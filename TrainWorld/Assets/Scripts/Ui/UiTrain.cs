using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TrainWorld.Station;
using TrainWorld.AI;

namespace TrainWorld
{
    public class UiTrain : MonoBehaviour
    {
        [SerializeField]
        StationPlacementManager stationPlacementManager;

        [SerializeField]
        Dropdown destination1;

        [SerializeField]
        Dropdown destination2;

        [SerializeField]
        Button changeDestinationButton;

        [SerializeField]
        private PathVisualizer pathVisualizer;

        AiAgent selectedAi;

        private void Awake()
        {
            changeDestinationButton.onClick.AddListener( ChangeDestinationHandler);
        }

        private void ChangeDestinationHandler()
        {
            TrainStation dest1 = stationPlacementManager.GetStationOfName(destination1.options[destination1.value].text);
            TrainStation dest2 = stationPlacementManager.GetStationOfName(destination2.options[destination2.value].text);

            if (selectedAi != null && dest1 != null && dest2 != null)
            {
                selectedAi.SetUpSchedule(new List<TrainStation> { dest1, dest2 });
            }
            else if (dest1 == null)
            {
                Debug.Log("dest1 is null" + destination1.options[destination1.value].text);
            }
            else if (dest2 == null)
            {
                Debug.Log("dest2 is null" + destination2.options[destination2.value].text);
            }
        }

        public void SetSelectedAi(ISelectableObject selected)
        {
            selectedAi = (AiAgent)selected;
        }

        public void SetUpDropdown(List<string> stationNameList)
        {
            destination1.ClearOptions();
            destination2.ClearOptions();
            foreach (var name in stationNameList)
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = name;
                destination1.options.Add(option);
                destination2.options.Add(option);
            }
        }

        internal void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}
