using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Outline))]
public class FixDrawer : MonoBehaviour, ISortable
{
    [SerializeField] UnityEvent onFixed;
    [SerializeField] UnityEvent onUnFixed;
    [SerializeField] Transform drawerTransform;
    [SerializeField] Vector3 closedLocalPosition;
    [SerializeField] float duration = 1f;

    private bool m_Sorted = false;
    bool ISortable.IsSorted => m_Sorted;

    [SerializeField] string sortableName = "Drawer";
    public string SortableName => sortableName;

    private bool inCameraRange = false;
    public bool InCameraRange { get => inCameraRange; set => inCameraRange = value; }

    private SortableSystemFOV sortableSystemFOV;

    private void Awake()
    {
        sortableSystemFOV = FindFirstObjectByType<SortableSystemFOV>();
    }

    private void Start()
    {
        GetComponent<Outline>().enabled = false;
    }

    public void Sort()
    {
        m_Sorted = true;

        // Move the drawer to the closed local position
        drawerTransform.DOLocalMove(closedLocalPosition, duration).SetEase(Ease.InOutSine);

        onFixed.Invoke();
        sortableSystemFOV?.NotifySortableStateChanged(this);
    }

    public void Unsort()
    {
        m_Sorted = false;
        onUnFixed.Invoke();
        sortableSystemFOV?.NotifySortableStateChanged(this);
    }
}
