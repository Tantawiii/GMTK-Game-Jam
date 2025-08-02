using UnityEngine;

public class UnSortedObject : MonoBehaviour
{
    private void OnEnable()
    {
        // Find SortObject and subscribe to its event
        SortObject sorter = FindFirstObjectByType<SortObject>();
        if (sorter != null)
        {
            sorter.OnSortEvent.AddListener(Sort);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe when disabled
        SortObject sorter = FindFirstObjectByType<SortObject>();
        if (sorter != null)
        {
            sorter.OnSortEvent.RemoveListener(Sort);
        }
    }

    public void Sort()
    {
        Debug.Log($"{gameObject.name} has been sorted.");
        // Add sorting logic here
    }
}