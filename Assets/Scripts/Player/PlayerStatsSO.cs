using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Game/Player Stats", order = 0)]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Health")]
    public float maxHealth = 100f;
    [SerializeField]
    private float health;

    [Header("Quantum Energy")]
    public float maxQuantumLevel = 100f;
    [SerializeField]
    private float quantumLevel;

    [Header("Movement")]
    public float maxSpeed = 3f;
    public float sprintSpeedMultiplier = 2f;
    public float deceleration = 5.0f;
    public float acceleration = 10.0f;

    [Header("Dash")]
    public float dashForce = 10.0f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 2f;

    [Header("Inventory")]
    public int offensiveSlots = 1;
    public int defensiveSlots = 1;
    public int modifierSlots = 1;

    public delegate void PlayerDeathHandler();
    public event PlayerDeathHandler OnPlayerDeath;

    public float Health
    {
        get => health;
        private set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                if (health <= 0)
                {
                    OnPlayerDeath?.Invoke();
                }
            }
        }
    }

    public float QuantumLevel
    {
        get => quantumLevel;
        private set => quantumLevel = Mathf.Clamp(value, 0, maxQuantumLevel);
    }

    private void OnEnable()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        Health = maxHealth;
        QuantumLevel = maxQuantumLevel;
    }

    public void IncreaseQuantumLevel(float amount)
    {
        QuantumLevel += amount;
    }

    public void DecreaseQuantumLevel(float amount)
    {
        QuantumLevel -= amount;
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Taking damage: " + amount);
        Health -= amount;
    }

    public void Heal(float amount)
    {
        Health += amount;
    }
}
