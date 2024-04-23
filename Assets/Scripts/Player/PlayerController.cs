using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private CharacterController characterController;

    public GunController gunController; // Reference to the GunController component

    private Vector3 movement;
    private bool positionLoaded = false;

    [SerializeField]
    private GameEventChannelSO saveEventChannel;
    [SerializeField]
    private GameEventChannelSO loadEventChannel;
    [SerializeField]
    private PlayerDataSO playerData;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Subscribe to the event when game data is loaded. Adjust "GameEventChannel" and "GameDataLoaded" as needed.
        if (EventChannelManager.Instance != null)
        {
            EventChannelManager.Instance.RegisterEvent(gameObject, saveEventChannel, "PlayerSave", SavePlayerData);

            EventChannelManager.Instance.RegisterEvent(gameObject, loadEventChannel, "PlayerData", UpdatePlayerPositionFromEventData);
        }
    }

    void Update()
    {

        // Check if position has been loaded, to skip one update cycle
        if (positionLoaded)
        {
            positionLoaded = false;
            return;
        }

        //playerData.playerPosition = this.transform.position;

        // Input handling for movement in 3D
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");

        if (Input.GetButton("Fire1") && Time.timeScale != 0)
        {
            gunController.AttemptShoot();
        }

        // Apply gravity manually
        if (!characterController.isGrounded)
        {
            movement.y += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            movement.y = 0f; // Reset the Y movement when grounded to prevent accumulating gravity
        }

        // Move the player using the CharacterController
        characterController.Move(movement * moveSpeed * Time.deltaTime);

        // Handle rotation to face the direction of movement
        HandleRotation();
    }

    void SavePlayerData(string eventName)
    {
        Debug.Log(eventName + "!!!!");
        playerData.playerPosition = this.transform.position;
        SaveLoadManager.Instance.SaveGame(playerData);
    }
    // This method will be called when the event is raised
    void UpdatePlayerPositionFromEventData(string gameDataName)
    {
        positionLoaded = false;

        // Assuming gameDataName can be used to identify or retrieve the relevant PlayerData
        // This part needs customization based on how your GameDataSOs are structured and how PlayerData is retrieved
        Debug.Log("LOAD!!" + playerData.playerPosition);
        ;

        if (playerData != null)
        {
            //SaveLoadManager.Instance.(playerData)
            LoadPosition(playerData.playerPosition);
        }
    }
    void HandleRotation()
    {
        Vector3 direction = new Vector3(movement.x, 0, movement.z);
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime * 100);
        }
    }

    // Method to apply force to the player
    public void ApplyForce(Vector3 force)
    {
        characterController.Move(force * Time.deltaTime);
    }
    public void LoadPosition(Vector3 loadedPosition)
    {
        characterController.enabled = false; // Disable to set position outside bounds
        transform.position = loadedPosition;
        characterController.enabled = true; // Re-enable
        positionLoaded = true;
    }
}
