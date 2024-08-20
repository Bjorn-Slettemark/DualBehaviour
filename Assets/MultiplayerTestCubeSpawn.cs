using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Globalization;


public class MultiplayerTestCubeSpawn : MultiBehaviour
{
    [Sync] public Vector3 Position { get; private set; }
    [Sync] public Quaternion Rotation { get; private set; }
    public string PeerId { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Position = transform.position;
        Rotation = transform.rotation;
    }

 

}