using UnityEngine;
using System;

public class GrapplingHook : MonoBehaviour
{
    public enum HookState
    {
        Idle,
        Firing,
        Attached,
        Retracting
    }

    [Header("Hook Settings")]
    public float maxRange = 15f;
    public float hookFireSpeed = 40f;
    public float hookRetractSpeed = 30f;
    public float reelInSpeed = 8f;
    public float reelMinDistance = 1.5f;
    public LayerMask grappleableLayers;

    [Header("Swing Physics")]
    public float maxSpeed = 30f;

    [Header("Anchor Points")]
    public Transform leftAnchor;
    public Transform rightAnchor;

    [Header("Audio")]
    public AudioClip fireSound;
    public AudioClip attachSound;
    public AudioClip retractSound;

    // State
    [HideInInspector] public HookState currentState = HookState.Idle;
    [HideInInspector] public Vector2 hookTipPosition;
    [HideInInspector] public Vector2 attachPoint;
    [HideInInspector] public float currentRopeLength;
    [HideInInspector] public bool isReelingIn;

    private PlayerMovement2D player;
    private Rigidbody2D rb;
    private Vector2 fireDirection;
    private float distanceTraveled;
    private AudioSource audioSource;

    // Eventos
    public event Action OnHookFired;
    public event Action OnHookAttached;
    public event Action OnHookRetractStart;
    public event Action OnHookRetracted;

