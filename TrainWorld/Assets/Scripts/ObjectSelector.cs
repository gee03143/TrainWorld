using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrainWorld
{
    public static class ObjectSelector
    {
        private static LayerMask layerMask = 1 << LayerMask.NameToLayer("Selectable");

        public static ISelectableObject GetObjectFromPointer()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                return hit.rigidbody.gameObject.GetComponent<ISelectableObject>();
            }
            return null;
        }
    }
}
