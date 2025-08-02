using UnityEngine;

public class UnSortedObject : MonoBehaviour, ISortable
{
   

    public void Sort()
    {

        if(gameObject.CompareTag("droplet"))
            { UnityEngine.Object.Destroy(gameObject); }
        Debug.Log($"{gameObject.name} has been sorted.");
        // Add sorting logic here
    }
}