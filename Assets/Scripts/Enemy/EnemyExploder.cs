using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyExploder : EnemyBase
{
    [Header("References")]
    public Transform player;

    [Header("Exploder Settings")]
    public float detectionRange = 15f;
    public float explodeRange = 2.5f;
    public float explodeHoldTime = 1.5f; // Player must stay in range for this long
    public float explosionDelay = 0.5f;  // Small delay after trigger before explosion
    public float explosionRadius = 4f;
    public float explosionDamage = 40f;
    public GameObject explosionEffect;

    [Header("Movement Settings")]
    public float patrolSpeed = 0.5f;
    public float chaseSpeed = 2f;
    public float patrolRadius = 20f;
    public float patrolWaitTime = 2f;
    public float reachThreshold = 1f;

    private bool isChasing = false;
    private bool isPatrolling = false;
    private bool isExploding = false;
    private Coroutine patrolRoutine;
    private Coroutine explodeRoutine; // So we can cancel it

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
        if (isDead || isExploding || !player) return;

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

            agent.isStopped = false;
            agent.SetDestination(player.position);

            // --- Handle explosion trigger ---
            if (distance <= explodeRange)
            {
                if (explodeRoutine == null)
                    explodeRoutine = StartCoroutine(ExplodeHoldRoutine());
            }
            else
            {
                // Player left explode range — cancel explosion
                if (explodeRoutine != null)
                {
                    StopCoroutine(explodeRoutine);
                    explodeRoutine = null;
                    ResetBlink();
                    Debug.Log($"{name} canceled explosion — player moved away!");
                }
            }
        }
        else if (isChasing)
        {
            isChasing = false;
            StartPatrol();
            Debug.Log($"{name} lost player. Returning to patrol.");
        }

        // -------------------- ANIMATION --------------------
        float speedPercent = agent.velocity.magnitude / Mathf.Max(agent.speed, 0.01f);
        animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
    }

    // -------------------- HOLD EXPLOSION ROUTINE --------------------
    IEnumerator ExplodeHoldRoutine()
    {
        float timer = 0f;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Vector3 originalScale = transform.localScale;

        Debug.Log($"{name} charging explosion...");

        while (timer < explodeHoldTime)
        {
            if (Vector3.Distance(transform.position, player.position) > explodeRange)
            {
                ResetBlink();
                yield break; // Player escaped — stop
            }

            timer += Time.deltaTime;

            // Blink and pulse while charging
            float scaleFactor = 1f + Mathf.PingPong(Time.time * 4f, 0.25f);
            transform.localScale = originalScale * scaleFactor;

            float blink = Mathf.PingPong(Time.time * 8f, 1f);
            Color blinkColor = Color.Lerp(Color.white, Color.red, blink);
            foreach (var r in renderers)
            {
                if (r.material.HasProperty("_Color"))
                    r.material.color = blinkColor;
            }

            yield return null;
        }

        // Ready to explode!
        transform.localScale = originalScale;
        foreach (var r in renderers)
        {
            if (r.material.HasProperty("_Color"))
                r.material.color = Color.white;
        }

        explodeRoutine = null;
        StartCoroutine(ExplodeRoutine());
    }

    void ResetBlink()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            if (r.material.HasProperty("_Color"))
                r.material.color = Color.white;
        }
        transform.localScale = Vector3.one;
    }

    // -------------------- EXPLOSION --------------------
    IEnumerator ExplodeRoutine()
    {
        if (isExploding) yield break;
        isExploding = true;
        agent.isStopped = true;
        animator.SetTrigger("Explode");

        Debug.Log($"{name} exploding now!");

        yield return new WaitForSeconds(explosionDelay);
        
        SoundManager.Instance?.PlayExplosion();

        if (explosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                hit.GetComponent<Health>()?.TakeDamage(explosionDamage);
        }

        Die();
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
        Gizmos.DrawWireSphere(transform.position, explodeRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
