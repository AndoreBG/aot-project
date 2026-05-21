using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Ground Movement")]
    public float moveSpeed = 8f;
    public float groundAcceleration = 50f;
    public float groundDeceleration = 40f;

    [Header("Air Movement (sem gancho)")]
    public float airAcceleration = 20f;
    public float airDeceleration = 5f;
    public float airMoveSpeed = 6f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.15f;
    public float jumpCutMultiplier = 0.4f;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public Vector2 groundCheckSize = new Vector2(0.4f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float normalGravityScale = 3f;
    public float fallGravityMultiplier = 1.6f;
    public float maxFallSpeed = 25f;
    public float hookedGravityScale = 3f;

    // Público para outros scripts
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isHooked;
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public bool facingRight = true;

    // Direção que o jogador QUER ir (usada pelo gás quando pendurado)
    [HideInInspector] public int intendedDirection; // -1 esquerda, 0 neutro, 1 direita

    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isJumping;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = normalGravityScale;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // ═══ Input ═══
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Guardar direção intencionada (sempre atualiza)
        if (horizontalInput > 0.01f)
            intendedDirection = 1;
        else if (horizontalInput < -0.01f)
            intendedDirection = -1;
        else
            intendedDirection = 0;

        // ═══ Ground check ═══
        isGrounded = Physics2D.OverlapBox(
            groundCheckPoint.position,
            groundCheckSize,
            0f,
            groundLayer
        );

        // ═══ Coyote time ═══
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // ═══ Jump com W ═══
        if (Input.GetKeyDown(KeyCode.W))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // Jump — só quando NÃO está no gancho
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isJumping && !isHooked)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            isJumping = true;
        }

        // Jump cut (soltar W)
        if (Input.GetKeyUp(KeyCode.W) && rb.linearVelocity.y > 0f && isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        if (isGrounded && rb.linearVelocity.y <= 0.1f)
            isJumping = false;

        // ═══ Flip ═══
        // Quando pendurado no ar: flip baseado no input (visual, sem movimento)
        // Quando no chão ou ar livre: flip normal
        if (horizontalInput > 0.01f && !facingRight) Flip();
        else if (horizontalInput < -0.01f && facingRight) Flip();
    }

    private void FixedUpdate()
    {
        // ═══ Quando no gancho: NÃO aplicar movimento terrestre ═══
        // O jogador só balança pela física do pêndulo.
        // A/D NÃO movem — apenas definem intendedDirection para o gás.
        if (isHooked)
        {
            rb.gravityScale = hookedGravityScale;
            // NÃO aplicar nenhuma força horizontal aqui.
            return;
        }

        // ═══ Movimento normal (não pendurado) ═══

        // Gravidade
        if (rb.linearVelocity.y < 0 && !isGrounded)
            rb.gravityScale = normalGravityScale * fallGravityMultiplier;
        else
            rb.gravityScale = normalGravityScale;

        // Movimento horizontal
        float targetSpeed = horizontalInput * (isGrounded ? moveSpeed : airMoveSpeed);
        float accel;

        if (isGrounded)
            accel = Mathf.Abs(horizontalInput) > 0.01f ? groundAcceleration : groundDeceleration;
        else
            accel = Mathf.Abs(horizontalInput) > 0.01f ? airAcceleration : airDeceleration;

        float speedDif = targetSpeed - rb.linearVelocity.x;
        float movement = speedDif * accel * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x + movement, rb.linearVelocity.y);

        // Clamp queda
        if (rb.linearVelocity.y < -maxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
    }
}