using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerFallDamage2D : MonoBehaviour
{
    [Header("Detection")]
    public LayerMask groundLayer;
    public float minimumAirTime = 0.15f;

    [Header("Medium Fall")]
    public float mediumFallSpeed = 14f;
    public float mediumFallDamage = 15f;

    [Header("High Fall")]
    public float highFallSpeed = 20f;
    public float highFallDamage = 30f;
    public bool highFallWoundsLegs = true;

    [Header("Knockback")]
    public bool applyFallKnockback;

    private Rigidbody2D rb;
    private PlayerHealth health;
    private PlayerMovement2D movement;
    private float fastestDownwardSpeed;
    private float airTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<PlayerHealth>();
        movement = GetComponent<PlayerMovement2D>();
    }

    private void Start()
    {
        if (groundLayer == 0 && movement != null)
            groundLayer = movement.groundLayer;
    }

    private void Update()
    {
        bool grounded = movement != null && movement.isGrounded;

        if (grounded)
        {
            fastestDownwardSpeed = 0f;
            airTimer = 0f;
            return;
        }

        airTimer += Time.deltaTime;
        fastestDownwardSpeed = Mathf.Max(fastestDownwardSpeed, Mathf.Max(0f, -rb.linearVelocity.y));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsGroundCollision(collision) || airTimer < minimumAirTime)
            return;

        float damage = 0f;
        bool woundLegs = false;

        if (fastestDownwardSpeed >= highFallSpeed)
        {
            damage = highFallDamage;
            woundLegs = highFallWoundsLegs;
        }
        else if (fastestDownwardSpeed >= mediumFallSpeed)
        {
            damage = mediumFallDamage;
        }

        if (damage > 0f)
        {
            Vector2 sourcePosition = collision.contactCount > 0
                ? collision.GetContact(0).point
                : (Vector2)transform.position + Vector2.down;

            if (woundLegs && health.CanTakeDamage)
                health.ApplyLegWound();

            health.TakeDamage(damage, sourcePosition, applyFallKnockback);
        }

        fastestDownwardSpeed = 0f;
        airTimer = 0f;
    }

    private bool IsGroundCollision(Collision2D collision)
    {
        if (groundLayer == 0)
            return true;

        int otherLayerMask = 1 << collision.gameObject.layer;
        return (groundLayer.value & otherLayerMask) != 0;
    }
}
