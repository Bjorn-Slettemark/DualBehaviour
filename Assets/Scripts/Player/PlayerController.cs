//using UnityEngine;

//public class PlayerController : MultiBehaviour
//{
//    public float moveSpeed = 5f;
//    public float rotationSpeed = 10f;

//    private CharacterController characterController;

//    public GunController gunControllerRef; // Reference to the GunController component
//    [SerializeField] public GameObject gunAttachment;

//    [Sync] public Vector3 Position { get; set; }
//    [Sync] public Quaternion Rotation { get; set; }
//    public Vector3 spawnPos;
//    public Quaternion spawnRot;
//    private Vector3 movement;

//    [SerializeField]
//    private GameEventChannelSO saveEventChannel;
//    [SerializeField]
//    private GameEventChannelSO loadEventChannel;
//    [SerializeField]
//    private PlayerDataSO playerData;

//    protected override void Awake()
//    {
//        base.Awake();
//        characterController = GetComponent<CharacterController>();
//        spawnPos = transform.position;
//        spawnRot = transform.rotation;
//    }

//    protected override void OnInitialized()
//    {
//        base.OnInitialized();



//        if (isLocalPlayer)
//        {
//            // Subscribe to event channels
//            if (EventChannelManager.Instance != null)
//            {
//                EventChannelManager.Instance.SubscribeEvent(gameObject, saveEventChannel, "PlayerSave", SavePlayerData);
//                EventChannelManager.Instance.SubscribeEvent(gameObject, loadEventChannel, "PlayerData", UpdatePlayerPositionFromEventData);
//            }


//        }

//        // Spawn and initialize GunController
//        SpawnGunController();

//        Position = spawnPos;
//        Rotation = spawnRot;
//    }

//    void Update()
//    {
//        if (isLocalPlayer)
//        {
//            HandleInput();
//        }

//        ApplyMovement();
//    }

//    void HandleInput()
//    {
//        movement.x = Input.GetAxis("Horizontal");
//        movement.z = Input.GetAxis("Vertical");

//        if (Input.GetButton("Fire1") && Time.timeScale != 0 && gunControllerRef != null)
//        {
//            gunControllerRef.AttemptShoot();
//        }

//        // Apply gravity manually
//        if (!characterController.isGrounded)
//        {
//            movement.y += Physics.gravity.y * Time.deltaTime;
//        }
//        else
//        {
//            movement.y = 0f;
//        }

//        Vector3 newPosition = Position + transform.TransformDirection(movement) * moveSpeed * Time.deltaTime;
//        RequestSyncedValueUpdate(nameof(Position), newPosition);

//        // Handle rotation
//        if (movement != Vector3.zero)
//        {
//            Quaternion newRotation = Quaternion.LookRotation(movement, Vector3.up);
//            RequestSyncedValueUpdate(nameof(Rotation), newRotation);
//        }
//    }

//    void ApplyMovement()
//    {
//        transform.position = Position;
//        transform.rotation = Rotation;

//        // Use CharacterController for collision detection
//        characterController.Move(movement * moveSpeed * Time.deltaTime);
//    }

//    void SpawnGunController()
//    {
//        if (gunControllerRef == null)
//        {
//            GameObject gunController = Resources.Load<GameObject>("GunController");

//            Debug.Log("Spawning " + gunController.name);
//            GameObject gun = Instantiate(gunController, gunAttachment.transform.position, Quaternion.identity, gunAttachment.transform);
//            //playerObject.name = playerPrefabName;
//            MultiBehaviour multiBehaviour = gun.GetComponent<MultiBehaviour>();
//            if (multiBehaviour != null)
//            {
//                multiBehaviour.Initialize(WebRTCEngine.Instance.LocalPeerId);
//            }
//            gunControllerRef = gun.GetComponent<GunController>();
//        }
//    }

//    void SavePlayerData(string eventName)
//    {
//        Debug.Log(eventName + "!!!!");
//        playerData.playerPosition = Position;
//        SaveLoadManager.Instance.SaveGame(playerData);
//    }

//    void UpdatePlayerPositionFromEventData(string gameDataName)
//    {
//        Debug.Log("LOAD!!" + playerData.playerPosition);

//        if (playerData != null)
//        {
//            LoadPosition(playerData.playerPosition);
//        }
//    }

//    public void ApplyForce(Vector3 force)
//    {
//        Vector3 newPosition = Position + force * Time.deltaTime;
//        RequestSyncedValueUpdate(nameof(Position), newPosition);
//    }

//    public void LoadPosition(Vector3 loadedPosition)
//    {
//        characterController.enabled = false;
//        RequestSyncedValueUpdate(nameof(Position), loadedPosition);
//        characterController.enabled = true;
//    }
//}