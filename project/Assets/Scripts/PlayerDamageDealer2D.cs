using UnityEngine;

public class PlayerDamageDealer2D : MonoBehaviour
{
    [Header("Damage")]
    [Min(0f)] public float damage = 10f;

    [Header("Hit Rules")]
    public bool damageOnTriggerEnter = true;
    public bool damageOnCollisionEnter = true;
    public float hitCooldown = 0.5f;
    public Transform damageSource;

    private float nextHitTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (damageOnTriggerEnter)
            TryDamage(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (damageOnCollisionEnter)
            TryDamage(collision.collider);
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
