using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShiftEndUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text message;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text confirmLabel;
    [SerializeField] private Button cancelButton;
    private int lastMode = -1;
    private int lastCount = -1;

    private void Awake()
    {
        if (panel == null || message == null || confirmButton == null ||
            confirmLabel == null || cancelButton == null)
        {
            Debug.LogError("ShiftEndUI: assign all fields using Setup Shift End UI.", this);
            enabled = false;
            return;
        }
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        lastMode = -1;
        lastCount = -1;
        if (confirmButton != null) confirmButton.onClick.AddListener(Confirm);
        if (cancelButton != null) cancelButton.onClick.AddListener(Cancel);
    }

    private void OnDisable()
    {
        if (confirmButton != null) confirmButton.onClick.RemoveListener(Confirm);
        if (cancelButton != null) cancelButton.onClick.RemoveListener(Cancel);
        if (panel != null) panel.SetActive(false);
    }

    private void LateUpdate()
    {
        ShiftController shift = ShiftController.Instance;
        int mode = shift == null ? 0 : shift.IsFinished ? 2 :
            shift.RestartConfirmationRequested ? 1 : 0;
        int count = shift == null ? 0 : shift.CarsWashed;
        if (mode == lastMode && count == lastCount) return;
        lastMode = mode;
        lastCount = count;
        panel.SetActive(mode != 0);
        if (mode == 0) return;
        panel.transform.SetAsLastSibling();
        message.text = mode == 2 ? $"Shift finished\nCars washed: {count}\nSession result only - saving comes next" :
            "Restart the whole shift?\nCurrent shift progress will be lost.";
        confirmLabel.text = mode == 2 ? "Next shift" : "Restart shift";
        cancelButton.gameObject.SetActive(mode == 1);
    }

    private void Confirm()
    {
        if (ShiftController.Instance != null) ShiftController.Instance.StartNewShift();
    }
    private void Cancel()
    {
        if (ShiftController.Instance != null) ShiftController.Instance.CancelRestart();
    }
}
