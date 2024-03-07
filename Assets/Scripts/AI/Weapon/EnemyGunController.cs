using UnityEngine;

public class EnemyGunController : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    public AISenseSystem aiSense; // Reference to the AISense component
    public float fireRate = 1f;
    public float rotationSpeed = 10f;
    public float damage = 10f; // Damage output of the bullet

    private float timeUntilFire = 0f;

    private void Start()
    {
        // Get reference to the AISense component attached to the same GameObject
        aiSense = gameObject.GetComponentInParent<AISenseSystem>();
    }

    void Update()
    {
        if (aiSense != null) // Check if the player is visible to the AI
        {
            AimTowardsPlayer();
        }
    }

    void AimTowardsPlayer()
    {
        if (aiSense.player != null) // Ensure player reference is not null
        {
            Vector3 directionToPlayer = aiSense.player.position - transform.position;
            directionToPlayer.y = 0; // Remove vertical component to ensure we're only rotating on the Y axis

            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }


    public void AttemptShoot()
    {
        // Check if it's time to fire based on the fire rate
        if (Time.time >= timeUntilFire)
        {
            timeUntilFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (firePoint != null && projectilePrefab != null)
        {
            // Instantiate a projectile at the fire point
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // Set the bullet's damage
            BulletController bulletController = bullet.GetComponent<BulletController>();
            if (bulletController != null)
            {
                bulletController.SetDamage(damage); // Set the bullet's damage
            }
        }
    }
}
