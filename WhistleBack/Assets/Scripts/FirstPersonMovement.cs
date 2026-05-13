using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Mouse Look Settings")]
    public Transform cameraTransform;
    public float mouseSensitivity = 100f;
    public float maxLookAngle = 80f;

    [Header("Head Bob Settings")]
    public float bobFrequency = 1.5f;
    public float bobHorizontalAmplitude = 0.05f;
    public float bobVerticalAmplitude = 0.05f;
    public float bobSmoothSpeed = 6f;

    [Header("Animation Settings")]
    public Animator anim;

    private Rigidbody rb;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f;

    private Vector3 defaultCamLocalPos;
    private float bobTimer = 0f;

    [Header("Input Mapping")]
    public InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction lookAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        InputActionMap playerMap = inputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform != null)
        {
            defaultCamLocalPos = cameraTransform.localPosition;
        }
    }

    void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        HandleMouseLook();
        HandleHeadBob();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        Vector3 targetVelocity = move * moveSpeed;

        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0f, velocity.z);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        if (move.magnitude > 0.1f)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    void HandleHeadBob()
    {
        if (cameraTransform == null) return;

        float speed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;

        if (speed > 0.1f)
        {
            bobTimer += Time.deltaTime * bobFrequency * speed;

            float bobX = Mathf.Cos(bobTimer) * bobHorizontalAmplitude;
            float bobY = Mathf.Sin(bobTimer * 2f) * bobVerticalAmplitude;

            Vector3 targetPosition = defaultCamLocalPos + new Vector3(bobX, bobY, 0f);

            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime * bobSmoothSpeed);
        }
        else
        {
            bobTimer = 0f;

            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, defaultCamLocalPos, Time.deltaTime * bobSmoothSpeed);
        }
    }
}
