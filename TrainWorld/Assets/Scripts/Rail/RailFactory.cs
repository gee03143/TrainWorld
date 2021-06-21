using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Rails
{
    public class RailFactory
    {
        public static Dictionary<RailType, GameObject> railDictionary;

        static RailFactory()
        {
            // We can load all the animals from that folder.
            var rails = Resources.LoadAll<GameObject>("Prefabs/Rail");
            railDictionary = new Dictionary<RailType, GameObject>(rails.Length);

            foreach (GameObject rail in rails)
            {
                var typeHolder = rail.GetComponent<RailTypeHolder>();
                if (typeHolder != null)
                {
                    railDictionary.Add(typeHolder.type, rail);
                }
            }
        }

        public static void SpawnRail(RailType railType, Vector3Int position, Direction8way direction, Transform parent)
        {
            if (railDictionary.ContainsKey(railType))
            {
                Object.Instantiate(railDictionary[railType], position, Quaternion.Euler(direction.ToEuler()), parent.transform);
            }
            else
            {
                Debug.LogError("Rail with " + railType + "could not be " +
                    "found and spawned.");
            }
        }
    }
}
