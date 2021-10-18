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
        private GameObject providerPrefab;
        [SerializeField]
        private GameObject consuerPrefab;
        [SerializeField]
        private GameObject tempBuildingParent;

        private List<GameObject> prefabList;

        [SerializeField]
        private List<GameObject> tempBuildingList;
        private int listIndex = 0;

        public void Awake()
        {
            prefabList = new List<GameObject> { buildingPrefab , fullBuildingPrefab , providerPrefab, consuerPrefab };
        }

        public void OnEnter()
        {
            Debug.Log("Building Placement Enter");
            SetTempbuildingActive(0);
        }

        public void OnExit()
        {
            Debug.Log("Building Placement Exit");
            SetTempbuildingUnvisible();
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

            newObject = Instantiate(prefabList[listIndex], roundedPosition, Quaternion.Euler(Direction8way.N.ToEuler()), buildingParent);
            newBuilding = newObject.GetComponent<Building>();

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
            tempBuildingParent.transform.position = roundedPosition;
        }

        public void OnRInput()
        {
            listIndex++;
            if(listIndex >= prefabList.Count)
            {
                listIndex = 0;
            }

            SetTempbuildingActive(listIndex);
        }

        private void SetTempbuildingActive(int index)
        {
            SetTempbuildingUnvisible();
            tempBuildingList[index].SetActive(true);
        }

        private void SetTempbuildingUnvisible()
        {
            foreach (var item in tempBuildingList)
            {
                item.SetActive(false);
            }
        }
    }
}
