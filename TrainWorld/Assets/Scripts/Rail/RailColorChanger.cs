using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Rails
{
    public class RailColorChanger : MonoBehaviour
    {
        [SerializeField]
        List<Material> materials;

        Dictionary<Color, Material> materialDictionary;

        void Awake()
        {
            materialDictionary = new Dictionary<Color, Material>();
            materialDictionary.Add(Color.green, materials[0]);
            materialDictionary.Add(Color.red, materials[1]);
            materialDictionary.Add(Color.white, materials[2]);
            materialDictionary.Add(Color.yellow, materials[3]);
            materialDictionary.Add(Color.blue, materials[4]);
            materialDictionary.Add(Color.black, materials[5]);
        }

        public void ChangeRailColor(Color newColor)
        {
            RailMaterialHolder[] materialHolders;

            materialHolders = GetComponentsInChildren<RailMaterialHolder>();

            foreach (var item in materialHolders)
            {
                item.SetMaterial(materialDictionary[newColor]);
            }
        }

        public void ChangeRailColorToDefault()
        {
            RailMaterialHolder[] materialHolders;

            materialHolders = GetComponentsInChildren<RailMaterialHolder>();

            foreach (var item in materialHolders)
            {
                item.SetMaterialToDefault();
            }
        }
    }
}
