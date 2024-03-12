using UnityEngine;

[CreateAssetMenu(fileName = "MigrationConfig_v1_to_v2", menuName = "Save Load/Migration Configuration/Version 1 to Version 2")]
public class MigrationConfig_v1_to_v2_SO : MigrationConfigBaseSO
{
    public override void Migrate(GameDataSO gameData)
    {
        // Implement migration logic from version 1 to version 2
        // Example:
        // If you need to add a new field to the game data, you can initialize it here
        // gameData.newField = defaultValue;
    }
}
