using TMPro;
using UnityEngine;

public class SoundToggleUI : MonoBehaviour
{
    private const string SoundEnabledKey =
        "CarWashASMR.SoundEnabled";

    [SerializeField] private AudioSource cleaningSource;
    [SerializeField] private TMP_Text buttonLabel;

    private bool soundEnabled;

    private void Awake()
    {
        if (cleaningSource == null || buttonLabel == null)
        {
            Debug.LogError(
                "SoundToggleUI: назначь AudioSource и текст кнопки.",
                this);

            enabled = false;
            return;
        }

        soundEnabled =
            PlayerPrefs.GetInt(SoundEnabledKey, 1) == 1;

        ApplyState();
    }

    public void ToggleSound()
    {
        if (!enabled)
            return;

        soundEnabled = !soundEnabled;
        ApplyState();

        PlayerPrefs.SetInt(
            SoundEnabledKey,
            soundEnabled ? 1 : 0);

        PlayerPrefs.Save();
    }

    private void ApplyState()
    {
        cleaningSource.mute = !soundEnabled;

        buttonLabel.text = soundEnabled
            ? "Sound: ON"
            : "Sound: OFF";
    }
}