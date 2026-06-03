using UnityEngine;

public enum PlayerHealingKind
{
    Bandage,
    MedicalKit
}

public class PlayerHealingItem2D : MonoBehaviour
{
    public PlayerHealingKind healingKind = PlayerHealingKind.Bandage;
    public bool consumeOnUse = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health == null)
            return;

        if (healingKind == PlayerHealingKind.Bandage)
            health.HealPercentOfMax(0.25f);
        else
            health.FullHeal();

        if (consumeOnUse)
            Destroy(gameObject);
    }
}
