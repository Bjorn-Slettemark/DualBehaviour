using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    public float damage = 10f;
    public float rotationSpeed = 10f;

    private float timeUntilFire = 0f;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
       RotateGunTowardsMouse();
    }

    public void AttemptShoot()
    {
        // Check if the current time is greater than or equal to the time until the next shot is allowed
        if (Time.time >= timeUntilFire)
        {
            // Update timeUntilFire to the current time plus the interval derived from fireRate
            timeUntilFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }


    void RotateGunTowardsMouse()
    {
        // Create a plane at the gun's position facing up
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        // Generate a ray from the cursor position
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects the plane
        float hitDist;
        if (groundPlane.Raycast(ray, out hitDist))
        {
            // Find the point along the ray that hits the calculated distance
            Vector3 targetPoint = ray.GetPoint(hitDist);

            // Determine the target rotation. This is the rotation if the gun looks at the target point
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

            // Set the gun's rotation to this new rotation but only rotate around Y axis
            targetRotation.x = 0;
            targetRotation.z = 0;

            // Smoothly rotate towards the target point
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
}
