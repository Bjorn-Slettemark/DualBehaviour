using UnityEngine;

[CreateAssetMenu(fileName = "IngameGameLevel", menuName = "GameLevel/IngameGameLevel", order = 1)]
public class IngameGameLevelSO : GameLevelSO
{
    [SerializeField]
    private PlayerDataSO playerData;
    public override void EnterLevel()
    {
        base.EnterLevel();
        Debug.Log("Entering level" + gameLevelName + " , spawning player");
        EventChannelManager.Instance.RaiseEvent("GameStateEventChannel", "SpawnPlayer");
    }
    public override void LevelUpdate()
    {
    }


}
