using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiRail : MonoBehaviour
{
    [SerializeField]
    private GameObject arrowA;
    [SerializeField]
    private GameObject arrowB;

    private void OnMouseExit()
    {
        arrowA.SetActive(false);
        arrowA.SetActive(false);
    }

    public void SetArrowVisibility(bool visibility)
    {
        arrowA.SetActive(visibility);
        arrowB.SetActive(visibility);
    }
}
