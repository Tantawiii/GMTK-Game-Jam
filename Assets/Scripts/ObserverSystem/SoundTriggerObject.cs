using UnityEngine;

public class SoundTriggerObject : ObservableObject
{
    [Header("Sound Settings")]
    [SerializeField] private AudioClip[] soundsWhenUnobserved;
    [SerializeField] private AudioClip[] soundsWhenObserved;
    [SerializeField] private float soundChance = 0.8f;
    [SerializeField] private float minDelay = 0.5f;
    [SerializeField] private float maxDelay = 3f;
    
    protected override void OnBecameObserved()
    {
        if (Random.value < soundChance)
        {
            float delay = Random.Range(minDelay, maxDelay);
            Invoke(nameof(PlayObservedSound), delay);
        }
    }
    
    protected override void OnLeftObservation()
    {
        if (Random.value < soundChance)
        {
            float delay = Random.Range(minDelay, maxDelay);
            Invoke(nameof(PlayUnobservedSound), delay);
        }
    }
    
    private void PlayObservedSound()
    {
        PlayRandomAudioClip(soundsWhenObserved);
    }
    
    private void PlayUnobservedSound()
    {
        PlayRandomAudioClip(soundsWhenUnobserved);
    }
}