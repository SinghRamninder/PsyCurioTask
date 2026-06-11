using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerCamera;

    private CharacterController characterController;
    private float xRotation = 0f;
    private Vector3 velocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (characterController != null)
        {
            HandleMovement();
        }
        else
        {
            Debug.LogWarning("PlayerController: CharacterController is missing on this GameObject.");
        }

        if (playerCamera != null)
        {
            HandleMouseLook();
        }
        else
        {
            Debug.LogWarning("PlayerController: Player Camera reference is not assigned in the inspector.");
        }
    }

    private void HandleMovement()
    {
        Vector2 input;
        if (InputManager.Instance != null && InputManager.Instance.controls != null)
        {
            input = InputManager.Instance.controls.Player.Move.ReadValue<Vector2>();
        }
        else
        {
            Debug.LogWarning("PlayerController: InputManager or controls reference is missing.");
            input = Vector2.zero;
        }

        float x = input.x;
        float z = input.y;

        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * moveSpeed * Time.deltaTime);

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        Vector2 lookInput;
        if (InputManager.Instance != null && InputManager.Instance.controls != null)
        {
            lookInput = InputManager.Instance.controls.Player.Look.ReadValue<Vector2>();
        }
        else
        {
            Debug.LogWarning("PlayerController: InputManager or controls reference is missing.");
            lookInput = Vector2.zero;
        }

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
