using UnityEngine;

[CreateAssetMenu(fileName = "IngameGameLevel", menuName = "GameLevel/IngameGameLevel", order = 1)]
public class IngameGameLevelSO : GameLevelSO
{
    [SerializeField]
    private PlayerDataSO playerData;
    public override void EnterLevel()
    {
        base.EnterLevel();
        Debug.Log("Entering level, spawning player");
        Instantiate(playerData.playerPrefab, Vector3.zero, Quaternion.identity);
    }
    public override void LevelUpdate()
    {
    }
}
