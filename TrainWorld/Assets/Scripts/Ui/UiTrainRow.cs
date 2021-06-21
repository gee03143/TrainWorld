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
            Destroy(gameObject);
        }

        internal void SetDropdownSelected(string value)
        {
            dropdown.value = dropdown.options.Select(option => option.text).ToList().IndexOf(value);
        }

        internal void SetDropdownSelected(int value)
        {
            dropdown.value = value - 1; // option starts from 0
        }

        internal string GetDropdownSelected()
        {
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

        private void OnDestroy()
        {
            onDestroy?.Invoke(this);
        }
    }
}
