using UnityEngine;
using UnityEngine.AI;

public class DamagePotionDrop : MonoBehaviour
{
    [Header("Drop Settings")]
    [Range(0f, 1f)]
    public float dropChance = 0.3f;          // 30% chance to drop
    public GameObject damagePotionPrefab;

    [Header("Spawn Settings")]
    public float dropHeightOffset = 0.4f;    // how high above ground to spawn
    public float navSampleRadius = 3f;       // how far to check for valid ground

    private bool hasDropped = false;

    public void DropPotion()
    {
        if (hasDropped || damagePotionPrefab == null)
            return;

        hasDropped = true;

        // Random chance check
        if (Random.value > dropChance)
            return;

        Vector3 spawnPos = transform.position;

        // ✅ Use NavMesh to align with ground under enemy
        if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, navSampleRadius, NavMesh.AllAreas))
        {
            spawnPos = hit.position + Vector3.up * dropHeightOffset;
        }
        else
        {
            // fallback if no valid navmesh found
            spawnPos += Vector3.up * dropHeightOffset;
        }

        GameObject item = Instantiate(damagePotionPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"{gameObject.name} dropped a damage potion at {spawnPos}");
    }
}
