using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void OnTriggerEnter(Collider other)
    {
        // --- Enemy projectile hit ---
        if (other.CompareTag("EnemyProjectile"))
        {
            EnemyProjectile proj = other.GetComponent<EnemyProjectile>();
            if (proj != null)
            {
                health.TakeDamage(proj.damage);

                // Spawn hit effect at projectile position
                if (HitEffectManager.Instance != null)
                    HitEffectManager.Instance.SpawnPlayerHitEffect(other.transform.position);

                Destroy(other.gameObject);
            }
        }

        // --- Melee enemy hit ---
        if (other.CompareTag("EnemyMeleeHitbox"))
        {
            EnemyMeleeHitbox melee = other.GetComponent<EnemyMeleeHitbox>();
            if (melee != null)
            {
                Debug.Log($"Player hit by melee for {melee.damage} dmg!");
                health.TakeDamage(melee.damage);

                // Spawn hit effect at contact point
                if (HitEffectManager.Instance != null)
                    HitEffectManager.Instance.SpawnPlayerHitEffect(other.ClosestPoint(transform.position));
            }
        }
    }
}
