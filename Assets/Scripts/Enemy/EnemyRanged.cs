using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRanged : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public NavMeshAgent agent;
    public Transform firePoint;
    public GameObject projectilePrefab;
    // public GameObject hitEffectPrefab;

    [Header("Stats")]
    public float maxHP = 100f;
    private float currentHP;
    public float detectionRange = 15f;
    public float attackRange = 10f;
    public float attackCooldown = 2f;
    private float nextAttackTime = 0f;
    private float fireDelayTime = 0.5f;
    private bool isDead = false;

    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 2f;
    private Vector3 patrolTarget;
    private bool isPatrolling = false;

    [Header("Rotation Fix")]
    public float modelFacingOffset = 0f;

    void Start()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();
        currentHP = maxHP;

        StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        if (isDead || !player) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            StopCoroutine(PatrolRoutine());
            HandleCombat(distance);
        }

        else if (!isPatrolling)
        {
            StartCoroutine(PatrolRoutine());
        }
    }

    void HandleCombat(float distance)
    {
        if (distance <= attackRange)
        {
            agent.isStopped = true;
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            transform.Rotate(0, modelFacingOffset, 0);

            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldown + fireDelayTime;
                animator.SetTrigger("Attack");
                StartCoroutine(FireAfterDelay(fireDelayTime)); // wait before shooting
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    IEnumerator FireAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShootProjectile();
    }

    public void ShootProjectile()
    {
        if (!firePoint || !projectilePrefab || isDead) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector3 dir = (player.position - firePoint.position).normalized;
        proj.GetComponent<Rigidbody>().velocity = dir * 12f;
    }

    IEnumerator PatrolRoutine()
    {
        isPatrolling = true;

        while (!isDead)
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolTarget = hit.position;
                agent.SetDestination(patrolTarget);
                animator.SetFloat("Speed", 1f);
            }

            // Wait until it reaches destination
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            // Idle wait
            animator.SetFloat("Speed", 0f);
            yield return new WaitForSeconds(patrolWaitTime);
        }

        isPatrolling = false;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP -= amount;
        animator.SetTrigger("Hurt");

        // if (hitEffectPrefab)
        //     Instantiate(hitEffectPrefab, transform.position + Vector3.up, Quaternion.identity);

        if (currentHP <= 0f)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("Die");
        agent.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        Destroy(gameObject, 3f);
    }
}
