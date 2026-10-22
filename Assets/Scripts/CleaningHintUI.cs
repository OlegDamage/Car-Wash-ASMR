using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CleaningHintUI : MonoBehaviour
{
    [SerializeField] private DirtCleaner dirtCleaner;

    private TextMeshProUGUI hintLabel;
    private int displayedState = -1;

    private void Awake()
    {
        hintLabel = GetComponent<TextMeshProUGUI>();
        hintLabel.raycastTarget = false;

        if (dirtCleaner == null)
        {
            Debug.LogError("CleaningHintUI: назначь DirtCleaner с объекта CarDirt.", this);
            hintLabel.enabled = false;
            enabled = false;
        }
    }

    private void OnEnable()
    {
        displayedState = -1;
    }

    private void Start()
    {
        if (enabled)
            RefreshHint();
    }

    private void LateUpdate()
    {
        RefreshHint();
    }

    private void RefreshHint()
    {
        int state;
        if (!dirtCleaner.isActiveAndEnabled)
            state = 1;
        else if (dirtCleaner.IsCompleted)
            state = 2;
        else if (dirtCleaner.CleaningProgress > 0f)
            state = 1;
        else
            state = 0;

        if (state == displayedState)
            return;

        displayedState = state;
        if (state == 1)
        {
            hintLabel.enabled = false;
            return;
        }

        hintLabel.text = state == 2
            ? "Hold Before to compare" : "Swipe to clean";
        hintLabel.enabled = true;
    }

    private void OnDisable()
    {
        if (hintLabel != null)
            hintLabel.enabled = false;
    }
}
