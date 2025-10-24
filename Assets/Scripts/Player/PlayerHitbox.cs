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
        if (other.CompareTag("EnemyProjectile"))
        {
            EnemyProjectile proj = other.GetComponent<EnemyProjectile>();
            if (proj != null)
            {
                health.TakeDamage(proj.damage);
                Destroy(other.gameObject);
            }
        }
    }
}
