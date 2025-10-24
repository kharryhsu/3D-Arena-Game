using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 10f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore other projectiles or the player
        if (other.CompareTag("Player"))
            return;
    }
}
