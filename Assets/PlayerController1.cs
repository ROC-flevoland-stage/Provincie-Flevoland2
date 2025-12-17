using UnityEngine;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;   // New Input System
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float sprintMultiplier = 1.5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 150f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();
    }

    // =======================
    // MOUSE LOOK
    // =======================
    void Look()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mouseDelta = mouse.delta.ReadValue() * mouseSensitivity * Time.deltaTime;
        float mouseX = mouseDelta.x;
        float mouseY = mouseDelta.y;
#else
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
#endif

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    // =======================
    // MOVEMENT + SPRINT
    // =======================
    void Move()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        Vector2 moveInput = Vector2.zero;
        bool jumpPressed = false;
        bool isSprinting = false;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        var kb = Keyboard.current;
        if (kb != null)
        {
            float x = 0f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  x -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;

            float z = 0f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    z += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  z -= 1f;

            moveInput = new Vector2(x, z);
            moveInput = Vector2.ClampMagnitude(moveInput, 1f);

            jumpPressed = kb.spaceKey.wasPressedThisFrame;
            isSprinting = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
        }
#else
        float xAxis = Input.GetAxisRaw("Horizontal");
        float zAxis = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(xAxis, zAxis);
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        jumpPressed = Input.GetButtonDown("Jump");
        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif

        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (isGrounded && jumpPressed)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
