using UnityEngine;

public class LevelChangeButton : MonoBehaviour
{

    private GameLevelSO desiredLevelSO;
    public void ChangeLevel(GameLevelSO eventLevelSO)
    {

        //LevelLoaderManager.Instance.LoadLevel(eventLevelSO);
        MultiplayerManager.Instance.RequestLevelChange(eventLevelSO.name);



        // Assuming GameManager has a method to change levels that accepts a level name or identifier
    }
}
