using System;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerDamageKind
{
    HitPoints,
    LegWound
}

[Serializable]
public class HealthChangedEvent : UnityEvent<float, float> { }

[Serializable]
public class DamageTakenEvent : UnityEvent<float, Vector2> { }

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [Min(1f)] public float maxHealth = 100f;
    [Min(0f)] public float currentHealth = 50f;
    public bool clampStartHealthToMax = true;

    [Header("Invulnerability")]
    public float invulnerabilityDuration = 0.6f;

    [Header("Knockback")]
    public float knockbackImpulse = 12f;
    public float upwardKnockbackBonus = 0.25f;
    public float controlLockDuration = 0.18f;

    [Header("Leg Wound")]
    [Range(0.1f, 1f)] public float woundedMoveSpeedMultiplier = 0.6f;
    [Range(0.1f, 1f)] public float woundedJumpForceMultiplier = 0.55f;

    [Header("Inspector Events")]
    public HealthChangedEvent onHealthChanged;
    public DamageTakenEvent onDamaged;
    public UnityEvent onHealed;
    public UnityEvent onLegWounded;
    public UnityEvent onLegWoundCleared;
    public UnityEvent onDeath;

    public event Action<float, float> HealthChanged;
    public event Action<float, Vector2> Damaged;
    public event Action Healed;
    public event Action LegWounded;
    public event Action LegWoundCleared;
    public event Action Died;

    public bool IsDead { get; private set; }
    public bool HasLegWound { get; private set; }
    public bool CanTakeDamage => !IsDead && invulnerabilityTimer <= 0f;
    public float HealthPercent => maxHealth <= 0f ? 0f : currentHealth / maxHealth;

    private Rigidbody2D rb;
    private PlayerMovement2D movement;
    private GrapplingHook grapplingHook;
    private float invulnerabilityTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement2D>();
        grapplingHook = GetComponent<GrapplingHook>();
    }

    private void Start()
    {
        if (clampStartHealthToMax)
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        ApplyLegWoundModifiers();
        NotifyHealthChanged();

        if (currentHealth <= 0f)
            Die();
    }

    private void Update()
    {
        if (invulnerabilityTimer > 0f)
            invulnerabilityTimer -= Time.deltaTime;
    }

    public void TakeDamage(float amount, Vector2 sourcePosition)
    {
        TakeDamage(amount, sourcePosition, true);
    }

    public void TakeDamage(float amount, Vector2 sourcePosition, bool applyKnockback)
    {
        if (amount <= 0f || !CanTakeDamage)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        invulnerabilityTimer = invulnerabilityDuration;

        if (applyKnockback)
            ApplyKnockback(sourcePosition);

        Damaged?.Invoke(amount, sourcePosition);
        onDamaged?.Invoke(amount, sourcePosition);
        NotifyHealthChanged();

        if (currentHealth <= 0f)
            Die();
    }

    public void ApplyLegWound()
    {
        if (IsDead || HasLegWound)
            return;

        HasLegWound = true;
        ApplyLegWoundModifiers();
        LegWounded?.Invoke();
        onLegWounded?.Invoke();
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || IsDead)
            return;

        float previousHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (!Mathf.Approximately(previousHealth, currentHealth))
        {
            Healed?.Invoke();
            onHealed?.Invoke();
            NotifyHealthChanged();
        }
    }

    public void HealPercentOfMax(float percent)
    {
        Heal(maxHealth * Mathf.Clamp01(percent));
    }

    public void HealToMaxPercent(float percent)
    {
        if (IsDead)
            return;

        float targetHealth = maxHealth * Mathf.Clamp01(percent);
        if (currentHealth < targetHealth)
            Heal(targetHealth - currentHealth);
    }

    public void FullHeal()
    {
        if (IsDead)
            return;

        currentHealth = maxHealth;
        ClearLegWounds();
        Healed?.Invoke();
        onHealed?.Invoke();
        NotifyHealthChanged();
    }

    public void ClearLegWounds()
    {
        if (!HasLegWound)
            return;

        HasLegWound = false;
        ApplyLegWoundModifiers();
        LegWoundCleared?.Invoke();
        onLegWoundCleared?.Invoke();
    }

    public void SetHealthFromCheckpoint(float savedHealth, bool savedLegWound)
    {
        IsDead = false;
        currentHealth = Mathf.Clamp(savedHealth, 0f, maxHealth);
        HasLegWound = savedLegWound;
        ApplyLegWoundModifiers();
        NotifyHealthChanged();

        if (currentHealth <= 0f)
            Die();
    }

    private void ApplyKnockback(Vector2 sourcePosition)
    {
        if (grapplingHook != null && grapplingHook.IsActive())
            grapplingHook.ForceDisconnect();

        if (rb == null)
            return;

        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        if (direction.sqrMagnitude < 0.001f)
            direction = transform.localScale.x >= 0f ? Vector2.left : Vector2.right;

        direction = (direction + Vector2.up * upwardKnockbackBonus).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackImpulse, ForceMode2D.Impulse);

        if (movement != null)
            movement.LockControl(controlLockDuration);
    }

    private void ApplyLegWoundModifiers()
    {
        if (movement == null)
            return;

        if (HasLegWound)
            movement.SetStatusMultipliers(woundedMoveSpeedMultiplier, woundedJumpForceMultiplier);
        else
            movement.SetStatusMultipliers(1f, 1f);
    }

    private void NotifyHealthChanged()
    {
        HealthChanged?.Invoke(currentHealth, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;
        Died?.Invoke();
        onDeath?.Invoke();
    }
}
