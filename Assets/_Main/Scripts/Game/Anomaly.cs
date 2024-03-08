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

    [SerializeField] Transform test01;
    [SerializeField] Transform test02;
    [SerializeField] float targetScreenHeight;
    [SerializeField] float maxWorldScale;

    [SerializeField] bool useLight;
    [SerializeField] Light lightRef;
    [SerializeField] float maxIntensity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Vector3 toOrigin = (transform.position - referenceTransform.position).normalized;
        //float distance = Vector3.Distance(transform.position, referenceTransform.position);

        ////Debug.Log(distance);
        ////graphics.position = referenceTransform.position + toOrigin * optimalDistance + offset + Vector3.up * distance * heightMultiplier;
        //if (distance - optimalDistance > 0)
        //{
        ////graphics.localScale = Vector3.one;
        //}
        //else if (constraintToOrigin)
        //{
        //graphics.position = transform.position;
        //}

        ////float scale = Mathf.Clamp(distance * scaleImpact, 1, 100);
        //graphics.localScale = Vector3.one * distance * scaleImpact;

        //Vector2 sizeOnScreen = Camera.main.WorldToScreenPoint(test01.position) - Camera.main.WorldToScreenPoint(transform.position);
        //float delta = (sizeOnScreen.y * sizeOnScreen.y) - targetScreenHeight;
        //float percent = delta / targetScreenHeight;
        //graphics.localScale = Vector3.one * percent;

        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        float scale = (targetScreenHeight * distance) / 540f;
        graphics.localScale = Vector3.one * scale;

        // light
        if (useLight)
        {
            lightRef.intensity = scale / maxWorldScale * maxIntensity;
        }

        //Debug.Log($"size on screen: {sizeOnScreen}, delta: {delta}, percent: {percent}");
    }
}
