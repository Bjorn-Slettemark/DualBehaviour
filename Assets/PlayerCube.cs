using UnityEngine;
using System.Collections;

public class PlayerCube : MultiBehaviour
{
    [Sync] public float moveSpeed = 5f;
    [Sync] public float jumpForce = 5f;
    [SerializeField] public string PeerId;
    [SerializeField] public bool IsLocalPlayer;

    private Rigidbody rb;
    private bool isGrounded = true;
    private Vector3 movement;
    private bool isInitialized = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(string assignedPeerId)
    {
        if (isInitialized)
        {
            Debug.LogWarning($"PlayerCube already initialized with PeerId: {PeerId}");
            return;
        }

        PeerId = assignedPeerId;
        base.SetOwnerId(PeerId);

        IsLocalPlayer = MultiplayerManager.Instance.IsLocalPeer(PeerId);

        MultiplayerManager.Instance.RegisterForPeerEvents(PeerId, HandlePeerEvent);
        Debug.Log($"[{PeerId}] Registered for peer events");

        MultiplayerManager.Instance.RegisterForMultiplayerEvents(HandleMultiplayerEvent);

        isInitialized = true;
        Debug.Log($"Player cube initialized with PeerId: {PeerId}, isLocal: {IsLocalPlayer}");

        StartCoroutine(VerifySetupCoroutine());
    }

    private IEnumerator VerifySetupCoroutine()
    {
        yield return new WaitForSeconds(1f);
        VerifySetup();
    }

    private void VerifySetup()
    {
        Debug.Log($"[{PeerId}] Verify Setup - IsInitialized: {isInitialized}, IsLocalPlayer: {IsLocalPlayer}, Position: {transform.position}, Rigidbody null: {rb == null}");
    }

    private void FixedUpdate()
    {
        if (!isInitialized) return;
        ApplyMovement();
    }

    public void ProcessInput(string inputEvent)
    {

        Debug.Log($"[{PeerId}] Processing input: {inputEvent}");
        MultiplayerManager.Instance.BroadcastEventToAllPeers(inputEvent);
        HandlePeerEvent(inputEvent);
    }

    private void HandlePeerEvent(string eventName)
    {
        Debug.Log($"[{PeerId}] Handling event: {eventName}");

        switch (eventName)
        {
            case "UpPressed": UpdateMovement(Vector3.forward); break;
            case "UpReleased": UpdateMovement(-Vector3.forward); break;
            case "DownPressed": UpdateMovement(Vector3.back); break;
            case "DownReleased": UpdateMovement(-Vector3.back); break;
            case "LeftPressed": UpdateMovement(Vector3.left); break;
            case "LeftReleased": UpdateMovement(-Vector3.left); break;
            case "RightPressed": UpdateMovement(Vector3.right); break;
            case "RightReleased": UpdateMovement(-Vector3.right); break;
            case "JumpPressed": Jump(); break;
            case "JumpReleased": CancelJump(); break;
        }
    }

    private void HandleMultiplayerEvent(string eventName)
    {
        Debug.Log($"[{PeerId}] Received multiplayer event: {eventName}");
    }

    private void UpdateMovement(Vector3 direction)
    {
        movement += direction;
        Debug.Log($"[{PeerId}] Updated movement: {movement}");
    }

    private void ApplyMovement()
    {
        if (movement != Vector3.zero)
        {
            Vector3 movePosition = transform.position + movement.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(movePosition);
            //Debug.Log($"[{PeerId}] Applied movement. New position: {movePosition}");
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            Debug.Log($"[{PeerId}] Jump executed");
        }
    }

    private void CancelJump()
    {
        // Optionally implement logic to cancel or modify jump if released early
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log($"[{PeerId}] Grounded");
        }
    }

    private void OnDestroy()
    {
        if (isInitialized)
        {
            MultiplayerManager.Instance.UnregisterFromPeerEvents(PeerId, HandlePeerEvent);
            MultiplayerManager.Instance.UnregisterFromMultiplayerEvents(HandleMultiplayerEvent);
        }
    }
}