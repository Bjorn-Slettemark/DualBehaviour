using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Game/Player Data", order = 1)]
public class PlayerDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public GameObject playerPrefab;
}
