using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerRB : MonoBehaviour
{
    [SerializeField] private float WalkSpeed = 5.5f;
    [SerializeField] private float RunningSpeed = 9.0f;
    [SerializeField] private float JumpForce = 8.0f;
    private Rigidbody rb;
    private Vector2 moveVector = Vector2.zero;
    private float CurrMovespeed = 0.0f;
    private bool JumpRequest = false;
    [SerializeField] private float GroundCheckExtra = 0.1f;
    private CapsuleCollider capsule;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;
    private InputAction moveInput;
    private InputAction runInput;
    private InputAction jumpInput;
    private InputAction lookInput;

    private Camera mainCamera;
    [SerializeField] private float LookSens = 0.2f;
    [SerializeField] private float LookAngLimit = 90f;
    private float lookAngle = 0.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        capsule = GetComponent<CapsuleCollider>();
        mainCamera = GetComponentInChildren<Camera>();
        moveInput = InputSystem.actions.FindAction("Move");
        runInput = InputSystem.actions.FindAction("Sprint");
        jumpInput = InputSystem.actions.FindAction("Jump");
        lookInput = InputSystem.actions.FindAction("Look");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CurrMovespeed = WalkSpeed;
    }

    void Update()
    {
        moveVector = moveInput.ReadValue<Vector2>();
        
        CurrMovespeed = runInput.IsPressed() ? RunningSpeed : WalkSpeed;
        if (jumpInput.WasPressedThisFrame())
        {
            JumpRequest = true;
        }

        Vector2 lookVec = lookInput.ReadValue<Vector2>();
        HandleCam(lookVec);
    }

    private void FixedUpdate()
    {
        Vector3 move = (transform.forward * moveVector.y + transform.right * moveVector.x) * CurrMovespeed;
        float verticalVel = rb.linearVelocity.y;

        float distance = capsule.height / 2f + GroundCheckExtra;
        Vector3 origin = transform.TransformPoint(capsule.center);
        isGrounded = Physics.Raycast(origin, Vector3.down, distance, groundMask);

        if(JumpRequest && isGrounded)
        {
            verticalVel = JumpForce;
        }
        JumpRequest =  false;

        rb.linearVelocity = new Vector3(move.x, verticalVel, move.z);
    }

    private void HandleCam(Vector2 lookVec)
    {
        lookAngle += -lookVec.y * LookSens;
        lookAngle = Mathf.Clamp(lookAngle, -LookAngLimit, LookAngLimit);
        mainCamera.transform.localRotation = Quaternion.Euler(lookAngle, 0, 0);
        transform.rotation *= Quaternion.Euler(0, lookVec.x * LookSens, 0);
    }
}
