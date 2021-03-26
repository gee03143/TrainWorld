using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class NeuturalStateManager : MonoBehaviour, InputHandler
    {
        [SerializeField]
        private UiStationData uiStationData;

        [SerializeField]
        private UiTrain uiTrain;

        public void OnMouseDown(Vector3 position)
        {
            //raycast hit
            ISelectableObject selectableObject = ObjectSelector.GetObjectFromPointer();
            if (selectableObject != null)
            {
                if (selectableObject.GetSelectableObjectType() == SelectableObjectType.Station)
                {
                    uiStationData.SetActive(true);
                    uiStationData.SetSelectedStation(selectableObject);
                    uiStationData.DisplaySelectedStationData();
                }else if (selectableObject.GetSelectableObjectType() == SelectableObjectType.Agent)
                {
                    uiTrain.SetActive(true);
                    uiTrain.SetSelectedAi(selectableObject);
                }
                else
                {
                    selectableObject.ShowMyUI();
                }
            }
            else
            {
                Debug.Log("No Selectable object found");
            }
        }

        public void OnMouseMove(Vector3 mousePosition)
        {
            //throw new System.NotImplementedException();
        }

        public void OnRInput()
        {
            //throw new System.NotImplementedException();
        }

        public void CloseUI()
        {
            uiStationData?.SetActive(false);
            uiTrain?.SetActive(false);
        }

        public void OnEnter()
        {
            Debug.Log("Neutural State Enter");
        }

        public void OnExit()
        {
            Debug.Log("Neutural State Exit");
        }
    }
}
