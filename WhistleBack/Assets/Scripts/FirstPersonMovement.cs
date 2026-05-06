using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Rigidbody rb;
    private float xRotation = 0f;
    private Vector3 defaultCamLocalPos;
    private float bobTimer = 0f;

    [Header("Animation Settings")]
    public Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform != null)
        {
            defaultCamLocalPos = cameraTransform.localPosition;
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleHeadBob();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 targetVelocity = move * moveSpeed;

        Vector3 velocityChange = targetVelocity - rb.linearVelocity;
        velocityChange.y = 0f; // Keep Y velocity (gravity/jump) unchanged

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        if(move.magnitude > 0.3f) 
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

        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        if (speed > 0.1f)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float bobX = Mathf.Cos(bobTimer) * bobHorizontalAmplitude;
            float bobY = Mathf.Sin(bobTimer * 2) * bobVerticalAmplitude;

            Vector3 target = defaultCamLocalPos + new Vector3(bobX, bobY, 0);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, target, Time.deltaTime * bobSmoothSpeed);
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, defaultCamLocalPos, Time.deltaTime * bobSmoothSpeed);
        }
    }
}
