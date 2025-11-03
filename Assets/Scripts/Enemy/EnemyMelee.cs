using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMelee : EnemyBase
{
    [Header("Combat Settings")]
    public Transform player;
    public EnemyMeleeHitbox meleeHitbox;
    public float detectionRange = 15f;
    public float attackRange = 2.2f;
    public float attackCooldown = 2.5f;
    public float attackDelay = 0.3f;
    public float damage = 10f;

    [Header("Movement Settings")]
    public float patrolSpeed = 0.5f;
    public float chaseSpeed = 1.2f;
    public float patrolRadius = 20f;
    public float patrolWaitTime = 2f;
    public float reachThreshold = 1f;

    private bool isChasing = false;
    private bool isPatrolling = false;
    private bool isAttacking = false;
    private float lastAttackTime;
    private Coroutine patrolRoutine;

    protected override void Start()
    {
        base.Start();

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (!meleeHitbox)
            meleeHitbox = GetComponentInChildren<EnemyMeleeHitbox>();

        if (agent)
        {
            agent.updateRotation = true;
            agent.updateUpAxis = true;
            agent.isStopped = false;
        }

        StartPatrol();
    }

    protected override void Update()
    {
        base.Update();
        if (isDead || player == null || agent == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        Debug.DrawLine(transform.position, player.position, Color.red);

        // -------------------- DETECTION & CHASE --------------------
        if (distance <= detectionRange)
        {
            if (!isChasing)
            {
                StopPatrol();
                isChasing = true;
                agent.speed = chaseSpeed;
                Debug.Log($"{name} detected player! Starting chase!");
            }

            if (distance <= attackRange)
            {
                agent.isStopped = true;
                TryAttack();
            }
            else
            {
                if (!isAttacking)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                }
            }
        }
        else if (isChasing)
        {
            isChasing = false;
            StartPatrol();
            Debug.Log($"{name} lost player. Returning to patrol.");
        }

        // -------------------- ANIMATION SPEED --------------------
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

        // Stop moving while attacking
        agent.isStopped = true;

        animator.SetTrigger("Attack");
        Debug.Log($"{name} attacking!");

        float elapsed = 0f;
        while (elapsed < attackDelay)
        {
            // Face player while winding up attack
            FacePlayer();
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Enable hitbox when attack lands
        if (meleeHitbox)
        {
            meleeHitbox.EnableHitbox();
            Debug.Log($"{name}: Melee hitbox enabled!");
        }

        yield return new WaitForSeconds(0.4f);

        if (meleeHitbox)
        {
            meleeHitbox.DisableHitbox();
            Debug.Log($"{name}: Melee hitbox disabled!");
        }

        isAttacking = false;
    }

    void FacePlayer()
    {
        if (!player) return;
        Vector3 dir = (player.position - transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
    }

    // -------------------- PATROL --------------------
    void StartPatrol()
    {
        if (isPatrolling || agent == null || isDead) return;
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
        if (agent) agent.isStopped = true;
    }

    IEnumerator PatrolRoutine()
    {
        isPatrolling = true;
        if (agent) agent.speed = patrolSpeed;

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
                        Debug.Log($"{name} reached patrol point.");
                        break;
                    }
                    yield return null;
                }

                animator.SetFloat("Speed", 0f);
                yield return new WaitForSeconds(patrolWaitTime);
            }
            else yield return null;
        }

        isPatrolling = false;
    }

    // -------------------- DIE OVERRIDE --------------------
    protected override void Die()
    {
        base.Die(); // Handles animation, collider, destroy, etc.
        StopPatrol();
        Debug.Log($"{name} has died.");
    }

    // -------------------- GIZMOS --------------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}
