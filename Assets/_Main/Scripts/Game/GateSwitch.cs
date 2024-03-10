using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GateSwitch : MonoBehaviour, Interactable
{
    [SerializeField] ColliderArea area;
    [SerializeField] Player target;
    [SerializeField] Vector3 inputPosition;
    [SerializeField] Vector2 inputRotation;
    [SerializeField] float targetLerpSpeed;
    bool isMovingTarget;

    [SerializeField] Vector3 targetPosition;
    [SerializeField] Vector2 targetRotation;

    Vector3 cachePosition;
    Vector2 cacheRotation;

    public bool IsTargetInTrigger()
    {
        return area.containsPlayer;
    }

    public async void MoveTargetToPosition()
    {
        isMovingTarget = true;
        target.canMove = false;
        target.canTurnCamera = false;
        target.animator.Play("Player_Cutscene_GateSwitchOn");

        targetPosition = inputPosition;
        targetRotation = inputRotation;

        cachePosition = target.transform.position;
        cachePosition.x = target.GetHorizontalRotation();
        cachePosition.y = target.GetVerticalRotation();

        await Task.Delay(3000);
        FinishAnimation();
    }

    async void FinishAnimation()
    {
        target.canMove = true;
        target.canTurnCamera = true;

        targetPosition = cachePosition;
        targetRotation = cacheRotation;

        await Task.Delay(1000);

        isMovingTarget = false;
    }

    void Update()
    {
        if (isMovingTarget)
        {
            target.SetPosition(Vector3.Lerp(target.transform.position, targetPosition, Time.deltaTime * targetLerpSpeed));
            target.SetHorizontalRotation(Mathf.LerpAngle(target.GetHorizontalRotation(), targetRotation.x, targetLerpSpeed * Time.deltaTime));
            target.SetVerticalRotation(Mathf.LerpAngle(target.GetVerticalRotation(), targetRotation.y, targetLerpSpeed * Time.deltaTime));
        }
    }
}
