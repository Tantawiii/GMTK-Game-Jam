using UnityEngine;
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Outline))]
public class UnSortedObject : MonoBehaviour, ISortable
{
    private void Start()
    {
        GetComponent<Outline>().enabled = false;
    }
    public void Sort()
    {
        Debug.Log($"{gameObject.name} has been sorted.");
        // Add sorting logic here
    }
}