using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class CleaningProgressUI : MonoBehaviour
{
    [SerializeField] private DirtCleaner dirtCleaner;

    private TMP_Text progressText;
    private int lastPercent = -1;
    private bool lastCompleted;
    private bool hasDisplayedValue;

    private void Awake()
    {
        progressText = GetComponent<TMP_Text>();

        if (dirtCleaner == null)
        {
            Debug.LogError(
                "CleaningProgressUI: назначь DirtCleaner.", this);
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        bool completed = dirtCleaner.IsCompleted;

        int percent = completed
            ? 100
            : Mathf.FloorToInt(
                Mathf.Clamp01(dirtCleaner.CleaningProgress) * 100f);

        if (hasDisplayedValue &&
            percent == lastPercent &&
            completed == lastCompleted)
        {
            return;
        }

        progressText.text = completed
            ? "Clean! 100%"
            : $"Cleaning: {percent}%";

        lastPercent = percent;
        lastCompleted = completed;
        hasDisplayedValue = true;
    }
}