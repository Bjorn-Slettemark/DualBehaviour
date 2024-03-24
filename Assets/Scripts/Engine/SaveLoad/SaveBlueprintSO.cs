using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NewSaveConfigSO", menuName = "Game/SaveConfigSO")]
public class SaveBlueprintSO : ScriptableObject
{
    public List<GameDataSO> gameDataSOList; // Reference to the specific game data

    public GameState postLoadGameState;
    // Define any other parameters necessary for saving/loading

    // Method to execute save logic
    public void Save()
    {
        // Custom save logic here
        EventChannelManager.Instance.RaiseEvent("SaveLoadEventChannel", "PlayerSave");
    }

    // Method to execute load logic
    public void Load()
    {
        // Custom load logic here
        // Example: SaveLoadManager.Instance.LoadGame(gameDataSO);
        EventChannelManager.Instance.RaiseEvent("SaveLoadEventChannel", "PlayerData");

    }
}
