using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    bool played;

    private void OnCollisionEnter(Collision collision)
    {

        if (played)
            return;

        played = true;

        AudioManager.PlaySound("ShellBounce");
        //Debug.Log("f");
    }
}
