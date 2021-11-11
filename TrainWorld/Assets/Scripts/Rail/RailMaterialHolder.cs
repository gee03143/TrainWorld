using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrainWorld.Rails
{
    public class RailMaterialHolder : MonoBehaviour
    {
        public Material railMaterial;

        public MeshRenderer[] railRenderer;


        public void SetMaterialToDefault()
        {
            foreach (MeshRenderer renderer in railRenderer)
            {
                renderer.material = railMaterial;
            }
        }

        public void SetMaterial(Material input)
        {
            foreach (MeshRenderer renderer in railRenderer)
            {
                renderer.material = input;
            }
        }
    }
}