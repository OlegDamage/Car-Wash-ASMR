using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ShiftClockUI : MonoBehaviour
{
    private TMP_Text label;
    private int lastMinute = -1;
    private int lastState = -1;

    private void Awake()
    {
        label = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        lastMinute = -1;
        lastState = -1;
    }

    private void LateUpdate()
    {
        ShiftController shift = ShiftController.Instance;
        if (shift == null)
        {
            if (lastState != 3)
            {
                label.text = "Clock unavailable";
                lastMinute = -1;
                lastState = 3;
            }
            return;
        }
        int minute = shift.CurrentMinute;
        int state = shift.IsFinished ? 2 : shift.IsPaused ? 1 : 0;
        if (minute == lastMinute && state == lastState)
            return;
        lastMinute = minute;
        lastState = state;
        string suffix = state == 2 ? "  |  Clock ended" :
            state == 1 ? "  |  Paused" : "";
        label.text = $"{minute / 60:00}:{minute % 60:00}{suffix}";
    }
}
