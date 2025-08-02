using UnityEngine;
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Outline))]


public class UnSortedObject : MonoBehaviour, ISortable
{
    bool m_Sorted = false;
    bool ISortable.IsSorted => m_Sorted;
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
        Debug.Log($"{gameObject.name} has been sorted.");
        sortableSystemFOV?.NotifySortableStateChanged(this);
    }
    
    public void Unsort()
    {
        m_Sorted = false;
        Debug.Log($"{gameObject.name} has been unsorted.");
        sortableSystemFOV?.NotifySortableStateChanged(this);
    }
}