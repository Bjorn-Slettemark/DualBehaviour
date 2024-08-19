using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Globalization;


public class PlayerCube : MultiBehaviour
{
    [Sync] public Vector3 Position { get; private set; }
    [Sync] public Quaternion Rotation { get; private set; }
    public string PeerId { get; private set; }
    public bool IsLocalPlayer { get; private set; }

    public void Initialize(string peerId, bool isLocal)
    {
        PeerId = peerId;
        IsLocalPlayer = isLocal;
        Position = transform.position;
        Rotation = transform.rotation;
        SetObjectId(peerId); // Set the ObjectId to be the same as the PeerId
        Debug.Log($"PlayerCube initialized: PeerId={PeerId}, IsLocal={IsLocalPlayer}, ObjectId={ObjectId}");
    }

    private void Update()
    {
        if (IsLocalPlayer)
        {
            HandleInput();
        }

        // Apply synced values
        transform.position = Position;
        transform.rotation = Rotation;
    }

    private void HandleInput()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (movement != Vector3.zero)
        {
            Vector3 newPosition = Position + movement * Time.deltaTime * 5f;
            RequestSyncedValueUpdate(nameof(Position), newPosition);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Quaternion newRotation = Rotation * Quaternion.Euler(0, 90, 0);
            RequestSyncedValueUpdate(nameof(Rotation), newRotation);
        }
    }
}