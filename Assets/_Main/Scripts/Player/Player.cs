using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class Player : MonoBehaviour
{
    // components
    CharacterController controller;
    [HideInInspector] public Animator animator;

    // referecnes
    Transform cameraRef;

    // state
    bool isReloadingMagazine;
    bool isAnimatorPlaying;

    // permissions
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canTurnCamera = true;
    public bool hasGun;

    // walking
    [Header("Movement")]
    [SerializeField] float speed;

    [Header("Walking Visuals")]
    [SerializeField] AnimationCurve walkingCurve;
    [SerializeField] float stepSpeed = 1;
    float walkingValue;
    bool walkingDirection;

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
    [SerializeField] Transform viewModel;
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
    [SerializeField] int damage;
    [SerializeField] Transform barrel;
    [SerializeField] float range;
    [SerializeField] float deadRange;
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
    bool stopReloading;

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
        viewModel.gameObject.SetActive(false);
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

        if (moveInput.magnitude > 1)
            moveInput.Normalize();

        if (moveInput == Vector2.zero)
        {
            walkingDirection = false;
            walkingValue = .1f;
        }

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
        if (Input.GetMouseButtonDown(0) && hasGun && !isClipping)
        {
            if (isReloadingMagazine)
            {
                stopReloading = true;
            }
            else
            {
                Action_Shoot();
            }
        }

        if (isClipping && isReloadingMagazine)
        {
            stopReloading = true;
        }

        if (Input.GetMouseButtonDown(1) && isReloadingMagazine)
        {
            stopReloading = true;
        }

        // reloading
        if (Input.GetKeyDown(KeyCode.R) && hasGun && !isClipping && !isReloadingMagazine && !isAnimatorPlaying)
        {
            Action_Reload();
        }

        if (Input.GetKeyDown(KeyCode.X) && hasGun && !isClipping && !isReloadingMagazine && !isAnimatorPlaying)
        {
            Action_ManuelRack();
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
        if (canMove)
        {
            controller.Move((transform.right * moveInput.x + transform.forward * moveInput.y + Vector3.down) * speed * Time.deltaTime);
        }

        // rotation
        if (canTurnCamera)
        {
            transform.eulerAngles += Vector3.up * turnInput.x * turnSpeed;
            cameraRef.localRotation = Quaternion.Euler(cameraRotation = Mathf.Clamp(cameraRotation -= turnInput.y * turnSpeed, -80, 80), 0, 0);
        }

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
            focusPoint = cameraRef.position + aimOffset + cameraRef.forward * deadRange;
            Debug.DrawLine(cameraRef.position + aimOffset, cameraRef.position + aimOffset + cameraRef.forward * deadRange, Color.green);
        }

        // visuals
        if (walkingDirection)
        {
            walkingValue += Time.deltaTime * stepSpeed * moveInput.magnitude;
            if (walkingValue > 1)
            {
                walkingDirection = false;
                AudioManager.PlaySound("Step01");
            }
        }
        else
        {
            walkingValue -= Time.deltaTime * stepSpeed * moveInput.magnitude;
            if (walkingValue < 0)
            {
                walkingDirection = true;
                AudioManager.PlaySound("Step02");
            }
        }
    }

    void HandleAnimation()
    {
        //if (isAnimatorPlaying)
        //    return;

        if (!hasGun)
            return;

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
            target = cameraRef.rotation * Quaternion.Euler(clipRotation);
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
        {
            AudioManager.PlaySound("EmptyClip");
            return;
        }

        // set state
        hasRoundInBarrel = false;
        hasEmptyShellInBarrel = true;

        AudioManager.PlaySound("ShotgunShoot");

        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 direction = (barrel.forward + Random.onUnitSphere * spread).normalized;

            if (Physics.Raycast(barrel.position, direction, out RaycastHit hit, range, hitMask))
            {
                //Debug.Log(hit.transform.name);
                Debug.DrawLine(barrel.position, hit.point, Color.green, 1);

                // environment
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

                // anomaly
                if (hit.transform.gameObject.layer == 8)
                {
                    Debug.Log(hit.transform.name);
                    Anomaly anomly = hit.transform.GetComponent<Anomaly>();
                    anomly.health -= damage;
                    anomly.OnTakeDamage();
                }

                if (hit.transform.TryGetComponent(out Shootable shootable))
                {
                    shootable.OnHit(damage);
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

        if (isReloadingMagazine || isAnimatorPlaying)
        {
            return;
        }
        
        isAnimatorPlaying = true;
        animator.SetTrigger("Shoot");
        await Task.Delay(250);
        AudioManager.PlaySound("RackShotgun");
        await Task.Delay(200);
        EjectShell();
        await Task.Delay(500);

        hasRoundInBarrel = true;
        roundsLeft--;
        isAnimatorPlaying = false;

    }

    async void Action_Reload()
    {
        isReloadingMagazine = true;
        isAnimatorPlaying = true;

        animator.SetTrigger("ReloadPose");

        await Task.Delay(250);

        if (isClipping)
        {
            isReloadingMagazine = false;
            isAnimatorPlaying = false;
            animator.SetTrigger("Idle");
            return;
        }

        // reload mag
        while (roundsLeft < magCapacity)
        {
            roundsLeft++;
            animator.SetTrigger("ReloadRound");
            AudioManager.PlaySound("ReloadRound");
            await Task.Delay(600);

            if (isClipping)
            {
                isReloadingMagazine = false;
                isAnimatorPlaying = false;
                animator.SetTrigger("Idle");
                return;
            }

            if (stopReloading)
            {
                stopReloading = false;
                break;
            }

        }

        // final rack
        //await Task.Delay(300);
        animator.SetTrigger("MagRack");
        AudioManager.PlaySound("RackShotgun");
        await Task.Delay(450);
        EjectShell();
        hasRoundInBarrel = true;
        roundsLeft--;

        // idle 
        await Task.Delay(300);
        animator.SetTrigger("Idle");
        await Task.Delay(250);

        isAnimatorPlaying = false;
        isReloadingMagazine = false;
        stopReloading = false;

        // load barrel
        //animator.SetTrigger("Shoot");
        //await Task.Delay(250);
        //AudioManager.PlaySound("RackShotgun");


    }

    async void Action_ManuelRack()
    {
        isAnimatorPlaying = true;

        animator.SetTrigger("Shoot");
        AudioManager.PlaySound("RackShotgun");
        await Task.Delay(450);
        EjectShell();

        if (roundsLeft > 0)
        {
            hasRoundInBarrel = true;
            roundsLeft--;
        }

        await Task.Delay(300);
        animator.SetTrigger("Idle");
        await Task.Delay(250);
        isAnimatorPlaying = false;
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

    public void SetPosition(Vector3 position)
    {
        controller.Move(position - transform.position);
    }

    public void SetHorizontalRotation(float rotation)
    {
        transform.eulerAngles = Vector3.up * rotation;
    }

    public void SetVerticalRotation(float rotation)
    {
        cameraRotation = rotation;
        cameraRef.localEulerAngles = Vector3.right * rotation;
    }

    public float GetHorizontalRotation()
    {
        return transform.eulerAngles.y;
    }

    public float GetVerticalRotation()
    {
        return cameraRef.localEulerAngles.x;
    }

    public void PickupGun()
    {
        hasGun = true;
        viewModel.gameObject.SetActive(true);
        animator.Play("Player_Shotgun_Idle");
    }
}
