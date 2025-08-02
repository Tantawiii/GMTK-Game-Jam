using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events; // Add this namespace


public class SortObject : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionAsset InputActions;
    private InputAction m_interactAction;
    
    [Header("Raycast Settings")]
    [SerializeField] private float raycastDistance = 50f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private string targetTag = "Sortable";
    
    [Header("Observer Integration")]
    [SerializeField] private bool useObserverSystem = true;
    [SerializeField] private bool onlyInteractWithVisibleObjects = true;
    
    private Outline sortableOutline = null;
    private Camera playerCamera;
     private SortableSystemFOV sortableSystemFOV;
    
    private void Awake()
    {
        playerCamera = Camera.main;
        sortableSystemFOV = FindFirstObjectByType<SortableSystemFOV>();
        
        var playerMap = InputActions.FindActionMap("Player");
        m_interactAction = playerMap.FindAction("Interact");
        
        if (m_interactAction == null)
        {
            Debug.LogError("Interact action not found!");
        }
    }
    
    private void OnEnable() => m_interactAction?.Enable();
    private void OnDisable() => m_interactAction?.Disable();
    
    private void Update()
    {
        TryInteractWithObject();
    }
    
    private void TryInteractWithObject()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, raycastDistance, interactableLayer))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                ISortable sortable = hit.collider.GetComponent<ISortable>();
                if (sortable != null)
                {
                    // Check if we should only interact with visible objects
                    if (onlyInteractWithVisibleObjects && useObserverSystem && sortableSystemFOV != null)
                    {
                        if (!sortableSystemFOV.IsSortableCurrentlyVisible(sortable))
                        {
                            // Object is not visible to observer system, don't interact
                            DisableOutline();
                            return;
                        }
                    }
                    
                    // Handle outline highlighting
                    Outline outline = hit.collider.GetComponent<Outline>();
                    if (outline != sortableOutline && sortableOutline != null)
                    {
                        sortableOutline.enabled = false;
                    }
                    sortableOutline = outline;
                    
                    if (sortableOutline != null)
                    {
                        sortableOutline.enabled = true;
                        // Set outline color based on sort state
                        sortableOutline.OutlineColor = sortable.IsSorted ? Color.green : Color.yellow;
                    }
                    
                    // Handle interaction input
                    if (m_interactAction != null && m_interactAction.WasPressedThisFrame())
                    {
                        if (sortable.IsSorted)
                        {
                            sortable.Unsort();
                        }
                        else
                        {
                            sortable.Sort();
                        }
                    }
                }
                else
                {
                    DisableOutline();
                }
            }
            else
            {
                DisableOutline();
            }
        }
        else
        {
            DisableOutline();
        }
    }
    
    private void DisableOutline()
    {
        if (sortableOutline != null)
        {
            sortableOutline.enabled = false;
            sortableOutline = null;
        }
    }
}
public interface ISortable
{
    bool IsSorted { get; }
    void Sort();
    void Unsort();
}