using UnityEngine;
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Outline))]
public class UnSortedObject : MonoBehaviour, ISortable
{
    private Animator animator;
    private void Start()
    {
        animator =  GetComponent<Animator>();
        GetComponent<Outline>().enabled = false;
    }
    public void Sort()
    {
        Debug.Log($"{gameObject.name} has been sorted.");
        animator.SetBool("IsSorted", true);
        // Add sorting logic here
    }
}