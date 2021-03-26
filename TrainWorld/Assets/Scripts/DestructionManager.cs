using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rail;
using TrainWorld.Station;
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

        private Direction8way cursorDirection;

        private void Awake()
        {
            cursorDirection = Direction8way.N;
        }

        public void OnEnter()
        {
            Debug.Log("Destruction Enter");
        }

        public void OnExit()
        {
            railPlacementManager.HideRailArrowUI();
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

                }else if(selectableObject.GetSelectableObjectType() == SelectableObjectType.Rail)
                {
                    railPlacementManager.RemoveRail(position);
                    railPlacementManager.HideRailArrowUI();
                }
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
                railPlacementManager.HideRailArrowUI();
                if (selectableObject.GetSelectableObjectType() == SelectableObjectType.Rail)
                {
                    railPlacementManager.MoveCursorAtDestruction(mousePosition);
                }
            }
            else
            {
                Debug.Log("No Selectable object found");
            }
        }

        public void OnRInput()
        {
            //
        }
    }
}
