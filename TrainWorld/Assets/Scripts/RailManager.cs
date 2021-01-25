using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class RailManager : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> tempRails;

        [SerializeField]
        private RailGraph railGraph;

        [SerializeField]
        private RailVisualizer railVisualizer;

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
            tempObject.transform.rotation = Quaternion.Euler(DirectionHelper.ToEuler(tempDirection));
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

            tempObject.transform.position = roundedPosition;
            Type? data = placementManager.GetPlacementDataAt(roundedPosition);
            if (data != null) // pointer is over object
            {
                tempObject.SetActive(false);
            }
            else
            {
                tempObject.SetActive(true);
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
                    directionVector * 2 + DirectionHelper.ToDirectionVector(DirectionHelper.Next90(placementStartDirection));
                placementEndDirection = DirectionHelper.Next(placementStartDirection);

            }
            else if (crossProduct == Vector3Int.down)
            {
                placementEndPosition = placementStartPosition +
                    directionVector * 2 + DirectionHelper.ToDirectionVector(DirectionHelper.Prev90(placementStartDirection));
                placementEndDirection = DirectionHelper.Prev(placementStartDirection);
            }
            else
            {
                placementEndPosition = placementStartPosition + directionVector;
                placementEndDirection = placementStartDirection;
            }
            ModifyTempRailTransform(placementStartPosition, placementStartDirection);
        }

        void ModifyTempRailTransform(Vector3Int placementStartPosition, Direction placementStartDirection)
        {
            tempObject.transform.position = placementStartPosition + DirectionHelper.ToDirectionVector(placementStartDirection);
            tempObject.transform.rotation = Quaternion.Euler(DirectionHelper.ToEuler(placementStartDirection));
        }

        public void PlaceRail(Vector3 position)
        {
            if (placementMode)  // at placementMode
            {
                railGraph.AddEdge(placementStartPosition, placementEndPosition,
                    placementStartDirection, placementEndDirection);
                railVisualizer.Visualize(placementStartPosition, placementEndPosition,
                    placementStartDirection, placementEndDirection);

                placementManager.AddTempObjectToDictionary(placementStartPosition, Type.Rail);
                placementManager.AddTempObjectToDictionary(placementEndPosition, Type.Rail);
                placementMode = false;
            }
            else
            {
                Type? data = placementManager.GetPlacementDataAt(Vector3Int.RoundToInt(position));
                if (data == null) // click empty space
                {
                    railGraph.AddNode(Vector3Int.RoundToInt(position), tempDirection);
                    //railVisualizer.VisualizeDeadEnd(placementStartPosition, placementStartDirection);
                    railVisualizer.Visualize(placementStartPosition, placementStartPosition + DirectionHelper.ToDirectionVector(placementStartDirection),
                        placementStartDirection, placementStartDirection);

                    placementManager.AddTempObjectToDictionary(placementStartPosition, Type.Rail);
                }
                else if (data == Type.Rail)   // click rail -> enter placementMode
                {
                    RailNode railNode = railGraph.GetNodeAt(Vector3Int.RoundToInt(position));   

                    placementMode = true;
                    placementStartDirection = railNode.GetClosestExtendDirection(position);

                    placementStartPosition = Vector3Int.RoundToInt(position);
                }
            }
        }

    }

}