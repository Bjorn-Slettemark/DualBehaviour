//using UnityEngine;

//public class BulletController : MultiBehaviour
//{
//    public float speed = 20f;
//    public float damage = 10f;
//    public float maxRange = 10f;
//    private Vector3 startPosition;

//    [Sync] public Vector3 Position { get; set; }
//    [Sync] public Quaternion Rotation { get; set; }

//    protected override void OnInitialized()
//    {
//        base.OnInitialized();
//        startPosition = transform.position;
//        Position = startPosition;
//        Rotation = transform.rotation;
//    }

//    void Update()
//    {
//        if (isLocalPlayer)
//        {
//            Vector3 newPosition = Position + transform.right * speed * Time.deltaTime;
//            RequestSyncedValueUpdate(nameof(Position), newPosition);

//            if (Vector3.Distance(startPosition, newPosition) > maxRange)
//            {
//                // Request destruction of the bullet
//                Destroy(this.gameObject);
//            }
//        }

//        // Apply the synced position and rotation
//        transform.position = Position;
//        transform.rotation = Rotation;
//    }

//    void OnTriggerEnter(Collider hitInfo)
//    {
//        if (isLocalPlayer)
//        {
//            AISenseSystem aISense = hitInfo.GetComponent<AISenseSystem>();
//            HealthSystem healthSystem = hitInfo.GetComponent<HealthSystem>();

//            if (healthSystem != null)
//            {
//                healthSystem.TakeDamage(damage);
//            }

//            if (aISense != null)
//            {
//                aISense.OnHitReceived?.Invoke();
//            }

//            // Request destruction of the bullet
//            Destroy(this.gameObject);
//                }
//    }

//    public void SetDamage(float newDamage)
//    {
//        damage = newDamage;
//    }

//    public void SetSpeed(float newSpeed)
//    {
//        speed = newSpeed;
//    }
//}