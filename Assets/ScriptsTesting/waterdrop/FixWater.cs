using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Outline))]
public class FixWater : MonoBehaviour, ISortable
{
    [SerializeField] UnityEvent onFixed;

    private void Start()
    {
        GetComponent<Outline>().enabled = false;
    }

    public void Sort()
    {
        onFixed.Invoke();
    }
}
