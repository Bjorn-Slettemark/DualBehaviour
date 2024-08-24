using UnityEngine;

public class MultiplayerTestCubeSpawn : MultiBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 100f;

    [Sync] private float someOtherValue;

    protected override void Update()
    {
        base.Update();

        if (IsOwner())
        {
            HandleInput();
            someOtherValue += Time.deltaTime;
            UpdateSyncField(nameof(someOtherValue), someOtherValue);
        }

        Debug.Log(someOtherValue);
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized * moveSpeed * Time.deltaTime;

        if (movement != Vector3.zero)
        {
            // Update position
            transform.position += movement;

            // Update rotation
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }
}