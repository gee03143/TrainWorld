using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rails;
using TrainWorld.Traffic;
using TrainWorld.AI;

namespace TrainWorld
{
    public class DestructionManager : MonoBehaviour, InputHandler
    {
        [SerializeField]
        private RailPlacementManager railPlacementManager;
        [SerializeField]
        private StationPlacementManager stationPlacementManager;
        [SerializeField]
        private TrainPlacementManager trainPlacementManager;
        [SerializeField]
        private TrafficPlacementManager trafficPlacementManager;
        [SerializeField]
        private RailBlockManager railBlockManager;
        [SerializeField]
        private RailArrowUI railArrowUI;

        public void OnEnter()
        {
            railBlockManager.ShowRailBlockDisplay();
            Debug.Log("Destruction Enter");
        }

        public void OnExit()
        {
            railArrowUI.HideRailArrowUI();
            railBlockManager.DisableRailBlockDisplay();
            Debug.Log("Destruction Exit");
        }

        public void OnMouseDown(Vector3 position)
        {
            //raycast hit
            ISelectableObject selectableObject = ObjectSelector.GetObjectFromPointer();
            if (selectableObject != null)
            {
                if(selectableObject.GetSelectableObjectType() == SelectableObjectType.Agent)
                {
                    trainPlacementManager.RemoveAgent((AiAgent)selectableObject);
                }else if (selectableObject.GetSelectableObjectType() == SelectableObjectType.Station)
                {
                    stationPlacementManager.RemoveStation((TrainStation)selectableObject);
                }else if(selectableObject.GetSelectableObjectType() == SelectableObjectType.Rail)
                {
                    railPlacementManager.RemoveRail(position);
                    railArrowUI.HideRailArrowUI();
                    
                }else if(selectableObject.GetSelectableObjectType() == SelectableObjectType.Traffic){
                    trafficPlacementManager.RemoveTraffic((TrafficSignal)selectableObject);
                }
                railBlockManager.ShowRailBlockDisplay();
            }
            else
            {
                Debug.Log("No Selectable object found");
            }
        }

        public void OnMouseMove(Vector3 mousePosition)
        {
            ISelectableObject selectableObject = ObjectSelector.GetObjectFromPointer();
            if (selectableObject != null)
            {
                railArrowUI.HideRailArrowUI();
                if (selectableObject.GetSelectableObjectType() == SelectableObjectType.Rail)
                {
                    railPlacementManager.MoveCursorAtDestruction(mousePosition);
                }
            }
        }

        public void OnRInput()
        {
            //
        }
    }
}
