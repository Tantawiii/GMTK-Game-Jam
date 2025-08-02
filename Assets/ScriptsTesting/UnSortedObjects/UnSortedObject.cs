using UnityEngine;
using UnityEngine.Events;

public class UnSortedObject : MonoBehaviour, ISortable
{
    [SerializeField] UnityEvent onFixed;

    public void Sort()
    {

        if(gameObject.CompareTag("droplet"))
        {
            onFixed.Invoke();
        }
        Debug.Log($"{gameObject.name} has been sorted.");
        // Add sorting logic here
    }
}