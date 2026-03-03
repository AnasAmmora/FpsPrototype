using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnim : MonoBehaviour
{
    public FirstPersonController player;

    [Header("Weapon Sway")]
    [SerializeField] private float positionalSway = 0.1f;
    [SerializeField] private float rotationalSway = 0.1f;
    [SerializeField] private float swaySmoothness = 1f;
    [SerializeField] private float swayRangeX = 10.0f; 
    [SerializeField] private float swayRangeY = 10.0f; 

    private Vector3 initialPosition = Vector3.zero;
    private Quaternion initialRotation = Quaternion.identity;

    [Header("Weapon Bobbing")]
    [SerializeField] private float bobbingSpeed = 5f;
    [SerializeField] private float bobbingAmount = 5f;

    private float bobTimer = 0f;

    [Header("Recoil Animation")]
    [SerializeField] private float positionalRecoilAmount = 0.2f;
    [SerializeField] private float rotationalRecoilAmount = 0.2f;
    [SerializeField] private float recoilSmoothness = 5f;

    [HideInInspector] public bool isRecoiling = false;
    private Vector3 currentPositionalRecoil = Vector3.zero;
    private Quaternion currentRotationalRecoil = Quaternion.identity;

    [Header("Aim Animation")]
    [SerializeField] private CinemachineVirtualCamera playerFollowCamera;
    [SerializeField] private CinemachineVirtualCamera playerAimCamera;
    [SerializeField] private int followPriority = 11; 
    [SerializeField] private int aimPriority = 10;    
    [HideInInspector] public bool isAiming;
  

    [Header("Weapon Aim Settings")]
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Vector3 aimWeaponPosition;
    [SerializeField] private Vector3 defaultWeaponPosition;
    [SerializeField] private float transitionSpeed = 10f;


   float originalSpeed;
   float originalRotationSpeed;

    private void Start()
    {
        originalSpeed = player.MoveSpeed;
        originalRotationSpeed = player.RotationSpeed;

        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        player.crosshair.gameObject.SetActive(true);
    }

    private void Update()
    {
        ApplySway();
        ApplyBobbing();
        ApplyRecoil();
        ApplyAim();
    }

    private void ApplySway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        mouseX = Mathf.Clamp(mouseX, -swayRangeX, swayRangeX);
        mouseY = Mathf.Clamp(mouseY, -swayRangeY, swayRangeY);

        Vector3 positionOffset = new Vector3(mouseX, mouseY, 0) * positionalSway;
        Quaternion rotationOffset = Quaternion.Euler(new Vector3(-mouseY, -mouseX, mouseX) * rotationalSway);

        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition - positionOffset, Time.deltaTime * swaySmoothness);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotation * rotationOffset, Time.deltaTime * swaySmoothness);
    }

    private void ApplyBobbing()
    {
        float moveSpeed = Input.GetAxis("Horizontal") + Input.GetAxis("Vertical");
        float bobOffsetY = 0f;
        float bobOffsetX = 0f;

        if (moveSpeed > 0.1f && player.Controller.isGrounded)
        {
            bobTimer += Time.deltaTime * bobbingSpeed * player.speed;
            bobOffsetY = Mathf.Sin(bobTimer) * bobbingAmount;
            //bobOffsetX = Mathf.Cos(bobTimer) * (bobbingAmount / 2);
        }
        else
        {
            bobTimer = 0f;
            bobOffsetY = Mathf.Lerp(bobTimer, 0, Time.deltaTime * swaySmoothness);
            bobOffsetX = Mathf.Lerp(bobTimer, 0, Time.deltaTime * swaySmoothness);
        }

        transform.localPosition += new Vector3(bobOffsetX, bobOffsetY, 0);
    }

    private void ApplyRecoil()
    {
        Vector3 targetPositionalRecoil = Vector3.zero;
        Quaternion targetRotationalReciil = Quaternion.identity;

        if (isRecoiling)
        {
            targetPositionalRecoil = new Vector3(0, 0, -positionalRecoilAmount);
            targetRotationalReciil = Quaternion.Euler(rotationalRecoilAmount, 0, 0);

            if(Vector3.Distance(currentPositionalRecoil, targetPositionalRecoil) < 0.5f)
            {
                isRecoiling = false;
            }
        }

        currentPositionalRecoil = Vector3.Lerp(currentPositionalRecoil, targetPositionalRecoil, Time.deltaTime * recoilSmoothness);
        currentRotationalRecoil = Quaternion.Lerp(currentRotationalRecoil, targetRotationalReciil, Time.deltaTime * recoilSmoothness);

        transform.localPosition += currentPositionalRecoil;
        transform.localRotation *= currentRotationalRecoil;
    }

    private void ApplyAim()
    {
        isAiming = Input.GetMouseButton(1);


        if (isAiming)
        {
            playerFollowCamera.Priority = followPriority - 1;
            playerAimCamera.Priority = aimPriority;
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, aimWeaponPosition, transitionSpeed * Time.deltaTime);
            player.crosshair.gameObject.SetActive(false);

            //player.MoveSpeed = Mathf.Lerp(player.speed, originalSpeed, transitionSpeed * Time.deltaTime);
            //player.RotationSpeed = Mathf.Lerp(player.RotationSpeed, originalRotationSpeed, transitionSpeed * Time.deltaTime);
        }
        else
        {
            playerFollowCamera.Priority = followPriority;
            playerAimCamera.Priority = aimPriority - 1;
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, defaultWeaponPosition, transitionSpeed * Time.deltaTime);
            player.crosshair.gameObject.SetActive(true);

            //player.MoveSpeed = Mathf.Lerp(player.speed, originalSpeed * 0.7f, transitionSpeed * Time.deltaTime);
            //player.RotationSpeed = Mathf.Lerp(player.RotationSpeed, originalRotationSpeed * 0.7f, transitionSpeed * Time.deltaTime);
        }
    }
}
