//using UnityEngine;
//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Linq;
//using System.Globalization;


//public class PlayerCube : MultiBehaviour
//{
//    [Sync] public Vector3 Position { get;  set; }
//    [Sync] public Quaternion Rotation { get;  set; }
//    public string PeerId { get; private set; }
//    public Vector3 spawnPos;

//    protected override void Awake()
//    {
//        base.Awake();
//        spawnPos = transform.position;
//    }

//    protected override void OnInitialized()
//    {
//        base.OnInitialized();


//        Position = spawnPos;
 


//    }
//    private void Update()
//    {


//        if (isLocalPlayer)
//        {
//            HandleInput();
//        }

//            transform.position = Position;
//            transform.rotation = Rotation;


        
//    }

//    private void HandleInput()
//    {
//        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
//        if (movement != Vector3.zero)
//        {
//            Vector3 newPosition = Position + movement * Time.deltaTime * 5f;
//            RequestSyncedValueUpdate(nameof(Position), newPosition);
//        }

//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            Quaternion newRotation = Rotation * Quaternion.Euler(0, 90, 0);
//            RequestSyncedValueUpdate(nameof(Rotation), newRotation);
//        }
//    }
//}