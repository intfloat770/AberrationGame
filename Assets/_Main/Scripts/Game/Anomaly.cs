using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Anomaly : MonoBehaviour
{
    Rigidbody rb;

    [Header("Setup")]
    [SerializeField] Transform graphics;
    //[SerializeField] float optimalDistance;
    //[SerializeField] float scaleImpact;
    //[SerializeField] bool constraintToOrigin;
    //[SerializeField] Transform referenceTransform;
    //[SerializeField] Vector3 offset;
    //[SerializeField] float heightMultiplier;

    //[SerializeField] Transform test01;
    //[SerializeField] Transform test02;
    [SerializeField] float targetScreenHeight;
    [SerializeField] float maxWorldSize;
    [SerializeField] float gravity;
    //[SerializeField] float maxWorldScale;

    [SerializeField] bool useLight;
    [SerializeField] Light lightRef;
    //[SerializeField] float maxIntensity;

    [Header("Colors")]
    [SerializeField] Material chillMaterial;
    [SerializeField] Material chillMaterial2;
    [SerializeField] Material enragedMaterial;
    [SerializeField] Material enragedMaterial2;
    [SerializeField] Color chillLightColor;
    [SerializeField] Color enragedLightColor;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshRenderer meshRenderer2;
    bool isEnraged;

    [Header("Impulses")]
    [SerializeField] float initialDelay;
    [SerializeField] float maxDistance;
    [SerializeField] AnimationCurve distanceCurve;
    [SerializeField] float distanceMultiplier;
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] float jumpStrength;
    [SerializeField] AnimationCurve spinCurve;
    [SerializeField] float spinStrength;
    [SerializeField] AnimationCurve impulseCurve;
    [SerializeField] float impulseStrength;
    float impulseTime;
    float delta;
    float percent;

    [Header("Health")]
    public int health;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer.material = chillMaterial;
        meshRenderer2.material = chillMaterial2;
    }

    // Update is called once per frame
    void Update()
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
        graphics.localScale = Vector3.one * Mathf.Clamp((targetScreenHeight * distance) / 540f, 0, maxWorldSize);

        // light
        if (useLight)
        {
            //lightRef.intensity = scale / maxWorldScale * maxIntensity;
            lightRef.transform.position = transform.position + Vector3.up * .1f;
        }

        //Debug.Log($"size on screen: {sizeOnScreen}, delta: {delta}, percent: {percent}");

        if (isEnraged)
        {
            impulseTime -= Time.deltaTime;
            delta = percent;
            percent = distance / maxDistance;
            delta = delta - percent;
            impulseTime *= Mathf.Clamp01(1 - delta * distanceMultiplier);

            if (impulseTime < 0)
            {
                float force = distanceCurve.Evaluate(percent) * distanceMultiplier;
                rb.AddForce(Random.onUnitSphere * spinCurve.Evaluate(percent) * jumpStrength);
                rb.AddTorque(Random.onUnitSphere * spinCurve.Evaluate(percent) * spinStrength);
                impulseTime = impulseCurve.Evaluate(percent) * impulseStrength;
            }
        }
        else
        {
            rb.AddForce(Vector3.down * gravity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        isEnraged = true;
        meshRenderer.material = enragedMaterial;
        meshRenderer2.material = enragedMaterial2;
        lightRef.color = enragedLightColor;
        impulseTime = initialDelay;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        isEnraged = false;
        meshRenderer.material = chillMaterial;
        meshRenderer2.material = chillMaterial2;
        lightRef.color = chillLightColor;
    }

    public void OnTakeDamage()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
