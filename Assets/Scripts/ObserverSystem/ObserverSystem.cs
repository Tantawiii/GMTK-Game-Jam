using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObserverSystem : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float maxDetectionDistance = 50f;
    [SerializeField] private LayerMask observableLayerMask = -1;
    [SerializeField] private float updateFrequency = 0.1f;
    
    [Header("Field of View Settings")]
    [SerializeField] private bool useCustomFOV = false;
    [SerializeField] private float horizontalFOV = 60f;
    [SerializeField] private float verticalFOV = 60f;
    [SerializeField] private bool useCameraFOV = true; // Use camera's actual FOV
    
    [Header("Raycast Settings")]
    [SerializeField] private bool useRaycastOcclusion = true;
    [SerializeField] private LayerMask occlusionLayerMask = -1;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugRays = false;
    [SerializeField] private bool showDebugLogs = false;
    
    private Camera playerCamera;
    private HashSet<ObservableObject> currentlyVisible = new HashSet<ObservableObject>();
    private HashSet<ObservableObject> previouslyVisible = new HashSet<ObservableObject>();
    private Coroutine observationCoroutine;
    
    // Events for external systems
    public UnityEvent<ObservableObject> OnObjectBecameVisible;
    public UnityEvent<ObservableObject> OnObjectLeftView;
    
    void Start()
    {
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
            playerCamera = Camera.main;
            
        // Initialize FOV settings
        if (useCameraFOV && playerCamera != null)
        {
            horizontalFOV = playerCamera.fieldOfView;
            // Calculate vertical FOV based on aspect ratio
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
            CheckVisibility();
            yield return new WaitForSeconds(updateFrequency);
        }
    }
    
    private void CheckVisibility()
    {
        previouslyVisible.Clear();
        previouslyVisible.UnionWith(currentlyVisible);
        currentlyVisible.Clear();
        
        Collider[] nearbyObjects = Physics.OverlapSphere(
            transform.position, 
            maxDetectionDistance, 
            observableLayerMask
        );
        
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        
        foreach (Collider col in nearbyObjects)
        {
            ObservableObject observable = col.GetComponent<ObservableObject>();
            if (observable == null || !observable.CanBeObserved()) continue;
                
            if (IsObjectVisible(col.gameObject, frustumPlanes))
            {
                currentlyVisible.Add(observable);
                
                if (!previouslyVisible.Contains(observable))
                {
                    OnObjectBecameVisible?.Invoke(observable);
                    observable.HandleBecameObserved();
                }
            }
        }
        
        foreach (ObservableObject observable in previouslyVisible)
        {
            if (!currentlyVisible.Contains(observable))
            {
                OnObjectLeftView?.Invoke(observable);
                observable.HandleLeftObservation();
            }
        }
    }
    
    private bool IsObjectVisible(GameObject obj, Plane[] frustumPlanes)
    {
        Collider objCollider = obj.GetComponent<Collider>();
        if (objCollider == null) return false;
        
        Vector3 objectCenter = objCollider.bounds.center;
        Vector3 directionToObject = (objectCenter - transform.position).normalized;
        float distanceToObject = Vector3.Distance(transform.position, objectCenter);
        
        // Distance check first (cheapest)
        if (distanceToObject > maxDetectionDistance)
            return false;
        
        // FOV check
        if (useCustomFOV)
        {
            if (!IsWithinCustomFOV(directionToObject))
                return false;
        }
        else
        {
            // Use Unity's frustum culling
            if (!GeometryUtility.TestPlanesAABB(frustumPlanes, objCollider.bounds))
                return false;
        }
        
        // Viewport bounds check (ensure object is actually on screen)
        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(objectCenter);
        if (viewportPoint.z <= 0 || 
            viewportPoint.x < -0.1f || viewportPoint.x > 1.1f || 
            viewportPoint.y < -0.1f || viewportPoint.y > 1.1f) // Small buffer for edge cases
            return false;
        
        // Occlusion check (most expensive, do last)
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
        // Convert direction to local camera space
        Vector3 localDirection = transform.InverseTransformDirection(directionToObject);
        
        // Calculate angles from camera forward
        float horizontalAngle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
        float verticalAngle = Mathf.Atan2(localDirection.y, localDirection.z) * Mathf.Rad2Deg;
        
        // Check if within FOV
        bool withinHorizontalFOV = Mathf.Abs(horizontalAngle) <= horizontalFOV * 0.5f;
        bool withinVerticalFOV = Mathf.Abs(verticalAngle) <= verticalFOV * 0.5f;
        
        return withinHorizontalFOV && withinVerticalFOV;
    }
    
    public bool IsObjectCurrentlyVisible(ObservableObject observable)
    {
        return currentlyVisible.Contains(observable);
    }
    
    void OnDrawGizmos()
    {
        // Draw detection range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxDetectionDistance);
        
        // Draw FOV visualization
        if (useCustomFOV)
        {
            DrawFOVGizmos();
        }
        
        // Draw currently visible objects
        Gizmos.color = Color.green;
        foreach (ObservableObject observable in currentlyVisible)
        {
            if (observable != null)
                Gizmos.DrawLine(transform.position, observable.transform.position);
        }
    }
    
    private void DrawFOVGizmos()
    {
        Gizmos.color = Color.yellow;
        
        // Draw horizontal FOV
        float halfHorizontalFOV = horizontalFOV * 0.5f;
        Vector3 leftBound = Quaternion.AngleAxis(-halfHorizontalFOV, transform.up) * transform.forward;
        Vector3 rightBound = Quaternion.AngleAxis(halfHorizontalFOV, transform.up) * transform.forward;
        
        Gizmos.DrawRay(transform.position, leftBound * maxDetectionDistance);
        Gizmos.DrawRay(transform.position, rightBound * maxDetectionDistance);
        
        // Draw vertical FOV
        float halfVerticalFOV = verticalFOV * 0.5f;
        Vector3 upBound = Quaternion.AngleAxis(-halfVerticalFOV, transform.right) * transform.forward;
        Vector3 downBound = Quaternion.AngleAxis(halfVerticalFOV, transform.right) * transform.forward;
        
        Gizmos.DrawRay(transform.position, upBound * maxDetectionDistance);
        Gizmos.DrawRay(transform.position, downBound * maxDetectionDistance);
        
        // Draw FOV cone outline
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
    
    // Public method to manually check FOV settings
    public void UpdateFOVFromCamera()
    {
        if (playerCamera != null)
        {
            horizontalFOV = playerCamera.fieldOfView;
            verticalFOV = horizontalFOV / playerCamera.aspect;
            Debug.Log($"FOV updated from camera: H{horizontalFOV:F1}° V{verticalFOV:F1}°");
        }
    }
    
    // Public getters for FOV values
    public float GetHorizontalFOV() => horizontalFOV;
    public float GetVerticalFOV() => verticalFOV;
}