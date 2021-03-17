using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class NeuturalStateManager : MonoBehaviour, InputHandler
    {
        public void OnClick(Vector3Int position)
        {
            Debug.Log("clicked Position" + position.ToString());
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
