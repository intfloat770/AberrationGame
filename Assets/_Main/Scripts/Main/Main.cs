using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Cam cam;
    [SerializeField] AudioManager audioManager;
    [SerializeField] Canvas canvas;

    private async void Awake()
    {
        // init
        audioManager.Init();
        player.Init();
        cam.Init();

        // hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        canvas.gameObject.SetActive(false);
        await Task.Delay(10);
        canvas.gameObject.SetActive(true);
    }
}
