using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaUI : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea;
    private Vector2Int lastScreenSize;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void Update()
    {
        Vector2Int screenSize =
            new Vector2Int(Screen.width, Screen.height);

        if (Screen.safeArea != lastSafeArea ||
            screenSize != lastScreenSize)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        if (Screen.width <= 0 || Screen.height <= 0)
            return;

        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.min;
        Vector2 anchorMax = safeArea.max;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        lastSafeArea = safeArea;
        lastScreenSize =
            new Vector2Int(Screen.width, Screen.height);
    }
}