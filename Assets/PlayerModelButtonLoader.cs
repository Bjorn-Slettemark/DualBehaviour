using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerModelDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string modelResourcePath;
    public Transform displayPoint;
    public Vector3 displayRotation = new Vector3(0, 180, 0);
    public Vector3 displayScale = Vector3.one;
    public float rotationSpeed = 30f;

    private GameObject displayedModel;
    private Button button;
    private Color originalColor;
    private bool isSelected = false;

    public delegate void ModelSelectedHandler(string modelPath);
    public event ModelSelectedHandler OnModelSelected;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            originalColor = button.image.color;
        }
        LoadAndDisplayModel();
    }

    void Update()
    {
        if (displayedModel != null)
        {
            displayedModel.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    void LoadAndDisplayModel()
    {
        if (displayPoint == null)
        {
            Debug.LogError($"Display point is not set for {gameObject.name}!");
            return;
        }

        GameObject modelPrefab = Resources.Load<GameObject>(modelResourcePath);
        if (modelPrefab != null)
        {
            displayedModel = Instantiate(modelPrefab, displayPoint.position, Quaternion.Euler(displayRotation));
            displayedModel.transform.SetParent(displayPoint);
            displayedModel.transform.localScale = displayScale;

            // Disable any components that might interfere with display (e.g., Colliders, Rigidbody)
            Collider[] colliders = displayedModel.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
            Rigidbody rb = displayedModel.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            Debug.Log($"Model displayed: {displayedModel.name} at position {displayedModel.transform.position}");
        }
        else
        {
            Debug.LogError($"Failed to load model: {modelResourcePath}");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && button != null)
        {
            button.image.color = Color.yellow;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected && button != null)
        {
            button.image.color = originalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }

    public void Select()
    {
        isSelected = true;
        if (button != null)
        {
            button.image.color = Color.blue;
        }
        OnModelSelected?.Invoke(modelResourcePath);
        Debug.Log($"Model selected: {modelResourcePath}");
    }

    public void Deselect()
    {
        isSelected = false;
        if (button != null)
        {
            button.image.color = originalColor;
        }
    }
}