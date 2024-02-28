using UnityEngine;

public class SwordController : MonoBehaviour
{
    public Transform attackPoint; // Point from where the sword will attack
    public float swingSpeed = -1000f;
    public float damage = 20f;
    public float knockbackForce = 100f;
    public float attackRange = 2f;

    private bool isSwinging = false;

    void Update()
    {
        if (isSwinging)
        {
            // Perform swinging motion
            transform.RotateAround(attackPoint.position, Vector3.up, swingSpeed * Time.deltaTime);
        }
    }

    public void StartSwinging()
    {
        isSwinging = true;
        Invoke("StopSwinging", 1.0f); // Swing for 1 second
    }

    private void StopSwinging()
    {
        isSwinging = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Damage the player
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Knock back the player if it has a PlayerController component
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                playerController.ApplyForce(knockbackDirection * knockbackForce);
            }
        }
    }

}
