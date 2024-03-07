using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Anomaly : MonoBehaviour
{
    [SerializeField] Transform graphics;
    [SerializeField] float optimalDistance;
    [SerializeField] float scaleImpact;
    [SerializeField] bool constraintToOrigin;
    [SerializeField] Transform referenceTransform;
    [SerializeField] Vector3 offset;
    [SerializeField] float heightMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 toOrigin = (transform.position - referenceTransform.position).normalized;
        float distance = Vector3.Distance(transform.position, referenceTransform.position);

        Debug.Log(distance);
            graphics.position = referenceTransform.position + toOrigin * optimalDistance + offset + Vector3.up * distance * heightMultiplier;
        if (distance - optimalDistance > 0)
        {
            //graphics.localScale = Vector3.one;
        }
        else if (constraintToOrigin)
        {
            graphics.position = transform.position;
            //graphics.localScale = Vector3.one * distance * scaleImpact;
        }
    }
}
