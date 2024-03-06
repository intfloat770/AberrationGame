using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] Player player;

    private void Awake()
    {
        // init player
        player.Init();

        // hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
