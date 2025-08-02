using UnityEngine;

public class UnSortedObject : MonoBehaviour, ISortable
{
    public void Sort()
    {
        Debug.Log($"{gameObject.name} has been sorted.");
        // Add sorting logic here
    }
}