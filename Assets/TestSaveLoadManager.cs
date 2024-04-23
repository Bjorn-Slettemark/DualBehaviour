using UnityEngine;

public class TestSaveLoadManager : MonoBehaviour
{
    public PlayerDataSO playerData; // Assign this in the Unity editor
    [SerializeField]
    private GameEventChannelSO saveEventChannel;

    [SerializeField]
    private GameEventChannelSO loadEventChannel;

    private void Update()
    {
        // Save player data when pressing '1'
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EventChannelManager.Instance.RaiseEvent(saveEventChannel, "Save");
        }

        // Load player data when pressing '2'
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EventChannelManager.Instance.RaiseEvent(loadEventChannel, "Load");

        }
    }
}
