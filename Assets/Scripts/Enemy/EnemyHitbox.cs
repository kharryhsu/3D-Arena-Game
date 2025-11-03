using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private EnemyBase enemy;

    void Awake()
    {
        enemy = GetComponentInParent<EnemyBase>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            PlayerProjectile proj = other.GetComponent<PlayerProjectile>();
            if (proj != null && enemy != null)
            {
                Debug.Log($"Hit detected! {enemy.gameObject.name} took {proj.damage} damage from projectile!");
                enemy.TakeDamage(proj.damage);

                // Spawn enemy hit effect at hit position
                if (HitEffectManager.Instance != null)
                    HitEffectManager.Instance.SpawnEnemyHitEffect(other.transform.position);

                Destroy(other.gameObject);
            }
        }
    }
}
