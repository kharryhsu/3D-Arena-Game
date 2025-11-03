using UnityEngine;

public class HitEffectManager : MonoBehaviour
{
    public static HitEffectManager Instance;

    [Header("Hit Effect Prefabs")]
    public GameObject enemyHitEffect;  // e.g. red sparks or explosion
    public GameObject playerHitEffect; // e.g. blue/white flash

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnEnemyHitEffect(Vector3 position)
    {
        if (enemyHitEffect)
            Instantiate(enemyHitEffect, position, Quaternion.identity);
    }

    public void SpawnPlayerHitEffect(Vector3 position)
    {
        if (playerHitEffect)
            Instantiate(playerHitEffect, position, Quaternion.identity);
    }
}
