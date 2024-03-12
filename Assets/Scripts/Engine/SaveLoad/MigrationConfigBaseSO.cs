using UnityEngine;

public abstract class MigrationConfigBaseSO : ScriptableObject
{
    // Define any common fields or methods that all migration configurations will share
    // This could include settings or parameters needed for migration

    // Define an abstract method for migration that derived classes must implement
    public abstract void Migrate(GameDataSO gameData);
}
