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
    private Animator animator;

    private bool isDashing;
    private float dashTimer;
    private float cooldownTimer;
    private Vector3 dashDirection;

    // Direção inicial quando o personagem está parado.
    // Vector2.down significa que ele começa olhando para S.
    private Vector2 lastMoveDirection = Vector2.down;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        animator = GetComponentInChildren<Animator>();

        inputActions = new PlayerInputActions();

        // Define a direção inicial do Idle.
        animator.SetFloat("LastMoveX", lastMoveDirection.x);
        animator.SetFloat("LastMoveY", lastMoveDirection.y);

        // Define também a direção inicial do Dash.
        animator.SetFloat("DashX", lastMoveDirection.x);
        animator.SetFloat("DashY", lastMoveDirection.y);
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

        // Impede que andar na diagonal seja mais rápido.
        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        float inputMagnitude = Mathf.Clamp01(input.magnitude);

        animator.SetFloat("Speed", inputMagnitude);

        if (inputMagnitude > 0.01f)
        {
            // Normaliza para obter uma direção exata.
            Vector2 animationDirection = input.normalized;

            // Direção usada enquanto anda.
            animator.SetFloat("MoveX", animationDirection.x);
            animator.SetFloat("MoveY", animationDirection.y);

            // Guarda a última direção usada.
            lastMoveDirection = animationDirection;

            animator.SetFloat("LastMoveX", lastMoveDirection.x);
            animator.SetFloat("LastMoveY", lastMoveDirection.y);
        }
        else
        {
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", 0f);
        }

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

        // Não executa o dash se estiver parado.
        if (moveDirection == Vector3.zero)
            return;

        StartDash(moveDirection);
    }

    private void StartDash(Vector3 direction)
    {
        dashDirection = direction.normalized;

        isDashing = true;

        // Envia para o Blend Tree a direção do dash.
        // A direção fica travada até o dash terminar.
        animator.SetFloat("DashX", lastMoveDirection.x);
        animator.SetFloat("DashY", lastMoveDirection.y);

        // Interrompe visualmente a caminhada.
        animator.SetFloat("Speed", 0f);

        // Inicia a animação do dash.
        animator.SetBool("IsDashing", true);

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
            EndDash();
        }
    }

    private void EndDash()
    {
        isDashing = false;

        // Retorna para caminhada ou Idle.
        animator.SetBool("IsDashing", false);
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