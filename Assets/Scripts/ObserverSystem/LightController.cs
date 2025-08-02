using UnityEngine;
using UnityEngine.Events;

public class LightController : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light lightComponent;
    [SerializeField] private bool isOn = true;
    [SerializeField] private bool allowPlayerToggle = true;

    
    [Header("Events")]
    public UnityEvent OnLightTurnedOn;
    public UnityEvent OnLightTurnedOff;
    
    public bool IsLightOn => isOn;
    
    void Start()
    {
        UpdateLightState();
    }
    
    void Update()
    {
        if (allowPlayerToggle && Input.GetKeyDown(KeyCode.E))
        {
            ToggleLight();
        }
    }
    
    public void ToggleLight()
    {
        SetLightState(!isOn);
    }
    
    public void TurnLightOn()
    {
        SetLightState(true);
    }
    
    public void TurnLightOff()
    {
        SetLightState(false);
    }
    
    public void SetLightState(bool newState)
    {
        if (isOn == newState) return;
        
        isOn = newState;
        UpdateLightState();
        
        if (isOn)
            OnLightTurnedOn?.Invoke();
        else
            OnLightTurnedOff?.Invoke();
    }
    
    private void UpdateLightState()
    {
        if (lightComponent != null)
            lightComponent.enabled = isOn;
    }
}