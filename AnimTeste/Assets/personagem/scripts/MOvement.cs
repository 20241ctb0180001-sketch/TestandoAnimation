using UnityEngine;
using UnityEngine.InputSystem;

public class MOvement : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction MoveAct;
    private InputAction RunAct;
    private InputAction JumpAct;

    public float walkSpeed;
    public float runSpeed;
    public float turnSmoothTime;
    public float movementSmoothing;

    private Rigidbody rb;
    Animator animator;
    private Transform cameraTransform;
    private float turnSmoothVelocity;
    private Vector3 moveDirection;
    private Vector3 inputDirection;
    private Vector3 currentVelocity;
    private bool isRunning;
    private float speed;

    void Start()
    {
        MoveAct = InputSystem.actions.FindAction("Move");
        RunAct = InputSystem.actions.FindAction("Sprint");
        JumpAct = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        if (JumpAct.WasPressedThisFrame())
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    void Move()
    {
        // Get input from the new Input System
        Vector2 moveInput = MoveAct.ReadValue<Vector2>();
        isRunning = RunAct.IsPressed();

        if (moveInput.magnitude >= 0.1f)
        {
            inputDirection = new Vector3(moveInput.x, 0f, moveInput.y);

            // Calculate the movement direction relative to the player's current rotation
            Vector3 targetDirection = cameraTransform.forward * inputDirection.z + cameraTransform.right * inputDirection.x;
            targetDirection.y = 0f; // Ensure the direction is parallel to the ground

            // Smooth the rotation of the character
            float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Smooth the movement direction using Vector3.Lerp
            moveDirection = Vector3.Lerp(moveDirection, transform.forward, movementSmoothing);

            // Determine speed based on whether the player is running or walking
            speed = isRunning ? runSpeed : walkSpeed;

            // Apply force to move the player smoothly
            Vector3 targetVelocity = moveDirection.normalized * speed;
            rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, movementSmoothing);
        }
        else
        {
            // Gradually slow down the player when no input is detected
            rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, Vector3.zero, ref currentVelocity, movementSmoothing);
            speed = 0f;
        }

        animator.SetFloat("Speed", speed);
    }
}