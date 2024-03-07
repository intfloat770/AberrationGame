using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Cam cam;
    [SerializeField] AudioManager audioManager;

    private void Awake()
    {
        // init
        audioManager.Init();
        player.Init();
        cam.Init();

        // hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
