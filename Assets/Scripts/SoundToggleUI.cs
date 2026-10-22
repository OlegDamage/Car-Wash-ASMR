using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggleUI : MonoBehaviour
{
    private const string SoundEnabledKey = "CarWashASMR.SoundEnabled";

    [SerializeField] private AudioSource cleaningSource;
    [SerializeField] private TMP_Text buttonLabel;
    [SerializeField] private Image buttonIcon;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;

    private bool soundEnabled;

    private void Awake()
    {
        if (cleaningSource == null)
        {
            Debug.LogError("SoundToggleUI: назначь AudioSource.", this);
            enabled = false;
            return;
        }

        if (buttonIcon != null &&
            (soundOnSprite == null || soundOffSprite == null))
        {
            Debug.LogError("SoundToggleUI: назначь оба спрайта звука.", this);
            enabled = false;
            return;
        }

        soundEnabled = PlayerPrefs.GetInt(SoundEnabledKey, 1) == 1;
        ApplyState();
    }

    public void ToggleSound()
    {
        if (!enabled)
            return;

        soundEnabled = !soundEnabled;
        ApplyState();
        PlayerPrefs.SetInt(SoundEnabledKey, soundEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyState()
    {
        cleaningSource.mute = !soundEnabled;

        if (buttonLabel != null)
            buttonLabel.text = soundEnabled ? "Sound: ON" : "Sound: OFF";

        if (buttonIcon != null)
            buttonIcon.sprite = soundEnabled ? soundOnSprite : soundOffSprite;
    }
}
