using Unity.VisualScripting;
using UnityEngine;

public class TankController : MultiBehaviour
{
    [SyncVar] private Vector3 syncPosition;
    [SyncVar] private Quaternion syncRotation;
    [SyncVar] private Quaternion syncTurretRotation;
    [SyncVar] private bool syncIsFiring;
    [SerializeField] private float health = 100f;

    public float moveSpeed = 5f;
    public float rotateSpeed = 100f;
    public float fireRate = 0.5f;
    public GameObject bulletPrefab;
    public GameObject firePoint;
    public GameObject turret;

    private float lastFireTime;
    private Camera mainCamera;
    private Rigidbody rb;
    private float moveInput;
    private float rotateInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        ConfigureRigidbody();

        mainCamera = Camera.main;
        EventChannelManager.Instance.SubscribeToChannel($"PlayerEventChannel", HandleLocalPlayerEvents);
    }

    private void ConfigureRigidbody()
    {
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    protected override void Update()
    {
        base.Update();
        if (IsOwner())
        {
            HandleInput();
            AimTurret();
        }
        else
        {
            // Smoothly interpolate position and rotation for non-owners
            transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, syncRotation, Time.deltaTime * 10f);
            if (turret != null)
            {
                turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, syncTurretRotation, Time.deltaTime * 10f);
            }
        }

        HandleFiring();
    }

    private void FixedUpdate()
    {
        if (IsOwner())
        {
            ApplyMovement();
            ApplyRotation();
            SyncTransform();
        }
    }

    private void HandleInput()
    {
        moveInput = Input.GetAxis("Vertical");
        rotateInput = Input.GetAxis("Horizontal");
        syncIsFiring = Input.GetButton("Fire1") && Time.time - lastFireTime >= fireRate;
    }

    private void ApplyMovement()
    {
        Vector3 movement = transform.forward * moveInput * moveSpeed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    private void ApplyRotation()
    {
        float rotation = rotateInput * rotateSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    private void SyncTransform()
    {
        syncPosition = transform.position;
        syncRotation = transform.rotation;
        if (turret != null)
        {
            syncTurretRotation = turret.transform.rotation;
        }
    }

    private void AimTurret()
    {
        if (turret != null && mainCamera != null)
        {
            // Create a plane at the tank's height
            Plane groundPlane = new Plane(Vector3.up, turret.transform.position);

            // Cast a ray from the mouse position into the world
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // Check if the ray intersects with the ground plane
            if (groundPlane.Raycast(ray, out float enterDistance))
            {
                // Get the world position where the ray intersects the plane
                Vector3 hitPoint = ray.GetPoint(enterDistance);

                // Calculate the direction from the turret to the hit point
                Vector3 aimDirection = (hitPoint - turret.transform.position).normalized;

                // Rotate the turret to face the aim direction
                turret.transform.rotation = Quaternion.LookRotation(aimDirection, Vector3.up);
            }
        }
    }
    private void HandleFiring()
    {
        if (syncIsFiring && IsOwner())
        {
            Fire();
            syncIsFiring = false;
            lastFireTime = Time.time;
        }
    }
    private void Fire()
    {
        if (firePoint != null)
        {
            Vector3 fireDir = turret.transform.forward;
            Vector3 firePosition = firePoint.transform.position;
            MultiplayerManager.Instance.RequestObjectSpawn("Bullet", firePosition, fireDir);
        }
        else
        {
            Debug.LogWarning("TankController: Unable to fire - firePoint is null");
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsOwner())
        {
            health -= damage;
            Debug.Log($"Tank took {damage} damage. Remaining health: {health}");
            if (health <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        MultiplayerManager.Instance.RequestDestroyGameObject(ObjectId);
    }

    private void HandleLocalPlayerEvents(string eventData)
    {
        string[] parts = NetworkUtility.SplitEventData(eventData);
        if (parts[0] != "DealDamage") return;
        if (parts != null && float.TryParse(parts[2], out float damageAmount))
        {
            if (parts[1] == LocalWebRTCEngine.Instance.LocalPeerId)
            {
                TakeDamage(damageAmount);
            }
        }
    }
}