using System;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Footstep,
    ButtonClick,
    DoorOpen,
    DoorClose,
    LightSwitch,
    WaterDrop,
    ClockTick,
    FixFrame,
    TapOpenClose,
    FridgeNoise,
    PickUp,
    Cleanig,
    TvNoise,
    TvOn,
    TvOff,
    PaperSound,
    Drawer,
    // Add more types as needed
}

[Serializable]
public class SoundEntry
{
    public SoundType Type;
    public AudioClip Clip;
}

public class AudioManager : MonoBehaviour
{
    // === Singleton Access ===
    public static AudioManager Instance { get; private set; }

    // === Serialized Fields (Editable in Inspector) ===
    [Header("Background Music")]
    [SerializeField] private AudioClip BackgroundMusic;
    [SerializeField, Range(0f, 1f)] private float BackgroundVolume = 0.2f;

    [Header("Sound Effects")]
    [SerializeField] private List<SoundEntry> SoundClipsList;

    // === Runtime ===
    private Dictionary<SoundType, AudioClip> SoundClipsDict = new();
    private AudioSource BackgroundAudioSource;

    // === Initialization ===
    private void Awake()
    {
        SetupSingleton();
        BuildSoundDictionary();
        InitializeBackgroundAudio();
    }

    private void SetupSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void BuildSoundDictionary()
    {
        foreach (var entry in SoundClipsList)
        {
            if (!SoundClipsDict.ContainsKey(entry.Type))
                SoundClipsDict.Add(entry.Type, entry.Clip);
        }
    }

    private void InitializeBackgroundAudio()
    {
        BackgroundAudioSource = GetComponent<AudioSource>();

        if (BackgroundAudioSource == null)
            BackgroundAudioSource = gameObject.AddComponent<AudioSource>();

        BackgroundAudioSource.loop = true;
        BackgroundAudioSource.playOnAwake = true;
        BackgroundAudioSource.volume = BackgroundVolume;
        BackgroundAudioSource.spatialBlend = 0.0f; // 2D sound

        if (BackgroundMusic != null)
        {
            BackgroundAudioSource.clip = BackgroundMusic;
            BackgroundAudioSource.Play();
        }
    }

    public void SetBackgroundVolume(float newVolume)
    {
        BackgroundVolume = Mathf.Clamp01(newVolume);
        if (BackgroundAudioSource != null)
            BackgroundAudioSource.volume = BackgroundVolume;
    }

    public void PlaySoundAt(SoundType type, GameObject target)
    {
        if (!SoundClipsDict.TryGetValue(type, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"Sound type '{type}' not found or clip is null.");
            return;
        }

        AudioSource tempSource = target.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.spatialBlend = 1.0f; // 3D sound
        tempSource.Play();

        Destroy(tempSource, clip.length);
    }


    public void PlayLoop(SoundType type, GameObject target)
    {
        if (!SoundClipsDict.TryGetValue(type, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"Sound type '{type}' not found or clip is null.");
            return;
        }

        AudioSource tempSource = target.AddComponent<AudioSource>();

        tempSource.clip = clip;
        tempSource.spatialBlend = 1.0f; // 3D sound
        tempSource.loop = true;
        tempSource.spatialBlend = 1.0f;
        tempSource.playOnAwake = false;
        tempSource.Play();
    }


    public void PauseSound(AudioSource source)
    {
        if (source != null && source.isPlaying)
            source.Pause();
    }


    public void ResumeSound(AudioSource source)
    {
        if (source != null && !source.isPlaying)
            source.Play();
    }


    public void StopSound(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            source.clip = null;
            Destroy(source, source.clip.length);
        }
    }

}
