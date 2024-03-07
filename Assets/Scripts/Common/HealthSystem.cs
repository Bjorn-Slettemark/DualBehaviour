using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    [SerializeField]
    public float currentHealth;

    void Start()
    {
        // Initialize health when the game starts
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        // Reduce health by the damage amount
        currentHealth -= amount;

        // Check if health has dropped below zero
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (this.CompareTag("Player"))
        {
            //GameManager.Instance.PlayerDied();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Optional: Method to heal the AI
    public void Heal(float amount)
    {
        currentHealth += amount;
        // Ensure we do not exceed max health
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
}
