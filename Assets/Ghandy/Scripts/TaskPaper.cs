using UnityEngine;

public class TaskPaper : MonoBehaviour
{
    [SerializeField] private GameObject panel; // Reference to the visible UI part
    private bool isVisible = false;

    void Start()
    {
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
