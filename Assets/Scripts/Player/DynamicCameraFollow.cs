using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicCameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    public float damping = 1;
    public float lookAheadFactor = 3;
    public float lookAheadReturnSpeed = 0.5f;
    public float lookAheadMoveThreshold = 0.1f;
    public bool averagePositionEnabled = false;
    public float averagePositionWeight = 0.5f;
    public Vector3 cameraOffset;
    public bool lockToXZPlane = false;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    [Header("X Offset Zoom Settings")]
    public float minZOffset = -10f;
    public float maxZOffset = 10f;

    [Header("Perspective Bias")]
    public AnimationCurve perspectiveBiasCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float perspectiveBiasScale = 1.0f;

    private Vector3 m_LastTargetPosition;
    private Vector3 m_CurrentVelocity;
    private Vector3 m_LookAheadPos;
    private Camera m_Camera;
    private Vector3 lastValidMouseWorldPosition;

    [Header("Event Channels")]
    public GameEventChannelSO spawnEventChannel; // Assign in the inspector

    private void OnEnable()
    {
        spawnEventChannel.RegisterListener(HandleEvent, "PlayerSpawned");
    }

    private void OnDisable()
    {
        spawnEventChannel.UnregisterListener(HandleEvent);
    }
    private void Start()
    {
        m_Camera = Camera.main;

        // Initialize cameraOffset.y to start zoomed out
        cameraOffset.y = maxZoom;

        // If you want to also set an initial Z Offset based on the max zoom, do it here
        float initialZoomFactor = (maxZoom - minZoom) / (maxZoom - minZoom); // This will be 1, but it's shown for consistency
        cameraOffset.z = Mathf.Lerp(minZOffset, maxZOffset, initialZoomFactor);


    }


    public void InitializeCamera(Transform newTarget)
    {
        target = newTarget;
        Vector3 originalOffsetFromOrigin = transform.position - Vector3.zero;
        transform.position = target.position + originalOffsetFromOrigin;
        m_LastTargetPosition = target.position;
    }

    private void Update()
    {
        if (target == null) return;

        // Handle zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cameraOffset.y -= scroll * zoomSpeed;
        cameraOffset.y = Mathf.Clamp(cameraOffset.y, minZoom, maxZoom);

        // Adjust X Offset based on Zoom
        float zoomFactor = (cameraOffset.y - minZoom) / (maxZoom - minZoom);
        cameraOffset.z = Mathf.Lerp(minZOffset, maxZOffset, zoomFactor);

        Vector3 moveDelta = target.position - m_LastTargetPosition;
        bool updateLookAheadTarget = moveDelta.magnitude > lookAheadMoveThreshold;

        if (updateLookAheadTarget)
        {
            m_LookAheadPos = lookAheadFactor * new Vector3(moveDelta.x, 0, moveDelta.z);
        }
        else
        {
            m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
        }

        Vector3 aheadTargetPos = target.position + m_LookAheadPos + cameraOffset;

        // Apply perspective bias
        float relativeZ = (target.position - transform.position).z;
        float bias = perspectiveBiasCurve.Evaluate(Mathf.InverseLerp(-perspectiveBiasScale, perspectiveBiasScale, relativeZ));
        aheadTargetPos += Vector3.forward * bias * perspectiveBiasScale;

        if (averagePositionEnabled)
        {
            Vector3 averagePosition = CalculateAveragePosition(aheadTargetPos);
            aheadTargetPos = Vector3.Lerp(aheadTargetPos, averagePosition, averagePositionWeight);
        }

        Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
        transform.position = newPos;

        m_LastTargetPosition = target.position;
    }

    private Vector3 CalculateAveragePosition(Vector3 aheadTargetPos)
    {
        Vector3 averagePosition;
        // Check if the EventSystem.current is not null and then if the mouse is over a UI element
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Return the last valid average position if the mouse is over UI
            averagePosition = (target.position + lastValidMouseWorldPosition) / 2;
            return averagePosition;
        }

        Vector3 mousePosition = Input.mousePosition;
        // Ensure m_Camera is not null before using it
        if (m_Camera == null)
        {
            Debug.LogWarning("Camera is not assigned in DynamicCameraFollow script.");
            return aheadTargetPos; // Return aheadTargetPos or some default value to avoid breaking the flow
        }
        mousePosition.z = m_Camera.WorldToScreenPoint(target.position).z;
        Vector3 mouseWorldPosition = m_Camera.ScreenToWorldPoint(mousePosition);

        // Update lastValidMouseWorldPosition with the current valid mouseWorldPosition
        lastValidMouseWorldPosition = mouseWorldPosition;

        averagePosition = (target.position + mouseWorldPosition) / 2;
        return averagePosition;
    }
    private void HandleEvent(string eventName)
    {
        if (eventName == "PlayerSpawned")
        {
            InitializeCamera(GameObject.FindGameObjectWithTag("Player").transform);
        }
        
    }
}
