using UnityEngine;

public class PlayerCheckpoint2D : MonoBehaviour
{
    [Range(0f, 1f)] public float healUpToMaxPercent = 0.5f;
    public bool clearLegWounds;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health == null)
            return;

        health.HealToMaxPercent(healUpToMaxPercent);

        if (clearLegWounds)
            health.ClearLegWounds();
    }
}
