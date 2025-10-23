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
    private bool isDead = false;

    void Start()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();
        currentHP = maxHP;
    }

    void Update()
    {
        if (isDead || !player) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            if (distance <= attackRange)
            {
                agent.isStopped = true;
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

                if (Time.time >= nextAttackTime)
                {
                    nextAttackTime = Time.time + attackCooldown;
                    animator.SetTrigger("Attack");
                    StartCoroutine(FireAfterDelay(0.5f)); // wait half a second
                }
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
        }
        else
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
        }
    }

    IEnumerator FireAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShootProjectile();
    }

    // Called from Animation Event
    public void ShootProjectile()
    {
        if (!firePoint || !projectilePrefab || isDead) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector3 dir = (player.position - firePoint.position).normalized;
        proj.GetComponent<Rigidbody>().velocity = dir * 12f; // projectile speed
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

        // Disable collider to prevent more hits
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        Destroy(gameObject, 3f);
    }
}
