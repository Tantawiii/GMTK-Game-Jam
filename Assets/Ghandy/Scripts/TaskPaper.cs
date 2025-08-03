using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskPaper : MonoBehaviour
{
    [SerializeField] private GameObject panel; // Reference to the visible UI part
    [SerializeField] private Transform content;
    private bool isVisible = false;
    [SerializeField] public List<GameObject> sortables;

    void Start()
    {
        foreach(GameObject sortable in sortables)
        {
            ISortable sortableComponent = sortable.GetComponent<ISortable>();
            TextMeshProUGUI text = new GameObject("TaskText", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            text.text = sortableComponent.SortableName;
            text.color = Color.black;
            text.transform.SetParent(content, false);
        }
        if (panel != null)
            panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (panel == null) return;

            isVisible = !isVisible;
            panel.SetActive(isVisible);

            AudioManager.Instance.PlaySound2D(SoundType.PaperSound, gameObject);
        }
    }
}
