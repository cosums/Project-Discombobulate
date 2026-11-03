using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    public Transform CameraAnchor;
    public float MouseSensitivity = 0.25f;
    public float Speed = 10f;
    public float SprintSpeed = 15f;
    public float CrouchSpeed = 2f;
    public float Gravity = -20f;
    public float JumpHeight = 1.5f;
    public float CrouchScale = 0.5f;

    [SerializeField] AudioClip footsteps;
    [SerializeField] float footstepInterval = 0.45f;
    float footstepTimer;

    private CharacterController m_CharacterController;
    float x, y;
    float camX, camY;
    float verticalVelocity;
    bool crouched;
    bool isCrouching;
    bool isSprinting;

    private Vector3 _initialScale;
    
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _initialScale = transform.localScale;
    }
    
    void Update()
    {
        MyInput();
        Look();

        HandleMovement();
    }

    private void MyInput()
    {
        x = 0f;
        y = 0f;

        if (Keyboard.current.wKey.isPressed) y += 1f;
        if (Keyboard.current.sKey.isPressed) y -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.aKey.isPressed) x -= 1f;

        camY += Mouse.current.delta.x.ReadValue() * MouseSensitivity;
        camX -= Mouse.current.delta.y.ReadValue() * MouseSensitivity;

        isCrouching = Keyboard.current.ctrlKey.isPressed;
        isSprinting = Keyboard.current.shiftKey.isPressed;
    }

    private void Look()
    {
        camX = Mathf.Clamp(camX, -90f, 90f);
        
        CameraAnchor.localRotation = Quaternion.Euler(camX, 0, 0);
        transform.rotation = Quaternion.Euler(0, camY, 0);
    }

    private void HandleMovement()
    {
        Vector3 move = transform.right * x + transform.forward * y;
        move.Normalize();

        bool grounded = m_CharacterController.isGrounded;

        if (grounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        if (grounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        verticalVelocity += Gravity * Time.deltaTime;

        Vector3 velocity = (isSprinting ? SprintSpeed : Speed) * move;
        velocity.y = verticalVelocity;

        m_CharacterController.Move(velocity * Time.deltaTime);

        HandleFootsteps(grounded, move);

        if (isCrouching)
        {
            transform.localScale = new Vector3(_initialScale.x, CrouchScale, _initialScale.y);
        } else
        {
            transform.localScale = _initialScale;
        }
    }

    private void HandleFootsteps(bool grounded, Vector3 move)
    {
        bool isWalking = grounded && move.sqrMagnitude > 0.01f;

        if (!isWalking)
        {
            // Prime the timer so the first step plays right when you start moving
            footstepTimer = footstepInterval;
            return;
        }

        footstepTimer += Time.deltaTime;
        if (footstepTimer >= footstepInterval)
        {
            footstepTimer -= footstepInterval;
            AudioManager.Instance.PlayOneShot(footsteps, 0.8f, null, 0.05f);
        }
    }
}
