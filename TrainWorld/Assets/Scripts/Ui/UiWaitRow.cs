using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TrainWorld
{

    public class UiWaitRow : MonoBehaviour
    {
        public Action<UiWaitRow> onDestroy;

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

        internal void SetDropdownSelected(int value)
        {
            dropdown.value = value;
        }

        internal string GetDropdownSelected()
        {
            return dropdown.options[dropdown.value].text;
        }

        private void OnDestroy()
        {
            onDestroy?.Invoke(this);
        }
    }
}
