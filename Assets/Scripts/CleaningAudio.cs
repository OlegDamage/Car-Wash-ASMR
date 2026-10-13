using UnityEngine;

[RequireComponent(typeof(DirtCleaner))]
[RequireComponent(typeof(AudioSource))]
public class CleaningAudio : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float maxVolume = 0.35f;

    [SerializeField, Min(0f)]
    private float activityTailSeconds = 0.08f;

    [SerializeField, Min(0.01f)]
    private float fadeSeconds = 0.1f;

    private DirtCleaner dirtCleaner;
    private AudioSource audioSource;

    private float previousProgress;
    private float lastCleaningTime = float.NegativeInfinity;

    private bool hasFocus = true;
    private bool isPaused;

    private void Awake()
    {
        dirtCleaner = GetComponent<DirtCleaner>();
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;

        StopImmediately();

        if (audioSource.clip == null)
        {
            Debug.LogError(
                "CleaningAudio: назначь клип в AudioSource.", this);
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        float progress = dirtCleaner.CleaningProgress;
        bool newDirtRemoved = progress > previousProgress;
        previousProgress = progress;

        if (!hasFocus || isPaused || !dirtCleaner.enabled)
        {
            StopImmediately();
            return;
        }

        if (newDirtRemoved && !dirtCleaner.IsCompleted)
            lastCleaningTime = Time.unscaledTime;

        bool active =
            !dirtCleaner.IsCompleted &&
            Time.unscaledTime - lastCleaningTime
                <= activityTailSeconds;

        float targetVolume = active ? maxVolume : 0f;

        if (active && !audioSource.isPlaying && maxVolume > 0f)
            audioSource.Play();

        float volumeStep =
            maxVolume * Time.unscaledDeltaTime
            / Mathf.Max(0.01f, fadeSeconds);

        audioSource.volume = Mathf.MoveTowards(
            audioSource.volume,
            targetVolume,
            volumeStep);

        if (!active && audioSource.volume <= 0f &&
            audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void StopImmediately()
    {
        lastCleaningTime = float.NegativeInfinity;

        if (audioSource == null)
            return;

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    private void OnApplicationFocus(bool focus)
    {
        hasFocus = focus;

        if (!focus)
            StopImmediately();
    }

    private void OnApplicationPause(bool paused)
    {
        isPaused = paused;

        if (paused)
            StopImmediately();
    }

    private void OnDisable()
    {
        StopImmediately();
    }
}