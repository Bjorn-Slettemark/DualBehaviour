using UnityEngine;

public class Bullet : MultiBehaviour
{
    [SyncVar] private Vector3 syncedPosition;
    public float speed = 20f;
    public float lifetime = 3f;
    public float damage = 10f;
    private Vector3 direction;
    private float spawnTime;
    public string shooterPeerId;

    private void Start()
    {
        syncedPosition = transform.position;
        direction = transform.forward;
        spawnTime = Time.time;
        shooterPeerId = OwnerPeerId;    
    }

    protected override void Update()
    {
        base.Update();
        if (IsOwner())
        {
            syncedPosition += direction * speed * Time.deltaTime;
            transform.position = syncedPosition;
            if (Time.time - spawnTime > lifetime)
            {
                DestroyBullet();
            }
        }
        else
        {
            transform.position = syncedPosition;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Environment")) 
        {
            DestroyBullet();
        }
        
        if (IsOwner())
        {
            return;
        }

        if (!NetworkEngine.Instance.IsLocalPeerId(collision.gameObject.GetComponent<MultiBehaviour>()?.OwnerPeerId))
        {
            return;
        }

        if (collision.gameObject.tag == "Player")
        {
            HandlePlayerCollision(collision);
        }

        DestroyBullet();
    }

    private void HandlePlayerCollision(Collision collision)
    {
        Debug.Log("Hit player!");
        TankController hitTank = collision.gameObject.GetComponent<TankController>();
        if (hitTank != null && hitTank.OwnerPeerId != shooterPeerId)
        {
            hitTank.TakeDamage(damage);
        }
    }

    private void DestroyBullet()
    {
        MultiplayerManager.Instance.RequestDestroyGameObject(ObjectId);
    }
}