//using UnityEngine;

//public class BulletController : MultiBehaviour
//{
//    public float speed = 20f;
//    public float damage = 10f;
//    public float maxRange = 10f;
//    private Vector3 startPosition;
//    [Sync] private Vector3 synchedPosition;
//    [Sync] private Vector3 synchedVelocity;
//    private bool isDestroyed = false;

//    private void Start()
//    {
//        startPosition = transform.position;
//        synchedPosition = startPosition;
//        synchedVelocity = transform.forward * speed;

//        SetSyncLink(nameof(synchedPosition),
//            () => synchedPosition,
//            value => synchedPosition = (Vector3)value);
//    }
//    protected override void OnInitialized()
//    {
//        base.OnInitialized();
 

//    }

//    void Update()
//    {
//        if (isDestroyed) return;

//        if (IsOwner())
//        {
//            MoveBullet();
//            CheckRange();
//            SyncPosition();
//        }
//        else
//        {
//            // Non-owner clients interpolate based on synced position and velocity
//            synchedPosition += synchedVelocity * Time.deltaTime;
//        }

//        // Update the actual position for all clients
//        transform.position = synchedPosition;
//    }

//    private void MoveBullet()
//    {
//        synchedPosition += synchedVelocity * Time.deltaTime;
//        //transform.forward = synchedVelocity.normalized;
//    }

//    private void CheckRange()
//    {
//        if (Vector3.Distance(startPosition, synchedPosition) > maxRange)
//        {
//            DestroyBullet();
//        }
//    }

//    private void SyncPosition()
//    {
//        NetworkMessage message = NetworkMessageFactory.CreatePlayerObjectMessage(
//            ObjectId,
//            synchedPosition,
//            Quaternion.LookRotation(synchedVelocity),
//            Quaternion.identity,
//            isDestroyed.ToString()
//        );
//        NetworkEngine.Instance.BroadcastEventToAllPeers(message);
//    }

//    void OnTriggerEnter(Collider hitInfo)
//    {
//        if (!IsOwner() && !isDestroyed)
//        {
//            HealthSystem healthSystem = hitInfo.GetComponent<HealthSystem>();
//            if (healthSystem != null)
//            {
//                healthSystem.TakeDamage(damage);
//            }
//            AISenseSystem aiSense = hitInfo.GetComponent<AISenseSystem>();
//            if (aiSense != null)
//            {
//                aiSense.OnHitReceived?.Invoke();
//            }
//            DestroyBullet();
//        }
//    }

//    private void DestroyBullet()
//    {
//        if (isDestroyed) return;
//        isDestroyed = true;
//        NetworkMessage destroyMessage = NetworkMessageFactory.CreateDestroyObjectMessage(ObjectId);
//        NetworkEngine.Instance.BroadcastEventToAllPeers(destroyMessage);
//        // You might want to add a delay before actually destroying the object
//        // to ensure the message is sent
//        Destroy(gameObject, 0.1f);
//    }

//    public override void ReceiveSyncUpdate(NetworkMessage message)
//    {
//        Vector3? position = message.GetData<Vector3?>("Position");
//        if (position.HasValue)
//            UpdateSyncField(nameof(synchedPosition), position.Value);

//        Quaternion? rotation = message.GetData<Quaternion?>("Rotation");
//        if (rotation.HasValue)
//            synchedVelocity = rotation.Value * Vector3.forward * speed;

//    }
//}