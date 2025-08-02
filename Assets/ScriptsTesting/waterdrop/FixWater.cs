using UnityEngine;
using UnityEngine.Events;

public class FixWater : MonoBehaviour, ISortable
{
    [SerializeField] UnityEvent onFixed;
    public void Sort()
    {
        onFixed.Invoke();
    }
}
