using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TrainWorld
{
    [CustomEditor(typeof(RailCreator))]
    public class RailEditor : Editor
    {
        RailCreator creator;


        private void OnSceneGUI()
        {
            if(creator.autoUpdate && Event.current.type == EventType.Repaint)
            {
                creator.UpdateRail(); 
            }
        }

        private void OnEnable()
        {
            creator = (RailCreator)target;
        }

    }
}