using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events; // Add this namespace

public class SortObject : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionAsset InputActions;
    private InputAction m_interactAction;

    [Header("Raycast Settings")]
    [SerializeField] private float raycastDistance = 5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private string targetTag = "Sortable";

    [Header("Sorting Event")]
    public UnityEvent OnSortEvent; // The Unity Event for delegation

    private Camera playerCamera;

    private void Awake()
    {
        playerCamera = Camera.main;
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
        if (m_interactAction != null && m_interactAction.WasPressedThisFrame())
        {
            TryInteractWithObject();
        }
    }

    private void TryInteractWithObject()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, interactableLayer))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                // Invoke the event instead of calling Sort() directly
                OnSortEvent.Invoke();
            }
        }
    }

    // Remove the empty Sort() method
}