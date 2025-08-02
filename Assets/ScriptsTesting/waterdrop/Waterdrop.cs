using DG.Tweening;
using UnityEngine;

public class Waterdrop : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    [SerializeField] float lifetime = 0.5f;
    [SerializeField] float waittime = 5f;
    private bool waterfixed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        godown();
    }

    void godown()
    {
        transform.DOMove(end.position, lifetime)
            .SetEase(Ease.InQuad)
            .OnComplete(() => endtween());
    }

    void endtween()
    {
        transform.position = start.position;
        if (waterfixed) return;
        godown();
    }

    public void fixwater()
    {
        waterfixed = true;
        Invoke(nameof(unfixwater), waittime);
    }

    void unfixwater()
    {
        waterfixed = false;
    }
}
