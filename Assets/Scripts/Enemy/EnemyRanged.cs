using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class EnemyRanged : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("UI")]
    public GameObject healthBarPrefab;   // ← assign your prefab here
    private Slider healthSlider;
    private Transform healthBarTransform;

    [Header("Settings")]
    public float detectionRange = 15f;
    public float stopDistance = 2f;

    [Header("Attack Settings")]
    public float attackRange = 6f;
    public float attackCooldown = 2f;
    public float projectileSpeed = 12f;
    public float fireDelay = 0.5f;
    private bool isAttacking = false;
    private float lastAttackTime;

    [Header("Speeds")]
    public float patrolSpeed = 0.5f;
    public float chaseSpeed = 1f;

    [Header("Patrol Settings")]
    public float patrolRadius = 20f;
    public float patrolWaitTime = 2f;
    public float reachThreshold = 1.0f;

    [Header("Health")]
    public float maxHP = 100f;
    private float currentHP;
    private bool isDead = false;

    private bool isChasing = false;
    private bool isPatrolling = false;
    private Coroutine patrolRoutine;

    void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();
        if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;

        currentHP = maxHP;

        agent.updateRotation = true;
        agent.updateUpAxis = true;
        agent.isStopped = false;

        // Instantiate health bar
        if (healthBarPrefab)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform);
            hb.transform.localPosition = new Vector3(0, 2.2f, 0); // adjust height
            healthBarTransform = hb.transform;
            healthSlider = hb.GetComponentInChildren<Slider>();
            if (healthSlider) healthSlider.value = 1f;
        }

        StartPatrol();
    }

    void Update()
    {
        if (isDead || !player || agent == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Face health bar to camera
        if (healthBarTransform && Camera.main)
        {
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0; // ignore camera tilt so bar stays upright
            healthBarTransform.rotation = Quaternion.LookRotation(camForward);
        }

        // --- Detect player ---
        if (distance <= detectionRange)
        {
            if (!isChasing)
            {
                StopPatrol();
                isChasing = true;
                agent.speed = chaseSpeed;
            }

            // --- Combat behavior ---
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
        else
        {
            if (isChasing)
            {
                isChasing = false;
                StartPatrol();
            }
        }

        // --- Animation movement ---
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

        Vector3 dir = (player.position - transform.position);
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(fireDelay);

        FireProjectile();

        yield return new WaitForSeconds(attackCooldown - fireDelay + 2f);
        isAttacking = false;
    }

    void FireProjectile()
    {
        if (!projectilePrefab || !firePoint || isDead) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position + Vector3.up * 1.0f - firePoint.position).normalized;
            rb.velocity = direction * projectileSpeed;
        }
    }

    // -------------------- PATROL --------------------
    void StartPatrol()
    {
        if (isPatrolling || agent == null) return;
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

    // -------------------- DAMAGE SYSTEM --------------------
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP -= amount;
        animator.SetTrigger("Hurt");

        // Update health bar
        if (healthSlider)
            healthSlider.value = Mathf.Clamp01(currentHP / maxHP);

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("Die");
        agent.isStopped = true;

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        if (healthBarTransform)
            Destroy(healthBarTransform.gameObject);

        Destroy(gameObject, 4.6f);
    }


    // -------------------- DEBUG --------------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
