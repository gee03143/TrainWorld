using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrainWorld
{
    public enum SelectableObjectType
    {
        Rail,
        Agent,
        Station
    }

    public interface ISelectableObject
    {
        SelectableObjectType GetSelectableObjectType();

        void ShowMyUI();
    }
}
