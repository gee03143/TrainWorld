using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Rails
{
    public class RailArrowUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject arrow;

        public void ShowRailUI(Rail railUnderCursor)
        {
            if (railUnderCursor != null)
            {
                arrow.SetActive(true);
                arrow.transform.position = railUnderCursor.Position;

                arrow.transform.rotation = Quaternion.Euler(railUnderCursor.Direction.ToEuler());
            }
            else
            {
                HideRailArrowUI();
            }
        }

        public void HideRailArrowUI()
        {
            arrow.SetActive(false);
        }
    }
}
