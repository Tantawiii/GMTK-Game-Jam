using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Outline))]
public class FixWater : MonoBehaviour, ISortable
{
    [SerializeField] UnityEvent onFixed;
    public void Sort()
    {
        onFixed.Invoke();
    }
}
