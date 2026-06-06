using UnityEngine;

public class ContactDamage2D : MonoBehaviour
{
    [Header("Collider")]
    public Collider2D contactCollider;

    [Header("Damage")]
    [Min(0f)] public float damage = 10f;
    public Transform damageSource;

    [Header("Hit Rules")]
    public bool damageOnTriggerEnter = true;
    public bool damageOnCollisionEnter = true;
    public float hitCooldown = 0.5f;

    private float nextHitTime;

    private void Reset()
    {
        contactCollider = GetComponent<Collider2D>();
    }

    private void OnValidate()
    {
        if (contactCollider == null)
            contactCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (damageOnTriggerEnter && TriggerUsesContactCollider(other))
            TryDamage(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (damageOnCollisionEnter && CollisionUsesOwnCollider(collision))
            TryDamage(collision.collider);
    }

    private bool TriggerUsesContactCollider(Collider2D other)
    {
        if (contactCollider == null)
            return true;

        return contactCollider.Distance(other).isOverlapped;
    }

    private bool CollisionUsesOwnCollider(Collision2D collision)
    {
        return contactCollider == null || collision.otherCollider == contactCollider;
    }

    private void TryDamage(Collider2D other)
    {
        if (Time.time < nextHitTime)
            return;

        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health == null)
            return;

        Vector2 sourcePosition = damageSource != null ? damageSource.position : transform.position;
        health.TakeDamage(damage, sourcePosition);
        nextHitTime = Time.time + hitCooldown;
    }
}
