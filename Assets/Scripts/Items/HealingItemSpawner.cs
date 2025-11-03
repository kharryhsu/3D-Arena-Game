using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class HealingItemSpawner : MonoBehaviour
{
    [Header("Healing Item Prefabs")]
    public List<GameObject> healingItemPrefabs;  // Assign potion / medkit prefabs

    [Header("Spawn Settings")]
    public int spawnCount = 10;           // how many to place
    public float spawnRadius = 40f;       // radius from this object
    public float navSampleDistance = 3f;  // NavMesh validation distance
    public float heightOffset = 0.5f;     // lift above ground level
    public bool spawnOnStart = true;      // auto spawn when level loads

    [Header("Gizmos")]
    public Color gizmoColor = new Color(0f, 0.8f, 1f, 0.3f);

    private List<GameObject> spawnedItems = new List<GameObject>();

    void Start()
    {
        if (spawnOnStart)
            SpawnHealingItems();
    }

    public void SpawnHealingItems()
    {
        if (healingItemPrefabs.Count == 0)
        {
            Debug.LogWarning("No healing item prefabs assigned!");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = transform.position.y;

            // Find a valid ground position on NavMesh
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, navSampleDistance, NavMesh.AllAreas))
            {
                Vector3 spawnPos = hit.position + Vector3.up * heightOffset;

                GameObject prefab = healingItemPrefabs[Random.Range(0, healingItemPrefabs.Count)];
                GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);
                spawnedItems.Add(item);
            }
            else
            {
                Debug.Log($"Failed to find ground for healing item {i}");
            }
        }

        Debug.Log($"Spawned {spawnedItems.Count} healing items.");
    }

    public void ClearItems()
    {
        foreach (var item in spawnedItems)
        {
            if (item) Destroy(item);
        }
        spawnedItems.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }
}
