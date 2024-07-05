using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftDetector : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Forklift"))
        {
            Debug.Log("forklift inside");
        }
    }
}
