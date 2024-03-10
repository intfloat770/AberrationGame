using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderArea : MonoBehaviour
{
    public bool containsPlayer {  get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        containsPlayer = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        containsPlayer = false;
    }
}
