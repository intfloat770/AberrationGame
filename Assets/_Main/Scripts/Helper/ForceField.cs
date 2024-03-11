using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    List<Rigidbody> rbs = new List<Rigidbody>();
    [SerializeField] float force;

    private void FixedUpdate()
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.AddForce(transform.forward * force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.attachedRigidbody.CompareTag("Door"))
            return;

        rbs.Add(other.attachedRigidbody);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.attachedRigidbody.CompareTag("Door"))
            return;
        
        rbs.Remove(other.attachedRigidbody);
    }
}
