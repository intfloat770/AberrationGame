using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LightObject : MonoBehaviour
{
    Light lightRef;

    int state;

    [SerializeField] float minFlickerTime;
    [SerializeField] float maxFlickerTime;
    [SerializeField] float minFlickerDuration;
    [SerializeField] float maxFlickerDuration;
    [SerializeField, Range(0f, 1f)] float chanceToReTrigger;
    float flickerCooldown;

    private void Awake()
    {
        lightRef = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        flickerCooldown -= Time.deltaTime;
        
        if (state == 0)
        {
            if (flickerCooldown < 0)
            {
                lightRef.gameObject.SetActive(false);
                flickerCooldown = Random.Range(minFlickerDuration, maxFlickerDuration);
                state = 1;
            }
        }

        if (state == 1)
        {
            if (flickerCooldown < 0)
            {
                lightRef.gameObject.SetActive(true);
                if (Random.value < chanceToReTrigger)
                    flickerCooldown = .1f;
                else
                    flickerCooldown = Random.Range(minFlickerTime, maxFlickerTime);
                state = 0;
            }
        }
    }
}
