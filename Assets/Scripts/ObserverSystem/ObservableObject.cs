using UnityEngine;
using UnityEngine.Events;

public abstract class ObservableObject : MonoBehaviour
{
    [Header("Observable Settings")]
    [SerializeField] protected bool isEnabled = true;
    [SerializeField] protected float cooldownTime = 1f;
    [SerializeField] protected bool requiresDarkness = false;
    [SerializeField] protected bool onlyTriggerOnce = false;
    
    [Header("Timing")]
    [SerializeField] protected float observedDelay = 0f;
    [SerializeField] protected float unobservedDelay = 0f;
    
    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected float audioVolume = 0.7f;
    
    [Header("Debug")]
    [SerializeField] protected bool showDebugLogs = false;
    
    [Header("Events")]
    public UnityEvent OnObservedEvent;
    public UnityEvent OnUnobservedEvent;
    
    // Protected fields for derived classes
    protected float lastActionTime = 0f;
    protected bool hasTriggered = false;
    protected LightController lightController;
    protected bool isCurrentlyObserved = false;
    
    // Properties
    public bool IsEnabled => isEnabled;
    public bool IsCurrentlyObserved => isCurrentlyObserved;
    public float TimeSinceLastAction => Time.time - lastActionTime;
    
    protected virtual void Start()
    {
        // Common initialization for all observable objects
        if (requiresDarkness)
            lightController = FindFirstObjectByType<LightController>();
            
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
            
        if (audioSource != null)
            audioSource.volume = audioVolume;
            
        OnStart(); // Call derived class initialization
    }
    
    // Template method pattern - derived classes override these
    protected virtual void OnStart() { }
    protected abstract void OnBecameObserved();
    protected abstract void OnLeftObservation();
    
    // Public methods called by ObserverSystem
    public virtual bool CanBeObserved()
    {
        return isEnabled && gameObject.activeInHierarchy;
    }
    
    public void HandleBecameObserved()
    {
        if (!CanTriggerAction()) return;
        
        isCurrentlyObserved = true;
        
        if (observedDelay > 0)
            Invoke(nameof(TriggerObservedAction), observedDelay);
        else
            TriggerObservedAction();
    }
    
    public void HandleLeftObservation()
    {
        if (!CanTriggerAction()) return;
        
        isCurrentlyObserved = false;
        
        if (unobservedDelay > 0)
            Invoke(nameof(TriggerUnobservedAction), unobservedDelay);
        else
            TriggerUnobservedAction();
    }
    
    protected virtual bool CanTriggerAction()
    {
        if (!isEnabled) return false;
        if (onlyTriggerOnce && hasTriggered) return false;
        if (Time.time - lastActionTime < cooldownTime) return false;
        if (requiresDarkness && lightController != null && lightController.IsLightOn) return false;
        return true;
    }
    
    private void TriggerObservedAction()
    {
        lastActionTime = Time.time;
        hasTriggered = true;
        
        LogAction("became observed");
        OnBecameObserved();
        OnObservedEvent?.Invoke();
    }
    
    private void TriggerUnobservedAction()
    {
        lastActionTime = Time.time;
        hasTriggered = true;
        
        LogAction("left observation");
        OnLeftObservation();
        OnUnobservedEvent?.Invoke();
    }
    
    protected void LogAction(string action)
    {
        if (showDebugLogs)
            Debug.Log($"Observable {GetType().Name} '{name}' {action}");
    }
    
    protected void PlayAudioClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip, audioVolume);
    }
    
    protected void PlayRandomAudioClip(AudioClip[] clips)
    {
        if (clips.Length > 0)
            PlayAudioClip(clips[Random.Range(0, clips.Length)]);
    }
    
    // Utility methods for derived classes
    protected void SetEnabled(bool enabled) => isEnabled = enabled;
    protected void ResetTriggerState() => hasTriggered = false;
}