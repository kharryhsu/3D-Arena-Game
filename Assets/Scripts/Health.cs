using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public HealthBarUI healthBarUI;

    void Awake()
    {
        currentHealth = maxHealth;
        if (healthBarUI)
            healthBarUI.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBarUI)
            healthBarUI.SetHealth(currentHealth);

        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        // Destroy(gameObject, 1f);
    }
}
