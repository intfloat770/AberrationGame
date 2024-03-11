using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour, Shootable
{
    [SerializeField] Rigidbody doorRigidBody;
    [SerializeField] Vector3 force;
    [SerializeField] Vector3 force2;
    bool wasHit;

    public void OnHit(int damage)
    {
        if (wasHit) 
            return;
        wasHit = true;

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.AddForce(force2);

        doorRigidBody.isKinematic = false;
        doorRigidBody.AddForce(force);
    }
}
