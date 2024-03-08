using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeRotation : MonoBehaviour
{
    [SerializeField] Vector3 constant;
    [SerializeField] Vector3 influence;

    // Start is called before the first frame update
    void Start()
    {
        transform.localRotation = Quaternion.Euler(Random.Range(0, 16) * influence * 90 + constant);
        Destroy(this, Random.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
