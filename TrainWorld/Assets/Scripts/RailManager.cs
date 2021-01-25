using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class RailManager : MonoBehaviour
    {
        [SerializeField]
        private RailGraph railGraph;

        [SerializeField]
        private PlacementManager placementManager;

        Direction tempDirection;
        [SerializeField]
        GameObject tempObject;

        private bool placementMode;

        private Vector3Int placementStartPosition;
        private Vector3Int placementEndPosition;
        private Direction placementStartDirection;
        private Direction placementEndDirection;


        internal void RotateTempRail(Vector3Int cursorPosition)
        {
            if (placementMode)
                return;

            tempDirection = DirectionHelper.Next(tempDirection);
        }

        internal void DisplayTempObjects(Vector3 cursorPosition)
        {
            if (placementMode)
            {
                DisplayTempObjectFromStartPosition(cursorPosition);
            }
            else
            {
                DisplayTempObjectFromCursorPosition(cursorPosition);
            }
        }

        private void DisplayTempObjectFromCursorPosition(Vector3 cursorPosition)
        {
            Vector3Int roundedPosition = Vector3Int.RoundToInt(cursorPosition);

            //tempObject.GetComponent<Rail>().InitRail(roundedPosition, roundedPosition, placementStartDirection, placementStartDirection);

            Type? data = placementManager.GetPlacementDataAt(roundedPosition);
            if (data != null) // pointer is over object
            {
               //tempObject.SetActive(false);
            }
            else
            {
                //tempObject.SetActive(true);
            }
        }

        private void DisplayTempObjectFromStartPosition(Vector3 cursorPosition)
        {
            Vector3Int deltaPosition = Vector3Int.RoundToInt(cursorPosition - placementStartPosition);
            Vector3 crossProduct = Vector3.Cross(DirectionHelper.ToDirectionVector(placementStartDirection), deltaPosition).normalized;
            Vector3Int directionVector = DirectionHelper.ToDirectionVector(placementStartDirection);
            if (crossProduct == Vector3Int.up)
            {
                placementEndPosition = placementStartPosition + 
                    directionVector + DirectionHelper.ToDirectionVector(DirectionHelper.Next(placementStartDirection));
                placementEndDirection = DirectionHelper.Opposite(DirectionHelper.Next(placementStartDirection));

            }
            else if (crossProduct == Vector3Int.down)
            {
                placementEndPosition = placementStartPosition +
                    directionVector + DirectionHelper.ToDirectionVector(DirectionHelper.Prev(placementStartDirection));
                placementEndDirection = DirectionHelper.Opposite(DirectionHelper.Prev(placementStartDirection));
            }
            else
            {
                placementEndPosition = placementStartPosition + directionVector;
                placementEndDirection = DirectionHelper.Opposite(placementStartDirection);
            }
            ModifyTempRailTransform();
        }

        void ModifyTempRailTransform()
        {
            Debug.Log("Modify Temp Rail Transform");
            tempObject.GetComponent<Rail>().InitRail(placementStartPosition, placementEndPosition, placementStartDirection, placementEndDirection);
        }

        public void PlaceRail(Vector3 position)
        {
            Type? data = placementManager.GetPlacementDataAt(Vector3Int.RoundToInt(position));
            if (placementMode)  // at placementMode
            {
                if (data == null)   //   click empty space, place rail
                {
                    railGraph.AddEdge(placementStartPosition, placementEndPosition,
                        placementStartDirection, placementEndDirection);

                    placementManager.AddTempObjectToDictionary(placementStartPosition, Type.Rail);
                    placementManager.AddTempObjectToDictionary(placementEndPosition, Type.Rail);
                } // else, cancle placementMode
                placementMode = false;
            }
            else
            {
                if (data == null) // click empty space, place deadend
                {
                    railGraph.AddNode(Vector3Int.RoundToInt(position), tempDirection);

                    placementManager.AddTempObjectToDictionary(Vector3Int.RoundToInt(position), Type.Rail);
                }
                else if (data == Type.Rail)   // click rail -> enter placementMode
                {
                    RailNode railNode = railGraph.GetNodeAt(Vector3Int.RoundToInt(position));   

                    placementMode = true;
                    placementStartDirection = railNode.GetClosestExtendDirection(position);

                    placementStartPosition = Vector3Int.RoundToInt(position);
                }
                // else ignore mouse click
            }
        }

    }

}