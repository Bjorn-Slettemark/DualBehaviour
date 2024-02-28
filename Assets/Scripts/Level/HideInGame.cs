using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class HideInGame : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    private void Start()
    {
        // Get the MeshRenderer component attached to this GameObject
        meshRenderer = GetComponent<MeshRenderer>();

        // Check if we are in the Unity Editor
        if (!Application.isPlaying)
        {
            // If in the Unity Editor, ensure the MeshRenderer is enabled
            meshRenderer.enabled = true;
        }
        else
        {
            // If in play mode, disable the MeshRenderer to hide it in the game
            meshRenderer.enabled = false;
        }
    }
}
