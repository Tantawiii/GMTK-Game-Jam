using DG.Tweening;
using UnityEngine;

public class Waterdrop : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    [SerializeField] float lifetime = 0.5f;
    [SerializeField] float waittime = 1f;
    [SerializeField] ParticleSystem splashEffect;
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
        AudioManager.Instance.PlaySoundAt(SoundType.WaterDrop, end.gameObject);
        splashEffect.Play();
        if (waterfixed) return;
        Invoke(nameof(godown), waittime);
    }

    public void fixwater()
    {
        waterfixed = true;
    }

    public void unfixwater()
    {
        waterfixed = false;
        godown();
    }
}
