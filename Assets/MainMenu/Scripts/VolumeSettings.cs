using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("Sliders")]
    public Slider volumeSlider;
    public Slider sfxSlider;

    [Header("Defaults")]
    [Range(0f, 1f)] public float defaultVolume = 0.5f;
    [Range(0f, 1f)] public float defaultSFX = 0.5f;


    private void Start()
    {
        if (volumeSlider == null || sfxSlider == null)
        {
            Debug.LogError("Volume or SFX slider not assigned!");
            return;
        }

        // Load saved values or use defaults
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", defaultVolume);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", defaultSFX);

        volumeSlider.value = savedVolume;
        sfxSlider.value = savedSFX;

        // Add listeners
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);

        // Apply initial values
        ApplyVolume(savedVolume);
        ApplySFX(savedSFX);
    }

    private void OnVolumeChanged(float value)
    {
        ApplyVolume(value);
        PlayerPrefs.SetFloat("GameVolume", value);
    }

    private void OnSFXChanged(float value)
    {
        ApplySFX(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void ApplyVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetBackgroundVolume(value);
    }

    private void ApplySFX(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
    }
}
