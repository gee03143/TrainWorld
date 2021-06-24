using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrainWorld.Rails
{
    public class RailMaterialHolder : MonoBehaviour
    {
        public Material railMaterial;

        public Material sideMaterial;

        public MeshRenderer railRenderer;

        public MeshRenderer sideRenderer;

        public void SetMaterialToDefault()
        {
            railRenderer.material = railMaterial;
            sideRenderer.material = sideMaterial;
        }

        public void SetMaterial(Material input)
        {
            railRenderer.material = input;
            sideRenderer.material = input;
        }
    }
}