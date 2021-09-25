using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Buildings
{
    public class BuildingPlacementManager : MonoBehaviour, InputHandler
    {
        [SerializeField]
        private Transform buildingParent;
        [SerializeField]
        private GameObject buildingPrefab;
        [SerializeField]
        private GameObject fullBuildingPrefab;
        [SerializeField]
        private GameObject tempBuildingObject;

        bool placingFullBuilding = false;

        public void OnEnter()
        {
            Debug.Log("Building Placement Enter");
            tempBuildingObject.SetActive(true);
            tempBuildingObject.GetComponent<Building>().Init(new Vector3Int(0, 0, 0));
        }

        public void OnExit()
        {
            Debug.Log("Building Placement Exit");
            tempBuildingObject.SetActive(false);
        }

        public void OnMouseDown(Vector3 mousePosition)
        {
            PlaceBuilding(mousePosition);
        }

        private void PlaceBuilding(Vector3 mousePosition)
        {
            Vector3Int roundedPosition = Vector3Int.RoundToInt(mousePosition);

            GameObject newObject;
            Building newBuilding;

            if (placingFullBuilding)
            {
                newObject = Instantiate(fullBuildingPrefab, roundedPosition, Quaternion.Euler(Direction8way.N.ToEuler()), buildingParent);
                newBuilding = newObject.GetComponent<Building>();
            }
            else
            {
                newObject = Instantiate(buildingPrefab, roundedPosition, Quaternion.Euler(Direction8way.N.ToEuler()), buildingParent);
                newBuilding = newObject.GetComponent<Building>();
            }

            
            newBuilding.Init(roundedPosition);
            Vector3Int minBuildingPos = newBuilding.GetMinPosition();
            Vector3Int maxBuildingPos = newBuilding.GetMaxPosition();


            //check if selected position is empty
            bool isPositionPlacable = true;
            for (int i = minBuildingPos.x; i <= maxBuildingPos.x; i++)
            {
                for (int j = minBuildingPos.z; j <= maxBuildingPos.z; j++)
                {
                    if(PlacementManager.GetPlacementTypeAt(new Vector3Int(i,0,j)) != PlacementType.eEmpty)
                        isPositionPlacable = false;
                }
            }
            if (isPositionPlacable)
            {

                for (int i = minBuildingPos.x; i <= maxBuildingPos.x; i++)
                {
                    for (int j = minBuildingPos.z; j <= maxBuildingPos.z; j++)
                    {
                        PlacementManager.AddBuildingAt(new Vector3Int(i, 0, j));
                    }
                }
            }
            else
            {
                Destroy(newObject);
            }
        }

        public void OnMouseMove(Vector3 mousePosition)
        {
            //throw new System.NotImplementedException();

            //show temp building

            Vector3Int roundedPosition = Vector3Int.RoundToInt(mousePosition);
            //visual red when not placeable
            tempBuildingObject.transform.position = roundedPosition;
        }

        public void OnRInput()
        {
            placingFullBuilding = !placingFullBuilding;
        }
    }
}
