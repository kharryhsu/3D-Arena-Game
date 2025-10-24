using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private EnemyRanged enemy;

    void Awake()
    {
        enemy = GetComponentInParent<EnemyRanged>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            PlayerProjectile proj = other.GetComponent<PlayerProjectile>();
            if (proj != null && enemy != null)
            {
                Debug.Log("HIT!");
                enemy.TakeDamage(proj.damage);
                Destroy(other.gameObject);
            }
        }
    }
}
