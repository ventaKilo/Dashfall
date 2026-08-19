using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 14f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 0.8f;

    private CharacterController controller;
    private Camera mainCamera;
    private PlayerInputActions inputActions;

    private bool isDashing;
    private float dashTimer;
    private float cooldownTimer;
    private Vector3 dashDirection;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isDashing)
        {
            UpdateDash();
            return;
        }

        Vector3 moveDirection = GetMoveDirection();

        CheckDash(moveDirection);

        if (!isDashing)
        {
            Move(moveDirection);
        }
    }

    private Vector3 GetMoveDirection()
    {
        Vector2 input =
            inputActions.Player.Move.ReadValue<Vector2>();

        Vector3 direction =
            new Vector3(input.x, 0f, input.y);

        if (direction.sqrMagnitude < 0.01f)
        {
            return Vector3.zero;
        }

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        direction =
            cameraForward * direction.z +
            cameraRight * direction.x;

        return direction.normalized;
    }

    private void CheckDash(Vector3 moveDirection)
    {
        if (cooldownTimer > 0f)
            return;

        if (!inputActions.Player.Dash.WasPressedThisFrame())
            return;

        // Não executa o dash se o personagem estiver parado
        if (moveDirection == Vector3.zero)
            return;

        StartDash(moveDirection);
    }

    private void StartDash(Vector3 direction)
    {
        dashDirection = direction;

        isDashing = true;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;

        transform.forward = dashDirection;
    }

    private void UpdateDash()
    {
        controller.Move(
            dashDirection * dashSpeed * Time.deltaTime
        );

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }

    private void Move(Vector3 direction)
    {
        if (direction == Vector3.zero)
            return;

        controller.Move(
            direction * moveSpeed * Time.deltaTime
        );

        transform.forward = direction;
    }
}