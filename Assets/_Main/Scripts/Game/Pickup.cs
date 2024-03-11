using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, Interactable
{
    [SerializeField] ColliderArea area;
    [SerializeField] Player player;
    [SerializeField] GameObject gunProp;
    [SerializeField] GameObject ammo;

    bool isUsed;

    public bool IsUseable()
    {
        if (area == null)
            return false;

        return area.containsPlayer;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsUseable() && !isUsed)
            {
                Use();
            }
        }
    }

    public void Use()
    {
        AudioManager.PlaySound("PickupGun");
        isUsed = true;
        player.PickupGun();
        ammo.SetActive(false);
        gunProp.SetActive(false);
    }
}
