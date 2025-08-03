using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField] private TaskPaper taskPaper; // Reference to the TaskPaper script
    private List<ISortable> sortables = new();
    private float TimeForCheckInSeconds = 60f;
    private float checkTimer;

    void Start()
    {
        foreach (GameObject sortable in taskPaper.sortables)
        {
            ISortable sortableComponent = sortable.GetComponent<ISortable>();
            if (sortableComponent != null)
                sortables.Add(sortableComponent);
        }

        checkTimer = TimeForCheckInSeconds;
    }

    void Update()
    {
        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            CheckAndUnsortIfNeeded();
            checkTimer = TimeForCheckInSeconds; // reset timer
        }
    }

    void CheckAndUnsortIfNeeded()
    {
        List<ISortable> sorted = new();
        List<ISortable> unsorted = new();

        foreach (var item in sortables)
        {
            if (item.IsSorted)
                sorted.Add(item);
            else
                unsorted.Add(item);
        }

        if (unsorted.Count >= 7)
        {
            Debug.Log("Enough unsorted objects. No action taken.");
            return;
        }

        if (sorted.Count == 0)
        {
            Debug.Log("No sorted objects available to unsort.");
            return;
        }

        // Pick a random sorted object and unsort it
        int index = Random.Range(0, sorted.Count);
        sorted[index].Unsort();
    }



    /// <summary>
    /// Checks if all objects in the list are unsorted.
    /// Logs the result (does not take any action).
    /// </summary>
    public void CheckIfAllObjectsAreUnsorted()
    {
        foreach (var item in sortables)
        {
            if (item.IsSorted)
            {
                Debug.Log("Not all objects are unsorted.");
                return;
            }
        }
        SceneManager.LoadScene("EndGameScene");
        Debug.Log("✅ All objects are unsorted.");
    }

}