    private void Awake()
    {
        player = GetComponent<PlayerMovement2D>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case HookState.Firing:
                FixedUpdateFiring();
                break;
            case HookState.Attached:
                FixedUpdateAttached();
                break;
            case HookState.Retracting:
                FixedUpdateRetracting();
                break;
        }
    }

    // ═══════════════════════════════════════════
    // INPUT
    // ═══════════════════════════════════════════
    private void HandleInput()
    {
        // Botão direito: disparar / soltar gancho
        if (Input.GetMouseButtonDown(1))
        {
            if (currentState == HookState.Idle)
                FireHook();
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (currentState == HookState.Attached || currentState == HookState.Firing)
                StartRetract();
        }

        // Shift: recolher corda
        isReelingIn = (currentState == HookState.Attached) &&
                      Input.GetKey(KeyCode.LeftShift);
    }

    // ═══════════════════════════════════════════
    // FIRE
    // ═══════════════════════════════════════════
    private void FireHook()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 origin = GetAnchorCenter();
        fireDirection = (mouseWorld - origin).normalized;

        hookTipPosition = origin;
        distanceTraveled = 0f;
        currentState = HookState.Firing;

        PlaySound(fireSound);
        OnHookFired?.Invoke();
    }

    private void FixedUpdateFiring()
    {
        float step = hookFireSpeed * Time.fixedDeltaTime;

        RaycastHit2D hit = Physics2D.Raycast(
            hookTipPosition,
            fireDirection,
            step,
            grappleableLayers
        );

        if (hit.collider != null)
        {
            attachPoint = hit.point;
            hookTipPosition = attachPoint;
            currentRopeLength = Vector2.Distance(rb.position, attachPoint);
            currentState = HookState.Attached;
            player.isHooked = true;

            // Preservar momentum ao prender
            PlaySound(attachSound);
            OnHookAttached?.Invoke();
            return;
        }

        hookTipPosition += fireDirection * step;
        distanceTraveled += step;

        if (distanceTraveled >= maxRange)
            StartRetract();
    }

    // ═══════════════════════════════════════════
    // ATTACHED — PÊNDULO PURO
    // ═══════════════════════════════════════════
    private void FixedUpdateAttached()
    {
        // ─── Recolher corda (Shift) ───
        if (isReelingIn)
        {
            currentRopeLength -= reelInSpeed * Time.fixedDeltaTime;
            currentRopeLength = Mathf.Max(currentRopeLength, reelMinDistance);
        }

        // ─── NÃO aplicar força tangencial por A/D ───
        // O jogador só balança pela gravidade e pelo gás.
        // A/D apenas definem a "direção intencionada" no PlayerMovement2D,
        // que o GasBooster lê quando Space é pressionado.

        // ─── Constraint de pêndulo ───
        EnforceRopeConstraint();

        // ─── Clamp velocidade ───
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    /// <summary>
    /// Constraint rígido de pêndulo.
    /// Impede que o jogador fique mais longe que currentRopeLength do attachPoint.
    /// Remove APENAS o componente radial da velocidade que esticaria a corda.
    /// Preserva 100% do componente tangencial (momentum do balanço).
    /// </summary>
    private void EnforceRopeConstraint()
    {
        Vector2 displacement = rb.position - attachPoint;
        float currentDist = displacement.magnitude;

        // Dentro do raio = corda frouxa, gravidade atua livremente
        if (currentDist <= currentRopeLength)
            return;

        // ─── Corda tensa ───

        // Direção do pivot para o jogador (radial, para fora)
        Vector2 ropeDir = displacement.normalized;

        // 1) Corrigir posição: colocar na borda exata do raio
        rb.position = attachPoint + ropeDir * currentRopeLength;

        // 2) Corrigir velocidade: remover componente que se afasta
        Vector2 velocity = rb.linearVelocity;
        float radialSpeed = Vector2.Dot(velocity, ropeDir);

        // radialSpeed > 0 significa "se afastando do pivot"
        if (radialSpeed > 0f)
        {
            velocity -= ropeDir * radialSpeed;
            rb.linearVelocity = velocity;
        }
    }

    // ═══════════════════════════════════════════
    // RETRACT
    // ═══════════════════════════════════════════
    public void StartRetract()
    {
        if (currentState == HookState.Idle || currentState == HookState.Retracting)
            return;

        currentState = HookState.Retracting;
        player.isHooked = false;
        isReelingIn = false;

        // Preservar velocidade ao soltar (momentum de lançamento)
        PlaySound(retractSound);
        OnHookRetractStart?.Invoke();
    }

    private void FixedUpdateRetracting()
    {
        Vector2 anchorCenter = GetAnchorCenter();
        float step = hookRetractSpeed * Time.fixedDeltaTime;

        hookTipPosition = Vector2.MoveTowards(hookTipPosition, anchorCenter, step);

        if (Vector2.Distance(hookTipPosition, anchorCenter) < 0.2f)
        {
            currentState = HookState.Idle;
            hookTipPosition = anchorCenter;
            OnHookRetracted?.Invoke();
        }
    }

    // ═══════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════
    public Vector2 GetAnchorCenter()
    {
        if (leftAnchor != null && rightAnchor != null)
            return ((Vector2)leftAnchor.position + (Vector2)rightAnchor.position) * 0.5f;
        return rb.position;
    }

    public Vector2 GetLeftAnchor()
    {
        return leftAnchor != null ? (Vector2)leftAnchor.position : rb.position;
    }

    public Vector2 GetRightAnchor()
    {
        return rightAnchor != null ? (Vector2)rightAnchor.position : rb.position;
    }

    public bool IsActive()
    {
        return currentState != HookState.Idle;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector2 center = Application.isPlaying ? GetAnchorCenter() : (Vector2)transform.position;
        int seg = 40;
        Vector2 prev = center + new Vector2(maxRange, 0);
        for (int i = 1; i <= seg; i++)
        {
            float a = i * 2f * Mathf.PI / seg;
            Vector2 p = center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * maxRange;
            Gizmos.DrawLine(prev, p);
            prev = p;
        }

        if (!Application.isPlaying) return;

        if (currentState != HookState.Idle)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hookTipPosition, 0.15f);
        }

        if (currentState == HookState.Attached)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            prev = attachPoint + new Vector2(currentRopeLength, 0);
            for (int i = 1; i <= seg; i++)
            {
                float a = i * 2f * Mathf.PI / seg;
                Vector2 p = attachPoint + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * currentRopeLength;
                Gizmos.DrawLine(prev, p);
                prev = p;
            }
            Gizmos.color = Color.green;
            Gizmos.DrawLine(attachPoint, transform.position);
        }
    }
}