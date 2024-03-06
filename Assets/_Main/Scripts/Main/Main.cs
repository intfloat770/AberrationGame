using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Cam cam;

    private void Awake()
    {
        // init player
        player.Init();
        cam.Init();

        // hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
