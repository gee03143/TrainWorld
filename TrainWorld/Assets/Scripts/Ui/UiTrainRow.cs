using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using TrainWorld.AI;

namespace TrainWorld
{
    public class UiTrainRow : MonoBehaviour
    {
        public Action<UiTrainRow> onDestroy;

        [SerializeField]
        Dropdown dropdown;
        [SerializeField]
        Button deleteButton;

        private void Awake()
        {
            deleteButton.onClick.AddListener(DestroyMyself);
        }

        public void DestroyMyself()
        {
            onDestroy?.Invoke(this);
            Destroy(gameObject);
        }

        internal void SetDropdownSelected(string value)
        {
            List<string> options = dropdown.options.Select(option => option.text).ToList();
            if (options.Contains(value) == false)
                DestroyMyself();
            dropdown.value = dropdown.options.Select(option => option.text).ToList().IndexOf(value);
        }

        internal void SetDropdownSelected(int value)
        {
            dropdown.value = value - 1; // option starts from 0
        }

        internal string GetDropdownSelected()
        {
            if(dropdown == null) // for load/unload task
            {
                return "";
            }
            return dropdown.options[dropdown.value].text;
        }

        internal void SetDropdownOptions(List<string> stationNameList)
        {
            dropdown.ClearOptions();
            foreach (var name in stationNameList)
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = name;
                dropdown.options.Add(option);
            }
        }
    }
}
