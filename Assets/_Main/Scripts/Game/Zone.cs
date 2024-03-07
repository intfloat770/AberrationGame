using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] AudioReverbPreset preset = AudioReverbPreset.Off;

    private void OnTriggerEnter(Collider other)
    {
        Cam.SetReverbFilter(preset);
    }

    private void OnTriggerExit(Collider other)
    {
        //Cam.SetReverbFilter(AudioReverbPreset.Off);
    }
}
