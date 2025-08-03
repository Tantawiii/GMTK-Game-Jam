using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Outline))]
public class FixWater : MonoBehaviour, ISortable
{
    [SerializeField] UnityEvent onFixed;
    [SerializeField] UnityEvent onUnFixed;
    
    bool m_Sorted = false;
    bool ISortable.IsSorted => m_Sorted;

    [SerializeField] string sortableName = "Water drop";
    public string SortableName => sortableName;

    bool inCameraRange = false;
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