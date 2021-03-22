using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class NeuturalStateManager : MonoBehaviour, InputHandler
    {
        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private UiStationData uiStationData;

        public void OnClick(Vector3Int position)
        {
            //raycast hit
            ISelectableObject selectableObject = GetObjectFromPointer();
            if (selectableObject != null)
            {
                if (selectableObject.GetSelectableObjectType() == SelectableObjectType.Station)
                {
                    uiStationData.SetActive(true);
                    uiStationData.GetStationReference(selectableObject);
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

        private ISelectableObject GetObjectFromPointer()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                return hit.rigidbody.gameObject.GetComponent<ISelectableObject>();
            }
            return null;
        }

        public void CloseUI()
        {
            uiStationData?.SetActive(false);
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
