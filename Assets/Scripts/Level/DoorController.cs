using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float slideDistance = 5f; // How far the door slides down
    public float slideSpeed = 3f; // Speed of the sliding motion
    public float openDuration = 5f; // Time in seconds the door stays open before closing

    private Vector3 closedPosition; // Starting position of the door, assumed to be 'closed'
    private Vector3 openPosition; // Target position when 'open'
    private bool isOpening = false; // Is the door currently opening?
    private bool isClosing = false; // Is the door currently closing?
    private float openTimer = 0f; // Timer to track how long the door stays open

    void Start()
    {
        closedPosition = transform.position; // Initialize closedPosition to the starting position
        openPosition = new Vector3(transform.position.x, transform.position.y - slideDistance, transform.position.z); // Calculate the open position
    }

    void Update()
    {
        if (isOpening)
        {
            // Smoothly translate the door to the open position
            transform.position = Vector3.MoveTowards(transform.position, openPosition, slideSpeed * Time.deltaTime);
            if (transform.position == openPosition)
            {
                isOpening = false; // Stop opening when the door reaches the open position
                openTimer = openDuration; // Reset and start the open timer
            }
        }
        else if (isClosing)
        {
            // Smoothly translate the door to the closed position
            transform.position = Vector3.MoveTowards(transform.position, closedPosition, slideSpeed * Time.deltaTime);
            if (transform.position == closedPosition)
            {
                isClosing = false; // Stop closing when the door reaches the closed position
            }
        }

        // Timer countdown
        if (openTimer > 0)
        {
            openTimer -= Time.deltaTime;
            if (openTimer <= 0)
            {
                // Time's up, start closing the door
                isClosing = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpening && !isClosing)
        {
            isOpening = true; // Trigger the door to start opening
            // No need to change the openTimer here as it's reset when the door fully opens
        }
    }
}
