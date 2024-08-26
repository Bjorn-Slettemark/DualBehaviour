using UnityEngine;

public class MultiplayerTestCubeSpawn : MultiBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 100f;
    public float turretRotateSpeed = 180f;
    [SerializeField] private Transform turretTransform;

    [Sync] private float someOtherValue;
    [Sync] private Vector3 synchedPosition;
    [Sync] private Quaternion synchedRotation;
    [Sync] private Quaternion synchedTurretRotation;

    [SerializeField] private float syncRate = 30f; // Sync rate in Hz
    private float syncInterval;
    private float lastSyncTime = 0f;
    private Camera mainCamera;
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetSyncLink(nameof(synchedPosition),
            () => transform.position,
            value => transform.position = (Vector3)value);
        SetSyncLink(nameof(synchedRotation),
            () => transform.rotation,
            value => transform.rotation = (Quaternion)value);
        SetSyncLink(nameof(synchedTurretRotation),
            () => turretTransform.rotation, // Use world rotation
            value => turretTransform.rotation = (Quaternion)value); // Set world rotation

        // Initialize synced variables with current transform values
        synchedPosition = transform.position;
        synchedRotation = transform.rotation;
        synchedTurretRotation = turretTransform.rotation; // Use world rotation

        mainCamera = Camera.main;

        syncInterval = 1f / syncRate;

    }

    private void Update()
    {
        if (IsOwner())
        {
            HandleTankInput();
            HandleTurretRotation();
            SyncPosition();
        }
    }

    private void HandleTankInput()
    {
        // Handle rotation
        float rotation = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;
        if (rotation != 0)
        {
            Quaternion deltaRotation = Quaternion.Euler(0f, rotation, 0f);
            UpdateSyncField(nameof(synchedRotation), synchedRotation * deltaRotation);
        }

        // Handle movement
        float movement = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        if (movement != 0)
        {
            Vector3 moveDirection = synchedRotation * Vector3.forward;
            Vector3 newPosition = synchedPosition + moveDirection * movement;
            UpdateSyncField(nameof(synchedPosition), newPosition);
        }
    }

    private void HandleTurretRotation()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 pointToLook = ray.GetPoint(rayDistance);
            Vector3 directionToPoint = (pointToLook - turretTransform.position).normalized;
            directionToPoint.y = 0; // Keep the turret rotation only on the Y-axis
            Quaternion targetRotation = Quaternion.LookRotation(directionToPoint);

            // Use Quaternion.RotateTowards for smooth rotation in world space
            Quaternion newTurretRotation = Quaternion.RotateTowards(
                turretTransform.rotation,
                targetRotation,
                turretRotateSpeed * Time.deltaTime
            );
            UpdateSyncField(nameof(synchedTurretRotation), newTurretRotation);
        }
    }

    private void SyncPosition()
    {
        if (Time.time - lastSyncTime >= syncInterval)
        {
            lastSyncTime = Time.time;
            NetworkMessage message = NetworkMessageFactory.CreatePlayerObjectMessage(
                ObjectId,
                synchedPosition,
                synchedRotation,
                synchedTurretRotation,
                "" // Additional data if needed
            );
            NetworkEngine.Instance.BroadcastEventToAllPeers(message);
        }
    }

    public override void ReceiveSyncUpdate(NetworkMessage message)
    {
        Vector3? position = message.GetData<Vector3?>("Position");
        if (position.HasValue)
            UpdateSyncField(nameof(synchedPosition), position.Value);

        Quaternion? rotation = message.GetData<Quaternion?>("Rotation");
        if (rotation.HasValue)
            UpdateSyncField(nameof(synchedRotation), rotation.Value);

        Quaternion? turretRotation = message.GetData<Quaternion?>("TurretRotation");
        if (turretRotation.HasValue)
            UpdateSyncField(nameof(synchedTurretRotation), turretRotation.Value);
    }
}