using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyRanged : EnemyBase
{
    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("Detection")]
    public float detectionRange = 15f;
    public float attackRange = 6f;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public float projectileSpeed = 12f;
    public float fireDelay = 0.5f;

    [Header("Movement Settings")]
    public float patrolSpeed = 0.5f;
    public float chaseSpeed = 1f;
    public float patrolRadius = 20f;
    public float patrolWaitTime = 2f;
    public float reachThreshold = 1f;

    private bool isChasing = false;
    private bool isPatrolling = false;
    private bool isAttacking = false;
    private Coroutine patrolRoutine;
    private float lastAttackTime;

    // -------------------- START --------------------
    protected override void Start()
    {
        base.Start();
        if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartPatrol();
    }

    // -------------------- UPDATE --------------------
    protected override void Update()
    {
        base.Update();
        if (isDead || !player) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // --- Detection and Chasing ---
        if (distance <= detectionRange)
        {
            if (!isChasing)
            {
                StopPatrol();
                isChasing = true;
                agent.speed = chaseSpeed;
            }

            if (distance <= attackRange)
            {
                agent.isStopped = true;
                TryAttack();
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
        else if (isChasing)
        {
            isChasing = false;
            StartPatrol();
        }

        // --- Animation Movement ---
        float speedPercent = agent.velocity.magnitude / Mathf.Max(agent.speed, 0.01f);
        animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
    }

    // -------------------- ATTACK --------------------
    void TryAttack()
    {
        if (isAttacking || isDead) return;
        if (Time.time - lastAttackTime < attackCooldown) return;

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Face player
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(fireDelay);

        FireProjectile();

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void FireProjectile()
    {
        if (!projectilePrefab || !firePoint || isDead) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        SoundManager.Instance?.PlayShoot();
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position + Vector3.up * 1.0f - firePoint.position).normalized;
            rb.velocity = direction * projectileSpeed;
        }

        Debug.Log($"{name} fired a projectile!");
    }

    // -------------------- PATROL --------------------
    void StartPatrol()
    {
        if (isPatrolling || isDead) return;
        patrolRoutine = StartCoroutine(PatrolRoutine());
    }

    void StopPatrol()
    {
        if (patrolRoutine != null)
        {
            StopCoroutine(patrolRoutine);
            patrolRoutine = null;
        }
        isPatrolling = false;
        agent.isStopped = true;
    }

    IEnumerator PatrolRoutine()
    {
        isPatrolling = true;
        agent.speed = patrolSpeed;

        while (!isChasing && !isDead)
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                Vector3 target = hit.position;
                agent.isStopped = false;
                agent.SetDestination(target);

                Debug.Log($"{name} patrolling to {target}");

                while (!isChasing && !isDead)
                {
                    if (!agent.pathPending && agent.remainingDistance <= reachThreshold)
                    {
                        agent.isStopped = true;
                        break;
                    }
                    yield return null;
                }

                animator.SetFloat("Speed", 0f);
                yield return new WaitForSeconds(patrolWaitTime);
            }
            else
            {
                yield return null;
            }
        }

        isPatrolling = false;
    }

    // -------------------- DEBUG --------------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}
