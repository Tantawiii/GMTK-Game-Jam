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

    private void Start()
    {
        GetComponent<Outline>().enabled = false;
    }

    public void Sort()
    {
        m_Sorted = true;
        onFixed.Invoke();
    }

    public void Unsort()
    {
        m_Sorted = false;
        onUnFixed.Invoke();
    }
}
