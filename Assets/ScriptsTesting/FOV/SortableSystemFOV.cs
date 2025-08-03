using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SortableSystemFOV : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float maxDetectionDistance = 50f;
    [SerializeField] private LayerMask observableLayerMask = -1;
    [SerializeField] private float updateFrequency = 0.1f;
    
    [Header("Field of View Settings")]
    [SerializeField] private bool useCustomFOV = false;
    [SerializeField] private float horizontalFOV = 60f;
    [SerializeField] private float verticalFOV = 60f;
    [SerializeField] private bool useCameraFOV = true;
    
    [Header("Raycast Settings")]
    [SerializeField] private bool useRaycastOcclusion = true;
    [SerializeField] private LayerMask occlusionLayerMask = -1;
    
    [Header("Sortable Object Integration")]
    [SerializeField] private bool trackSortableObjects = true;
    [SerializeField] private bool highlightVisibleSortables = true;
    [SerializeField] private Color sortedObjectColor = Color.green;
    [SerializeField] private Color unsortedObjectColor = Color.red;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugRays = false;
    [SerializeField] private bool showDebugLogs = false;
    
    private Camera playerCamera;
    private HashSet<ISortable> currentlyVisibleSortables = new HashSet<ISortable>();
    private HashSet<ISortable> previouslyVisibleSortables = new HashSet<ISortable>();
    private Coroutine observationCoroutine;
    
    // Events for external systems
    
    // Events specifically for sortable objects
    public UnityEvent<ISortable> OnSortableBecameVisible;
    public UnityEvent<ISortable> OnSortableLeftView;
    public UnityEvent<ISortable> OnSortableStateChanged;
    
    void Start()
    {
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
            playerCamera = Camera.main;
            
        if (useCameraFOV && playerCamera != null)
        {
            horizontalFOV = playerCamera.fieldOfView;
            verticalFOV = horizontalFOV / playerCamera.aspect;
        }
            
        observationCoroutine = StartCoroutine(ObservationLoop());
        
        if (showDebugLogs)
            Debug.Log($"Observer System initialized - FOV: H{horizontalFOV:F1}° V{verticalFOV:F1}°");
    }
    
    void OnDestroy()
    {
        if (observationCoroutine != null)
            StopCoroutine(observationCoroutine);
    }
    
    private IEnumerator ObservationLoop()
    {
        while (true)
        {
            if (trackSortableObjects)
                CheckSortableVisibility();
            yield return new WaitForSeconds(updateFrequency);
        }
    }
    
    private void CheckSortableVisibility()
    {
        previouslyVisibleSortables.Clear();
        previouslyVisibleSortables.UnionWith(currentlyVisibleSortables);
        currentlyVisibleSortables.Clear();
        
        // Find all sortable objects in range
        Collider[] nearbyObjects = Physics.OverlapSphere(
            transform.position, 
            maxDetectionDistance, 
            observableLayerMask
        );
        
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        
        foreach (Collider col in nearbyObjects)
        {
            ISortable sortable = col.GetComponent<ISortable>();
            if (sortable == null) continue;
            
            if (IsObjectVisible(col.gameObject, frustumPlanes))
            {
                currentlyVisibleSortables.Add(sortable);
                
                // Check if this sortable just became visible
                if (!previouslyVisibleSortables.Contains(sortable))
                {
                    OnSortableBecameVisible?.Invoke(sortable);
                    
                    if (highlightVisibleSortables)
                        UpdateSortableHighlight(sortable, true);
                    
                    if (showDebugLogs)
                        Debug.Log($"Sortable object {((MonoBehaviour)sortable).name} became visible");
                }
            }
            else
            {
                col.GetComponent<Outline>().enabled = false;
                sortable.InCameraRange = false;
            }
        }
        
        // Check for sortables that left view
        foreach (ISortable sortable in previouslyVisibleSortables)
        {
            if (!currentlyVisibleSortables.Contains(sortable))
            {
                OnSortableLeftView?.Invoke(sortable);
                
                if (highlightVisibleSortables)
                    UpdateSortableHighlight(sortable, false);
                
                if (showDebugLogs)
                    Debug.Log($"Sortable object {((MonoBehaviour)sortable).name} left view");
            }
        }
    }
    
    private void UpdateSortableHighlight(ISortable sortable, bool isVisible)
    {
        MonoBehaviour sortableComponent = sortable as MonoBehaviour;
        if (sortableComponent == null) return;
        
        Outline outline = sortableComponent.GetComponent<Outline>();
        if (outline == null) return;
        
        if (isVisible)
        {
            //outline.enabled = true;
            //outline.OutlineColor = sortable.IsSorted ? sortedObjectColor : unsortedObjectColor;
            sortable.InCameraRange = true;
        }
        else
        {
            outline.enabled = false;
            sortable.InCameraRange = false;
        }
    }
    
    private bool IsObjectVisible(GameObject obj, Plane[] frustumPlanes)
    {
        Collider objCollider = obj.GetComponent<Collider>();
        if (objCollider == null) return false;
        
        Vector3 objectCenter = objCollider.bounds.center;
        Vector3 directionToObject = (objectCenter - transform.position).normalized;
        float distanceToObject = Vector3.Distance(transform.position, objectCenter);
        
        if (distanceToObject > maxDetectionDistance)
            return false;
        
        if (useCustomFOV)
        {
            if (!IsWithinCustomFOV(directionToObject))
                return false;
        }
        else
        {
            if (!GeometryUtility.TestPlanesAABB(frustumPlanes, objCollider.bounds))
                return false;
        }
        
        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(objectCenter);
        if (viewportPoint.z <= 0 || 
            viewportPoint.x < -0.1f || viewportPoint.x > 1.1f || 
            viewportPoint.y < -0.1f || viewportPoint.y > 1.1f)
            return false;
        
        if (useRaycastOcclusion)
        {
            if (Physics.Raycast(transform.position, directionToObject, out RaycastHit hit, 
                distanceToObject, occlusionLayerMask))
            {
                if (showDebugRays)
                {
                    Debug.DrawRay(transform.position, directionToObject * distanceToObject, 
                        hit.collider.gameObject == obj ? Color.green : Color.red, 0.1f);
                }
                return hit.collider.gameObject == obj;
            }
        }
        
        return true;
    }
    
    private bool IsWithinCustomFOV(Vector3 directionToObject)
    {
        Vector3 localDirection = transform.InverseTransformDirection(directionToObject);
        
        float horizontalAngle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
        float verticalAngle = Mathf.Atan2(localDirection.y, localDirection.z) * Mathf.Rad2Deg;
        
        bool withinHorizontalFOV = Mathf.Abs(horizontalAngle) <= horizontalFOV * 0.5f;
        bool withinVerticalFOV = Mathf.Abs(verticalAngle) <= verticalFOV * 0.5f;
        
        return withinHorizontalFOV && withinVerticalFOV;
    }
    
    public bool IsSortableCurrentlyVisible(ISortable sortable)
    {
        return currentlyVisibleSortables.Contains(sortable);
    }
    
    public HashSet<ISortable> GetVisibleSortables()
    {
        return new HashSet<ISortable>(currentlyVisibleSortables);
    }
    
    public void NotifySortableStateChanged(ISortable sortable)
    {
        OnSortableStateChanged?.Invoke(sortable);
        
        if (currentlyVisibleSortables.Contains(sortable) && highlightVisibleSortables)
        {
            UpdateSortableHighlight(sortable, true);
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxDetectionDistance);
        
        if (useCustomFOV)
        {
            DrawFOVGizmos();
        }
        
        // Draw currently visible objects
        Gizmos.color = Color.green;
        // Draw currently visible sortables
        if (trackSortableObjects)
        {
            foreach (ISortable sortable in currentlyVisibleSortables)
            {
                MonoBehaviour sortableComponent = sortable as MonoBehaviour;
                if (sortableComponent != null)
                {
                    Gizmos.color = sortable.IsSorted ? Color.green : Color.red;
                    Gizmos.DrawLine(transform.position, sortableComponent.transform.position);
                }
            }
        }
    }
    
    private void DrawFOVGizmos()
    {
        Gizmos.color = Color.yellow;
        
        float halfHorizontalFOV = horizontalFOV * 0.5f;
        Vector3 leftBound = Quaternion.AngleAxis(-halfHorizontalFOV, transform.up) * transform.forward;
        Vector3 rightBound = Quaternion.AngleAxis(halfHorizontalFOV, transform.up) * transform.forward;
        
        Gizmos.DrawRay(transform.position, leftBound * maxDetectionDistance);
        Gizmos.DrawRay(transform.position, rightBound * maxDetectionDistance);
        
        float halfVerticalFOV = verticalFOV * 0.5f;
        Vector3 upBound = Quaternion.AngleAxis(-halfVerticalFOV, transform.right) * transform.forward;
        Vector3 downBound = Quaternion.AngleAxis(halfVerticalFOV, transform.right) * transform.forward;
        
        Gizmos.DrawRay(transform.position, upBound * maxDetectionDistance);
        Gizmos.DrawRay(transform.position, downBound * maxDetectionDistance);
        
        int segments = 20;
        Vector3[] fovPoints = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float angle = (i / (float)segments) * horizontalFOV - halfHorizontalFOV;
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            fovPoints[i] = transform.position + direction * maxDetectionDistance;
            
            if (i > 0)
                Gizmos.DrawLine(fovPoints[i-1], fovPoints[i]);
        }
    }
    
    public void UpdateFOVFromCamera()
    {
        if (playerCamera != null)
        {
            horizontalFOV = playerCamera.fieldOfView;
            verticalFOV = horizontalFOV / playerCamera.aspect;
            Debug.Log($"FOV updated from camera: H{horizontalFOV:F1}° V{verticalFOV:F1}°");
        }
    }
    
    public float GetHorizontalFOV() => horizontalFOV;
    public float GetVerticalFOV() => verticalFOV;
}
