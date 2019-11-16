using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightStation : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<LightLevel>().isSafe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<LightLevel>().isSafe = false;
        }
    }
}
