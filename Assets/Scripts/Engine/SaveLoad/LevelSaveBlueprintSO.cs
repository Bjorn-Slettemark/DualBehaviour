using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSaveBlueprint", menuName = "Game/SaveBlueprintSO")]
public class LevelSaveBlueprintSO : SaveBlueprintSO
{
    // Define any other parameters necessary for saving/loading

    // Method to execute save logic
    public new void Save()
    {
        // Custom save logic here
        //SaveLoadManager.Instance.SaveGame(gameDataSO[0]);
    }

    // Method to execute load logic
    public new void Load()
    {
        // Custom load logic here
        //SaveLoadManager.Instance.LoadGame(gameDataSO[0]);

    }
}
