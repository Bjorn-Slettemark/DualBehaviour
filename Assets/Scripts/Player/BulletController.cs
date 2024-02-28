using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 10f; // The damage this bullet will deal
    public float maxRange = 10f; // Maximum distance the bullet can travel

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // Store the starting position of the bullet
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime); // Use Vector3 for 3D movement

        // Check if the bullet has exceeded its maximum range
        if (Vector3.Distance(startPosition, transform.position) > maxRange)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider hitInfo)
    {
        // Check if the hit object has a HealthSystem component
        AISenseSystem aISense = hitInfo.GetComponent<AISenseSystem>();

        HealthSystem healthSystem = hitInfo.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            Debug.Log("hithithit");
            // Damage the hit object
            healthSystem.TakeDamage(damage);
        }
        if (aISense != null)
        {
            // Damage the hit object
            aISense.OnHitReceived?.Invoke();
        }
        // Destroy the bullet after hitting something
        Destroy(gameObject);
    }


    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
