using UnityEngine;

public class EnemyMeleeHitbox : MonoBehaviour
{
    EnemyMelee enemyMelee;
    public float damage;
    private Collider col;
    private bool canHit = true;

    void Awake()
    {
        col = GetComponent<Collider>();

        // find the EnemyMelee component on this object or a parent and copy its damage
        enemyMelee = GetComponent<EnemyMelee>() ?? GetComponentInParent<EnemyMelee>();
        if (enemyMelee != null)
        {
            damage = enemyMelee.damage;
        }

        if (col) col.enabled = false;
    }

    public void EnableHitbox()
    {
        if (col)
        {
            col.enabled = true;
            canHit = true; // reset hit state when enabling
            Debug.Log($"{name}: Collider enabled");
        }
    }

    public void DisableHitbox()
    {
        if (col)
        {
            col.enabled = false;
            Debug.Log($"{name}: Collider disabled");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log($"Hit player collider — notifying PlayerHitbox.");
            canHit = false; // prevent multiple triggers this swing
        }
    }
}
