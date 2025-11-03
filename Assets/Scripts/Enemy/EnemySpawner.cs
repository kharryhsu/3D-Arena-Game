using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public List<GameObject> enemyPrefabs; // assign your melee, ranged, exploder, etc.

    [Header("Spawn Settings")]
    public int spawnCount = 5;
    public float spawnRadius = 30f;
    public bool spawnOnStart = true;

    [Header("NavMesh Area")]
    public float navSampleDistance = 5f; // how far to check for valid navmesh

    [Header("Safety Settings")]
    public int maxSpawnAttemptsPerEnemy = 20; // prevent infinite loops

    [Header("Gizmos")]
    public Color gizmoColor = new Color(0, 1, 0, 0.3f);

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool hasSpawned = false;

    void Start()
    {
        if (spawnOnStart)
            SpawnAllEnemies();
    }

    public void SpawnAllEnemies()
    {
        if (hasSpawned)
        {
            Debug.LogWarning("Spawner has already spawned enemies — skipping.");
            return;
        }

        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned to spawner!");
            return;
        }

        int spawned = 0;
        int attempts = 0;

        while (spawned < spawnCount && attempts < spawnCount * maxSpawnAttemptsPerEnemy)
        {
            attempts++;

            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = transform.position.y;

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, navSampleDistance, NavMesh.AllAreas))
            {
                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                GameObject enemy = Instantiate(prefab, hit.position, Quaternion.identity);
                spawnedEnemies.Add(enemy);
                spawned++;
                Debug.Log($"✅ Spawned enemy #{spawned}: {enemy.name} at {hit.position}");
            }
            else
            {
                // Optional debug to see how often it fails
                // Debug.LogWarning($"❌ Failed to find NavMesh position (Attempt {attempts})");
            }
        }

        if (spawned < spawnCount)
            Debug.LogWarning($"⚠️ Only spawned {spawned}/{spawnCount} enemies after {attempts} attempts.");
        else
            Debug.Log($"🎯 Successfully spawned {spawned} enemies after {attempts} attempts.");

        hasSpawned = true;
    }

    public void ClearEnemies()
    {
        foreach (var e in spawnedEnemies)
        {
            if (e)
                Destroy(e);
        }

        spawnedEnemies.Clear();
        hasSpawned = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
