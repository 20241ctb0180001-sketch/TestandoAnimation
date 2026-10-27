using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControlerCC : MonoBehaviour
{
    private CharacterController CC;
    [SerializeField] private Animator anim;

    [Header("Movement Settings")]
    [SerializeField] private float WalkSpeed = 5.5f;
    [SerializeField] private float RunningSpeed = 9.0f;
    [SerializeField] private float JumpForce = 8.0f;
    [SerializeField] private float Gravity = 20f;
    [SerializeField] private float rotationSmoothness = 10f;
    private float CurMoveSpeed = 0.0f;
    private float verticalVel = 0f;

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float LookSens = 0.2f;
    [SerializeField] private float LookAngLimit = 90f;
    private float lookAngle = 0.0f;

    private InputAction moveInput;
    private InputAction runInput;
    private InputAction jumpInput;
    private InputAction lookinput;

    private float speed;
    private bool isJumping;

    void Start()
    {
        CC = GetComponent<CharacterController>();
        mainCamera = GetComponentInChildren<Camera>();

        anim = GetComponentInChildren<Animator>();

        moveInput = InputSystem.actions.FindAction("Move");
        runInput = InputSystem.actions.FindAction("Sprint");
        jumpInput = InputSystem.actions.FindAction("Jump");
        lookinput = InputSystem.actions.FindAction("Look");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CurMoveSpeed = WalkSpeed;
    }

    void Update()
    {
        Vector2 moveVec = moveInput.ReadValue<Vector2>();
        CurMoveSpeed = runInput.IsPressed() ? RunningSpeed : WalkSpeed;
        HandleMovement(moveVec);

        Vector2 lookVec = lookinput.ReadValue<Vector2>();
        HandleCam(lookVec);
    }

    private void HandleMovement(Vector2 moveVec)
    {
        Vector3 moveDirection = (transform.forward * moveVec.y + transform.right * moveVec.x) * CurMoveSpeed;

        //if (moveVec.sqrMagnitude > 0.01f) anim.transform.localRotation = Quaternion.LookRotation(new Vector3(moveVec.x, 0f, moveVec.y));
        if (moveVec.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveVec.x, 0f, moveVec.y));
            float rotationT = 1f - Mathf.Exp(-rotationSmoothness * Time.deltaTime);
            anim.transform.localRotation = Quaternion.Slerp(anim.transform.localRotation, targetRotation, rotationT);
        }

        if (CC.isGrounded)
        {
            speed = runInput.IsPressed() ? moveVec.magnitude * RunningSpeed : moveVec.magnitude * WalkSpeed;
            
            if (jumpInput.WasPressedThisFrame())
            {
                verticalVel = JumpForce;
                isJumping = true;
            }
            else
            {
                isJumping = false;
                verticalVel = -2f;
            }
        }
        else
        {
            isJumping = false;
            speed = Mathf.MoveTowards(speed, 0f, 10f * Time.deltaTime);
            verticalVel -= Gravity * Time.deltaTime;
        }
        moveDirection.y = verticalVel;
        CC.Move(moveDirection * Time.deltaTime);
        anim.SetFloat("Speed", speed);
        anim.SetBool("jumpact", isJumping);
    }

    private void HandleCam(Vector2 lookVec)
    {
        lookAngle += -lookVec.y * LookSens;
        lookAngle = Mathf.Clamp(lookAngle, -LookAngLimit, LookAngLimit);
        mainCamera.transform.localRotation = Quaternion.Euler(lookAngle, 0, 0);
        transform.rotation *= Quaternion.Euler(0, lookVec.x * LookSens, 0);
    }
}
