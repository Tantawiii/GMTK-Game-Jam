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

    public bool InCameraRange => throw new System.NotImplementedException();

    public string SortableName => "Water drop";

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