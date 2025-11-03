using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public HealthBarUI healthBarUI;
    public Animator animator;
    public SceneFader sceneFader;

    void Awake()
    {
        // Load saved HP if PlayerStats exists
        if (PlayerStats.Instance != null)
        {
            maxHealth = PlayerStats.Instance.maxHealth;
            currentHealth = PlayerStats.Instance.currentHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }

        if (healthBarUI)
            healthBarUI.SetMaxHealth(maxHealth);
        if (healthBarUI)
            healthBarUI.SetHealth(currentHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update persistent value
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.currentHealth = currentHealth;

        if (healthBarUI)
            healthBarUI.SetHealth(currentHealth);

        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        // Sync persistent data
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.currentHealth = currentHealth;

        if (healthBarUI)
            healthBarUI.SetHealth(currentHealth);

        Debug.Log($"{gameObject.name} healed by {amount}. HP: {currentHealth}");
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        animator.SetTrigger("Die");

        // Start coroutine for delayed fade
        StartCoroutine(HandleDeathSequence());
    }

    IEnumerator HandleDeathSequence()
    {
        // Wait for 3 seconds (or however long your death animation lasts)
        yield return new WaitForSeconds(3f);

        // Try to find SceneFader
        SceneFader fader = FindObjectOfType<SceneFader>();
        fader.FadeToScene("Lose");

        // Optional: Destroy the player after fade starts
        Destroy(gameObject);
    }
}
