using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Game/Player Data", order = 1)]
public class PlayerDataSO : GameDataSO
{
    [SerializeField]
    public GameObject playerPrefab;
    [SerializeField]
    public Vector3 playerPosition;
    public string dataName;
}
