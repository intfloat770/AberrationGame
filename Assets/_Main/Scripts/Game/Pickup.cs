using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.Inverse(Camera.main.transform.rotation * Quaternion.Euler(offset));
        //transform.rotation = Camera.main.transform.rotation;
    }
}
