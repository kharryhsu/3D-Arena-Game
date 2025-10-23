using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 10f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Just destroy when it hits something that isn't another enemy
        if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
