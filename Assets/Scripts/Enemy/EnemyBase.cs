using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public NavMeshAgent agent;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    protected float currentHealth;
    protected bool isDead = false;

    [Header("UI")]
    public GameObject healthBarPrefab;
    protected Slider healthSlider;
    protected Transform healthBarTransform;

    [Header("Potion Drop Settings")]
    [Range(0f, 1f)] public float dropChance = 0.3f;
    public GameObject damagePotionPrefab;
    public float dropHeightOffset = 0.4f;
    public float navSampleRadius = 3f;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        if (!animator) animator = GetComponent<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();

        if (healthBarPrefab)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform);
            hb.transform.localPosition = new Vector3(0, 2.2f, 0);
            healthBarTransform = hb.transform;
            healthSlider = hb.GetComponentInChildren<Slider>();
            if (healthSlider) healthSlider.value = 1f;
        }

        // Register enemy
        if (GameManager.Instance)
            GameManager.Instance.RegisterEnemy(this);
    }

    protected virtual void Update()
    {
        if (healthBarTransform && Camera.main)
        {
            healthBarTransform.LookAt(
                healthBarTransform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up
            );
        }
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (animator) animator.SetTrigger("Hurt");

        if (healthSlider)
            healthSlider.value = Mathf.Clamp01(currentHealth / maxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator) animator.SetTrigger("Die");
        if (agent) agent.isStopped = true;

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        if (healthBarTransform)
            Destroy(healthBarTransform.gameObject);

        TryDropPotion();

        if (GameManager.Instance)
        {
            GameManager.Instance.AddScore(100, transform.position + Vector3.up * 2f);
            GameManager.Instance.UnregisterEnemy(this);
        }

        Destroy(gameObject, 5f);
    }

    protected void TryDropPotion()
    {
        if (damagePotionPrefab == null) return;
        if (Random.value > dropChance) return;

        Vector3 spawnPos = transform.position;
        if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, navSampleRadius, NavMesh.AllAreas))
            spawnPos = hit.position + Vector3.up * dropHeightOffset;
        else
            spawnPos += Vector3.up * dropHeightOffset;

        Instantiate(damagePotionPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"{name} dropped a damage potion at {spawnPos}");
    }
}
