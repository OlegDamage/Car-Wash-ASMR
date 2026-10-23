using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class CleaningProgressUI : MonoBehaviour
{
    [SerializeField] private DirtCleaner dirtCleaner;
    [SerializeField] private Slider progressBar;

    private TMP_Text progressText;
    private int lastPercent = -1;

    private void Awake()
    {
        progressText = GetComponent<TMP_Text>();
        progressText.raycastTarget = false;
        progressText.text = "0%";
        if (progressBar != null)
            progressBar.SetValueWithoutNotify(0f);

        if (dirtCleaner == null)
        {
            Debug.LogError("CleaningProgressUI: назначь DirtCleaner.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        lastPercent = -1;
    }

    private void LateUpdate()
    {
        float progress = dirtCleaner.IsCompleted
            ? 1f : Mathf.Clamp01(dirtCleaner.CleaningProgress);
        if (progressBar != null)
            progressBar.SetValueWithoutNotify(progress);

        int percent = Mathf.FloorToInt(progress * 100f);
        if (percent == lastPercent)
            return;

        progressText.text = $"{percent}%";
        lastPercent = percent;
    }
}
