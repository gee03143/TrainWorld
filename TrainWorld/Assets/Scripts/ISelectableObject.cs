using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace TrainWorld
{
    public enum SelectableObjectType
    {
        Rail,
        Agent,
        Station,
        Traffic,
        Building
    }

    public interface ISelectableObject
    {
        SelectableObjectType GetSelectableObjectType();

        void ShowMyUI();
    }
}
