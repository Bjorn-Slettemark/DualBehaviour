//using UnityEngine;
//using UnityEngine.UIElements;

//public class GunController : MultiBehaviour
//{
//    [Sync] public Quaternion Rotation { get; set; }
//    public Transform firePoint;
//    public GameObject projectilePrefab;
//    public float fireRate = 1f;
//    public float damage = 10f;
//    public float rotationSpeed = 10f;
//    private float timeUntilFire = 0f;
//    private Camera cam;

//    protected override void OnInitialized()
//    {
//        base.OnInitialized();
//        cam = Camera.main;
//    }

//    void Update()
//    {
//        if (isLocalPlayer)
//        {
//            RotateGunTowardsMouse();
//        }
//    }

//    public void AttemptShoot()
//    {
//        if (isLocalPlayer && Time.time >= timeUntilFire)
//        {
//            timeUntilFire = Time.time + 1f / fireRate;
//            Shoot();
//        }
//    }

//    void RotateGunTowardsMouse()
//    {
//        Plane groundPlane = new Plane(Vector3.up, transform.position);
//        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
//        if (groundPlane.Raycast(ray, out float hitDist))
//        {
//            Vector3 targetPoint = ray.GetPoint(hitDist);
//            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
//            targetRotation.x = 0;
//            targetRotation.z = 0;
//            Quaternion newRot = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
//            RequestSyncedValueUpdate(nameof(Rotation), newRot);
//            transform.rotation = Rotation;
//        }
//    }

//    private void Shoot()
//    {
//        // Instantiate the projectile
//        GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

//        // Get the BulletController component
//        BulletController bulletController = projectileObject.GetComponent<BulletController>();

//        if (bulletController != null)
//        {
//            // Initialize the bullet as a MultiBehaviour
//            bulletController.Initialize(WebRTCEngine.Instance.LocalPeerId);

//            // Set initial properties
//            bulletController.SetDamage(damage);
//            // You can set other properties here if needed, like speed

//            // Notify other clients about the new projectile
//        }
//        else
//        {
//            Debug.LogError("BulletController component not found on the projectile prefab!");
//        }
//    }
//}