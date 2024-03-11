using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GateSwitch : MonoBehaviour, Interactable
{
    // components
    Animation animationComp;

    [SerializeField] ColliderArea area;
    [SerializeField] Player target;
    [SerializeField] Vector3 inputPosition;
    [SerializeField] Vector2 inputRotation;
    [SerializeField] float targetLerpSpeed;
    bool isMovingTarget;

    Vector3 targetPosition;
    Vector2 targetRotation;

    Vector3 cachedPosition;
    Vector2 cachedRotation;
    bool isOpen;

    void Awake()
    {
        animationComp = GetComponent<Animation>();
    }

    public bool IsUseable()
    {
        return area.containsPlayer;
    }

    public async void Use()
    {
        if (isOpen)
            return;
        isOpen = true;

        //isMovingTarget = true;
        //target.canMove = false;
        //target.canTurnCamera = false;
        //target.animator.Play("Player_Cutscene_GateSwitchOn");

        //targetPosition = inputPosition;
        //targetRotation = inputRotation;

        //cachedPosition = target.transform.position;
        //cachedRotation.x = target.GetHorizontalRotation();
        //cachedRotation.y = target.GetVerticalRotation();

        animationComp.Play();
        await Task.Delay(150);
        AudioManager.PlaySound("LightSwitch");
        //await Task.Delay(2000);
        //FinishAnimation();
    }

    async void FinishAnimation()
    {
        target.canMove = true;
        target.canTurnCamera = true;

        targetPosition = cachedPosition;
        targetRotation = cachedRotation;

        await Task.Delay(500);

        isMovingTarget = false;
    }

    void Update()
    {
        //if (isMovingTarget)
        //{
        //    target.SetPosition(Vector3.Lerp(target.transform.position, targetPosition, Time.deltaTime * targetLerpSpeed));
        //    target.SetHorizontalRotation(Mathf.LerpAngle(target.GetHorizontalRotation(), targetRotation.x, targetLerpSpeed * Time.deltaTime));
        //    target.SetVerticalRotation(Mathf.LerpAngle(target.GetVerticalRotation(), targetRotation.y, targetLerpSpeed * Time.deltaTime));
        //}
    }

    public void Animation_PlayOpenSound()
    {
        AudioManager.PlaySound("OpenGate");
    }
}
