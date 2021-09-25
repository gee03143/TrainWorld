using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Buildings
{
    public class BuildingColorChanger : MonoBehaviour
    {
        [SerializeField]
        Material redMaterial;

        MeshRenderer meshRenderer;
        Material defaultMaterial;

        private void Awake()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            defaultMaterial = meshRenderer.material;
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.tag);
            if (other.gameObject.CompareTag("Rail") || other.gameObject.CompareTag("Building"))
            {
                ChangeColorToRed();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Rail") || other.gameObject.CompareTag("Building"))
            {
                ChangeColorToDefault();
            }
        }

        public void ChangeColorToRed()
        {
            meshRenderer.material = redMaterial;
        }

        public void ChangeColorToDefault()
        {
            meshRenderer.material = defaultMaterial;
        }
    }
}
