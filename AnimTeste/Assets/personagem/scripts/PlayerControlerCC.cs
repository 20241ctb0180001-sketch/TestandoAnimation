using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControlerCC : MonoBehaviour
{
    [Header("Move & Look")]
    private CharacterController CC;
    [SerializeField] private float WalkSpeed = 5.5f;
    [SerializeField] private float RunningSpeed = 9.0f;
    [SerializeField] private float JumpForce = 8.0f;
    [SerializeField] private float Gravity = 20f;
    private float CurMoveSpeed = 0.0f;
    private float verticalVel = 0f;
    private  Camera mainCamera;
    [SerializeField] private float LookSens = 0.2f;
    [SerializeField] private float LookAngLimit = 90f;
    private float lookAngle = 0.0f;
    private InputAction moveInput;
    private InputAction runInput;
    private InputAction jumpInput;
    private InputAction lookinput;

    void Start()
    {
        CC = GetComponent<CharacterController>();
        mainCamera = GetComponentInChildren<Camera>();
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
        if (CC.isGrounded)
        {
            if (jumpInput.WasPressedThisFrame())
                verticalVel = JumpForce;
            else
                verticalVel = -2f;
        }
        else
        {
            verticalVel -= Gravity * Time.deltaTime;
        }
        moveDirection.y = verticalVel;
        CC.Move(moveDirection * Time.deltaTime);
    }

    private void HandleCam(Vector2 lookVec)
    {
        lookAngle += -lookVec.y * LookSens;
        lookAngle = Mathf.Clamp(lookAngle, -LookAngLimit, LookAngLimit);
        mainCamera.transform.localRotation = Quaternion.Euler(lookAngle, 0, 0);
        transform.rotation *= Quaternion.Euler(0, lookVec.x * LookSens, 0);
    }
}
