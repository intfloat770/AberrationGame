using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{
    // components
    CharacterController controller;
    Animator animator;

    // referecnes
    Transform cameraRef;

    // state
    bool isReloadingMagazine;
    bool isAnimatorPlaying;

    // walking
    [Header("Movement")]
    [SerializeField] float speed;

    // turning
    [Header("Turning")]
    [SerializeField] float turnSpeed;
    float cameraRotation = 0;

    // weapon offset
    [Header("Weapon Offset")]
    [SerializeField] Vector3 idleWeaponOffset;
    [SerializeField] Vector3 idleWeaponRotation;
    [SerializeField] Vector3 aimWeaponOffset;
    [SerializeField] Vector3 aimWeaponRotation;
    [SerializeField] float aimLerpSpeed;
    Transform weaponOffset;
    Vector3 targetWeaponOffset;
    Quaternion weaponRotation;

    // weapon sway
    [Header("Weapon Sway")]
    [SerializeField] float weaponSwayIntensity;
    [SerializeField] float swayLerpSpeed;
    Vector2 rotationLastFrame;

    [Header("Model-Clipping")]
    [SerializeField] float clipOriginOffset;
    [SerializeField] float clipDistance;
    [SerializeField] Vector3 clipRotation;
    [SerializeField] LayerMask clipMask;
    bool isClipping;

    [Header("Weapon Aim")]
    [SerializeField] RectTransform crosshairTransform;
    [SerializeField] LayerMask aimMask;
    Vector3 focusPoint = Vector3.one;
    Vector3 crosshairWorldPosition;
    [SerializeField] Vector3 aimOffset;

    [Header("Shooting")]
    [SerializeField] int bulletCount;
    [SerializeField] Transform barrel;
    [SerializeField] float range;
    [SerializeField] float spread;
    [SerializeField] LayerMask hitMask;
    [SerializeField] GameObject[] bulletImpactPrefabs;
    [SerializeField] float kickStrength;
    [SerializeField] float kickFallof;
    float kick;

    [Header("Mag")]
    [SerializeField] int magCapacity;
    int roundsLeft;
    bool hasRoundInBarrel;
    bool hasEmptyShellInBarrel;

    [Header("Shooting Visuals")]
    [SerializeField] GameObject muzzleFlashLight;
    [SerializeField] int muzzleFlashDuration;
    [SerializeField] GameObject ejectedShell;
    [SerializeField] Transform ejectionPoint;
    [SerializeField] Vector3 localEjectionForce;
    [SerializeField] Vector3 localEjectionTorque;

    [Header("Flash light")]
    [SerializeField] GameObject flashLight;
    bool waitForFlashlightInput;

    // input
    Vector2 moveInput;
    Vector2 turnInput;
    bool isAiming, wasAimingLastFrame;

    [SerializeField] int testInt;

    public void Init()
    {
        // get components
        controller = GetComponent<CharacterController>();
        animator = transform.Find("Camera/YBotArms").GetComponent<Animator>();

        // get references
        cameraRef = transform.Find("Camera");
        weaponOffset = transform.Find("Camera/YBotArms/WeaponOffset");

        // init state
        hasRoundInBarrel = false;

        // prepare visuals
        muzzleFlashLight.SetActive(false);
    }

    void Update()
    {
        HandleInput();
        
        HandleMovement();
    }

    private void LateUpdate()
    {
        HandleAnimation();
    }

    void HandleInput()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        turnInput.x = Input.GetAxis("Mouse X");
        turnInput.y = Input.GetAxis("Mouse Y");

        isAiming = Input.GetMouseButton(1);
        targetWeaponOffset = isAiming ? aimWeaponOffset : idleWeaponOffset;
        //if (isAiming && !wasAimingLastFrame) 
        //{
        //    targetWeaponOffset = aimWeaponOffset;
        //}
        //else if (!isAiming && wasAimingLastFrame)
        //{
        //    targetWeaponOffset = idleWeaponOffset;
        //}
        //wasAimingLastFrame = isAiming;

        // overrite
        if (isReloadingMagazine)
        {
            targetWeaponOffset = idleWeaponOffset;
        }

        // shooting
        if (Input.GetMouseButtonDown(0) && !isClipping && hasRoundInBarrel && !isReloadingMagazine)
        {
            Action_Shoot();
        }

        // reloading
        if (Input.GetKeyDown(KeyCode.R) && !isClipping && !isReloadingMagazine)
        {
            Action_Reload();
        }

        // toggle flash light
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashLight.activeInHierarchy)
            {
                flashLight.SetActive(false);
                waitForFlashlightInput = true;
                AudioManager.PlaySound("FlashLightDown");
            }
            else
            {
                AudioManager.PlaySound("FlashLightUp");
            }
            //flashLight.SetActive(!flashLight.activeInHierarchy);
            //await Task.Delay(200);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (!flashLight.activeInHierarchy && !waitForFlashlightInput)
            {
                flashLight.SetActive(true);
                AudioManager.PlaySound("FlashLightDown");
            }
            else
            {
                AudioManager.PlaySound("FlashLightUp");
            }

            waitForFlashlightInput = false;
        }
    }

    void HandleMovement()
    {
        // movement
        controller.Move((transform.right * moveInput.x + transform.forward * moveInput.y + Vector3.down) * speed * Time.deltaTime);

        // rotation
        transform.eulerAngles += Vector3.up * turnInput.x * turnSpeed;
        cameraRef.localRotation = Quaternion.Euler(cameraRotation = Mathf.Clamp(cameraRotation -= turnInput.y * turnSpeed, -80, 80), 0, 0);

        // clipping
        //Vector3 targetWeapon = cameraRef.TransformDirection(isAiming ? aimWeaponOffset : idleWeaponOffset) - Vector3.up * cameraRef.position.y;
        Vector3 direction = (focusPoint - weaponOffset.position).normalized; //Quaternion.LookRotation((focusPoint - barrel.position).normalized) * cameraRef.forward;
        isClipping = Physics.Raycast(weaponOffset.position + direction * clipOriginOffset, direction, out RaycastHit hit, clipDistance, clipMask);
        Debug.DrawLine(focusPoint, weaponOffset.position, Color.cyan);
        Debug.DrawLine(weaponOffset.position + direction * clipOriginOffset, weaponOffset.position + direction * clipOriginOffset + direction * clipDistance, Color.yellow);

        // focus point
        if (Physics.Raycast(cameraRef.position + aimOffset, cameraRef.forward, out RaycastHit result, range, aimMask))
        {
            focusPoint = result.point;
            Debug.DrawLine(cameraRef.position + aimOffset, cameraRef.position + aimOffset + cameraRef.forward * range, Color.red);
        }
        else
        {
            focusPoint = cameraRef.position + aimOffset + cameraRef.forward * range;
            Debug.DrawLine(cameraRef.position + aimOffset, cameraRef.position + aimOffset + cameraRef.forward * range, Color.green);
        }
    }

    void HandleAnimation()
    {
        //if (isAnimatorPlaying)
        //    return;

        // offset
        weaponOffset.localPosition = Vector3.Lerp(weaponOffset.localPosition, targetWeaponOffset, aimLerpSpeed * Time.deltaTime);

        Quaternion target = Quaternion.identity;

        // rotation by weapoin aim
        if (!isClipping)
        {
            target = Quaternion.LookRotation((focusPoint - barrel.position).normalized);
            Debug.DrawLine(barrel.position, barrel.position + barrel.forward * range, Color.blue);
            Debug.DrawLine(barrel.position, barrel.position + (focusPoint - barrel.position).normalized * range, Color.magenta);
        }

        // rotation by clipping
        else
        {
            target = transform.rotation * Quaternion.Euler(clipRotation);
        }

        // sway
        float deltaX = rotationLastFrame.x - cameraRef.eulerAngles.x;
        float deltaY = rotationLastFrame.y - transform.eulerAngles.y;
        target *= Quaternion.AngleAxis(deltaX * weaponSwayIntensity, Vector3.right) * Quaternion.AngleAxis(deltaY * weaponSwayIntensity, Vector3.up);

        // rotation by offset
        //weaponRotation = Quaternion.Lerp(weaponRotation, Quaternion.Euler(isAiming ? aimWeaponRotation : idleWeaponRotation), aimLerpSpeed * Time.deltaTime);
        //target *= weaponRotation;

        // rotation by clip prevention
        //if (isClipping)
        //    target *= Quaternion.Euler(clipRotation);


        // add kick
        target *= Quaternion.Euler(Vector3.right * kick);
        kick -= kick * kickFallof * Time.deltaTime;

        weaponOffset.rotation = Quaternion.Lerp(weaponOffset.rotation, target, swayLerpSpeed * Time.deltaTime);
        rotationLastFrame.x = cameraRef.eulerAngles.x;
        rotationLastFrame.y = transform.eulerAngles.y;
    }

    async void Action_Shoot()
    {
        if (!hasRoundInBarrel)
            return;

        // set state
        hasRoundInBarrel = false;
        hasEmptyShellInBarrel = true;

        AudioManager.PlaySound("ShotgunShoot");

        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 direction = (barrel.forward + Random.onUnitSphere * spread).normalized;

            if (Physics.Raycast(barrel.position, direction, out RaycastHit hit, range, hitMask))
            {
                Debug.DrawLine(barrel.position, hit.point, Color.green, 1);
                if (hit.transform.gameObject.layer == 10)
                {
                    if (int.TryParse(hit.transform.name.Substring(hit.transform.name.Length - 1), out int materialIndex))
                    {
                        // spawn impact
                        GameObject impact = Instantiate(bulletImpactPrefabs[materialIndex - 1], hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
                        Destroy(impact, 100);
                    }
                    else
                        Debug.Log($"No material index on gameobject: {hit.transform.name}");
                }
            }
            else
                Debug.DrawLine(barrel.position, barrel.position + direction * range, Color.red, 1);
        }

        // add kick
        kick += kickStrength;

        // show muzzle flash
        muzzleFlashLight.SetActive(true);
        await Task.Delay(muzzleFlashDuration);
        muzzleFlashLight.SetActive(false);

        // reload
        if (roundsLeft == 0)
            return;

        while (kick < -1)
        {
            await Task.Yield();
        }

        await Task.Delay(250);
        animator.SetTrigger("Shoot");
        isAnimatorPlaying = true;
        await Task.Delay(250);
        AudioManager.PlaySound("RackShotgun");
        await Task.Delay(150);
        EjectShell();
        await Task.Delay(500);

        hasRoundInBarrel = true;
        roundsLeft--;
        isAnimatorPlaying = false;

    }

    async void Action_Reload()
    {
        isReloadingMagazine = true;

        animator.SetTrigger("ReloadPose");

        await Task.Delay(250);

        // reload mag
        while (roundsLeft < magCapacity)
        {
            roundsLeft++;
            animator.SetTrigger("ReloadRound");
            AudioManager.PlaySound("ReloadRound");
            await Task.Delay(600);
        }

        // final rack
        await Task.Delay(300);
        animator.SetTrigger("MagRack");
        AudioManager.PlaySound("RackShotgun");
        await Task.Delay(450);
        EjectShell();
        hasRoundInBarrel = true;
        roundsLeft--;

        // idle 
        await Task.Delay(1000);
        animator.SetTrigger("Idle");
        await Task.Delay(250);
        
        isReloadingMagazine = false;

        // load barrel
        //animator.SetTrigger("Shoot");
        //await Task.Delay(250);
        //AudioManager.PlaySound("RackShotgun");


    }

    void EjectShell()
    {
        if (hasRoundInBarrel || hasEmptyShellInBarrel)
        {
            hasRoundInBarrel = false;
            hasEmptyShellInBarrel = false;
            GameObject obj = Instantiate(ejectedShell);
            obj.transform.position = ejectionPoint.position;
            obj.transform.rotation = ejectionPoint.rotation;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.AddForce(ejectionPoint.TransformDirection(localEjectionForce));
            rb.AddTorque(ejectionPoint.TransformDirection(localEjectionTorque));
            Destroy(obj, 100);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(focusPoint, .1f);
    }
}
