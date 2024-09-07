    //using UnityEngine;
    //using System.Collections;

    //public class TankController : MultiBehaviour
    //{
    //    [Sync] public Vector3 Position { get; set; }
    //    [Sync] public Vector3 BodyRotation { get; set; }
    //    [Sync] public Vector3 TurretRotation { get; set; }

    //    public float moveSpeed = 5f;
    //    public float rotationSpeed = 100f;
    //    public float turretRotationSpeed = 2f;
    //    public GameObject bulletPrefab;
    //    public Transform bulletSpawnPoint;

    //    public Vector3 startPos;

    //    [SerializeField] private Transform turretTransform;

    //    protected override void Awake()
    //    {
    //        base.Awake();
    //        startPos = transform.position;
    //        FindTurretTransform();
    //    }

    //    private void FindTurretTransform()
    //    {
    //        // First, try to find the Turret as a direct child
    //        turretTransform = transform.Find("Turret");
    //        turretTransform = transform.Find("Gunpoint");

    //        // If not found, search recursively
    //        if (turretTransform == null)
    //        {
    //            turretTransform = GetComponentInChildren<Transform>(true).Find("Turret");
    //        }

    //        // If still not found, log detailed information
    //        if (turretTransform == null)
    //        {
    //            Debug.LogError($"Turret transform not found on {gameObject.name}. Hierarchy:");
    //            LogHierarchy(transform);
    //        }
    //        else 
    //        {
    //            Debug.Log($"Turret found: {turretTransform.name}");
    //        }
    //    }

    //    private void LogHierarchy(Transform parent, string indent = "")
    //    {
    //        Debug.Log($"{indent}{parent.name}");
    //        foreach (Transform child in parent)
    //        {
    //            LogHierarchy(child, indent + "  ");
    //        }
    //    }

    //    protected override void OnInitialized()
    //    {
    //        base.OnInitialized();
    //        Position = startPos;
    //        BodyRotation = transform.eulerAngles;
    //        if (turretTransform != null)
    //        {
    //            TurretRotation = turretTransform.localEulerAngles;
    //        }
    //        else
    //        {
    //            Debug.LogWarning("TurretRotation not initialized due to missing Turret transform.");
    //        }
    //    }

    //    private void Update()
    //    {
    //        if (isLocalPlayer)
    //        {
    //            HandleInput();
    //        }

    //        ApplyTransforms();
    //    }

    //    private void HandleInput()
    //    {
    //        // Movement
    //        float moveVertical = Input.GetAxis("Vertical");
    //        Vector3 movement = transform.forward * moveVertical * moveSpeed * Time.deltaTime;
    //        Vector3 newPosition = Position + movement;
    //        RequestSyncedValueUpdate(nameof(Position), newPosition);

    //        // Body Rotation
    //        float rotateHorizontal = Input.GetAxis("Horizontal");
    //        Vector3 newBodyRotation = BodyRotation + new Vector3(0, rotateHorizontal * rotationSpeed * Time.deltaTime, 0);
    //        RequestSyncedValueUpdate(nameof(BodyRotation), newBodyRotation);

    //        // Turret Rotation
    //        if (turretTransform != null)
    //        {
    //            float turretRotateHorizontal = Input.GetAxis("Mouse X");
    //            Vector3 newTurretRotation = TurretRotation + new Vector3(0, turretRotateHorizontal * turretRotationSpeed, 0);
    //            RequestSyncedValueUpdate(nameof(TurretRotation), newTurretRotation);
    //        }

    //        // Shooting
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            Shoot();
    //        }
    //    }

    //    private void ApplyTransforms()
    //    {
    //        transform.position = Position;
    //        transform.eulerAngles = BodyRotation;
    //        if (turretTransform != null)
    //        {
    //            turretTransform.localEulerAngles = TurretRotation;
    //        }
    //    }

    //    private void Shoot()
    //    {
    //        if (bulletSpawnPoint == null)
    //        {
    //            Debug.LogError("Bullet spawn point is not set.");
    //            return;
    //        }

    //        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    //        BulletController bulletController = bullet.GetComponent<BulletController>(); 

    //        MultiBehaviour bulletMB = bulletController.GetComponent<MultiBehaviour>();
    //        if (bulletMB != null)
    //        {
    //            bulletMB.Initialize(WebRTCEngine.Instance.LocalPeerId);
    //        }
    //        else
    //        {
    //            Debug.LogError("Bullet prefab does not have a MultiBehaviour component.");
    //        }
    //    }

    //    public override void Initialize(string ownerId, string objectId = null)
    //    {
    //        base.Initialize(ownerId, objectId);
    //        if (ownerId == WebRTCEngine.Instance.LocalPeerId)
    //        {
    //            isLocalPlayer = true;
    //        }
    //    }
    //}