using UnityEngine;
using UnityEngine.UI;

public class RotatingPrefabButton : MonoBehaviour
{
    [SerializeField] private GameObject prefabToDisplay;
    [SerializeField] private RawImage displayImage;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 30, 0); // Degrees per second
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, -5);
    [SerializeField] private Vector3 prefabScale = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 prefabPos = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 prefabRot = new Vector3(-20f, -1.9f, 0);

    [SerializeField] private bool addLightToPrefab = true;

    private GameObject instantiatedPrefab;
    private RenderTexture renderTexture;
    private Camera renderCamera;

    private void Start()
    {
        if (prefabToDisplay == null || displayImage == null)
        {
            Debug.LogError("Prefab or RawImage not assigned!");
            return;
        }

        SetupRenderTexture();
        SetupCamera();
        InstantiatePrefab();
    }

    private void SetupRenderTexture()
    {
        renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        renderTexture.Create();
        displayImage.texture = renderTexture;
    }

    private void SetupCamera()
    {
        GameObject cameraObj = new GameObject("RenderCamera");
        renderCamera = cameraObj.AddComponent<Camera>();
        renderCamera.transform.tag = "not";
        renderCamera.targetTexture = renderTexture;
        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.backgroundColor = Color.white;
        renderCamera.cullingMask = -1; // Render everything

        cameraObj.transform.SetParent(transform, false);
        cameraObj.transform.localPosition = cameraOffset;
        cameraObj.transform.LookAt(transform.position);
    }

    private void InstantiatePrefab()
    {
        instantiatedPrefab = Instantiate(prefabToDisplay, transform);
        instantiatedPrefab.transform.localPosition = prefabPos;
        instantiatedPrefab.transform.localRotation = Quaternion.Euler(prefabRot);
        // Adjust the scale if needed
        instantiatedPrefab.transform.localScale = prefabScale;


        // Ensure all child renderers are visible to the camera
        Renderer[] renderers = instantiatedPrefab.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }
    }

    private void Update()
    {
        if (instantiatedPrefab != null)
        {
            instantiatedPrefab.transform.Rotate(rotationSpeed * Time.deltaTime);
        }
        instantiatedPrefab.transform.localScale = prefabScale;
        instantiatedPrefab.transform.localPosition = prefabPos;

    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}