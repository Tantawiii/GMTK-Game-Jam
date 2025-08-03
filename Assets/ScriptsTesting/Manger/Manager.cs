using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] TaskPaper taskPaper; // Reference to the TaskPaper script
    List<ISortable> sortables = new List<ISortable>(); // List to hold sortable items
    int sortedCount = 0; // Counter for sorted items
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject sortable in taskPaper.sortables)
        {
            sortables.Add(sortable.GetComponent<ISortable>());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
}
