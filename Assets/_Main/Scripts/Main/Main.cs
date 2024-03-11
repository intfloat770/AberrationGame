using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

enum GameState
{
    tunnel,
    startingTunnelOpenAnimation,
}

public class Main : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Player player;
    [SerializeField] Cam cam;
    [SerializeField] AudioManager audioManager;
    [SerializeField] Canvas canvas;

    GameState gameState;
    [Header("Game Start")]
    [SerializeField] bool startNormally;
    [SerializeField] Vector3 playerStartPosition;

    [Header("Tunnel")]
    [SerializeField] GateSwitch tunnelSwitchArea;
    [SerializeField] Vector3 tunnelSwitchTargetPosition;
    [SerializeField] float tunnelSwitchTargetRotation;

    private async void Awake()
    {
        // init
        audioManager.Init();
        player.Init();
        cam.Init();

        // hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        gameState = GameState.tunnel;
        if (startNormally)
        {
            player.transform.position = playerStartPosition;
            player.transform.rotation = Quaternion.identity;
        }
        else
        {
            player.PickupGun();
        }

        canvas.gameObject.SetActive(false);
        await Task.Delay(10);
        canvas.gameObject.SetActive(true);
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.tunnel:
                State_Tunnel();
                break;
            case GameState.startingTunnelOpenAnimation:
                break;
            default:
                break;
        }
    }

    void State_Tunnel()
    {
        if (Input.GetKeyDown(KeyCode.E) && tunnelSwitchArea.IsUseable())
        {
            tunnelSwitchArea.Use();
        }
    }

    //void State_Tun
}
