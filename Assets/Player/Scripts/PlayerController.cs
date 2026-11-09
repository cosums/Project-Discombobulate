using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private enum MovementMode
    {
        Walking,
        Crouching,
        Sprinting
    }

    public static bool CanMove = true;
    public Transform CameraAnchor;
    public float MouseSensitivity = 0.25f;
    public float WalkSpeed = 4.5f;
    public float SprintSpeed = 7f;
    public float CrouchSpeed = 2f;
    public float Gravity = -20f;
    public float JumpHeight = 1.5f;
    public float CrouchScale = 0.5f;
    public float CrouchTime = 0.3f;

    private float _crouchVelocity;

    [SerializeField] AudioClip footsteps;
    [SerializeField] float footstepInterval = 0.45f;
    float footstepTimer;

    private CharacterController m_CharacterController;
    float x, y;
    float camX, camY;
    float verticalVelocity;
    
    bool isCrouching;
    bool isSprinting;

    private Vector3 _initialScale;
    private MovementMode _currentMovementMode = MovementMode.Walking;
    
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        DisableCursor();
        _initialScale = transform.localScale;
    }
    
    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;
        
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

        // reorder if we want, but i imagine crouching is prioritized
        isCrouching = Keyboard.current.ctrlKey.isPressed;
        isSprinting = Keyboard.current.shiftKey.isPressed;

        if (isCrouching) _currentMovementMode = MovementMode.Crouching;
        else if (isSprinting) _currentMovementMode = MovementMode.Sprinting;
        else _currentMovementMode = MovementMode.Walking;
    }

    private void Look()
    {
        if (!CanMove) return;

        camX = Mathf.Clamp(camX, -90f, 90f);
        
        CameraAnchor.localRotation = Quaternion.Euler(camX, 0, 0);
        transform.rotation = Quaternion.Euler(0, camY, 0);
    }

    private void HandleMovement()
    {
        if (!CanMove) return;

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

        float speed = GetCurrentSpeed();

        Vector3 velocity = speed * move;
        velocity.y = verticalVelocity;

        m_CharacterController.Move(velocity * Time.deltaTime);

        HandleFootsteps(grounded, move);

        float targetY = _currentMovementMode == MovementMode.Crouching ? CrouchScale : _initialScale.y;
        float currentYScale = Mathf.SmoothDamp(transform.localScale.y, targetY, ref _crouchVelocity, CrouchTime);
        transform.localScale = new Vector3(_initialScale.x, currentYScale, _initialScale.z);
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

    private float GetCurrentSpeed()
    {
        switch (_currentMovementMode)
        {
            case MovementMode.Walking: return WalkSpeed;
            case MovementMode.Crouching: return CrouchSpeed;
            case MovementMode.Sprinting: return SprintSpeed;
            default: return WalkSpeed;
        }
    }

    private void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
