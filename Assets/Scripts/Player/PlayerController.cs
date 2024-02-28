using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private CharacterController characterController;

    public GunController gunController; // Reference to the GunController component

    private Vector3 movement;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
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

}
