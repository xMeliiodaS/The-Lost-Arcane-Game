
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterGate : MonoBehaviour
{
    [SerializeField] private Transform destGate;        // Reference to the new destination

    /// <summary>
    /// Teleport the player from one place to another
    /// </summary>
    /// <param name="other">The player's reference</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // Deactivating the Player to make sure nothing interfering with the player's teleportation
            other.gameObject.SetActive(false);
            //other.transform.parent.gameObject.SetActive(false);

            other.transform.parent.position = new Vector3(destGate.position.x, destGate.position.y, destGate.position.z);

            other.gameObject.SetActive(true);
            //other.transform.parent.gameObject.SetActive(true);
        }
    }
}
