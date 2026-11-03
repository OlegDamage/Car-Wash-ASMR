using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ShiftCounterUI : MonoBehaviour
{
    private TMP_Text label;
    private int lastCount = -1;
    private bool unavailable;

    private void Awake()
    {
        label = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        lastCount = -1;
        unavailable = false;
    }

    private void LateUpdate()
    {
        ShiftController shift = ShiftController.Instance;
        if (shift == null)
        {
            if (!unavailable)
            {
                label.text = "Cars washed: --";
                unavailable = true;
                lastCount = -1;
            }
            return;
        }
        if (!unavailable && lastCount == shift.CarsWashed)
            return;
        unavailable = false;
        lastCount = shift.CarsWashed;
        label.text = $"Cars washed: {lastCount}";
    }
}
