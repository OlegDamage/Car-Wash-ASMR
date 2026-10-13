using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class DirtCleaner : MonoBehaviour
{
    [SerializeField] private Camera inputCamera;
    [SerializeField, Min(1)] private int brushRadiusPixels = 24;

    [Header("Soft brush")]
    [SerializeField, Range(0.05f, 1f)]
    private float softEdgeFraction = 0.35f;

    [SerializeField, Min(0.1f)]
    private float cleaningStrengthPerSecond = 4f;

    [Header("Completion")]
    [SerializeField, Range(1f, 100f)]
    private float completionThresholdPercent = 95f;

    [Header("Progress — runtime debug")]
    [SerializeField, Range(0f, 100f)]
    private float cleaningPercent;

    [SerializeField] private bool isCompleted;

    private SpriteRenderer dirtRenderer;
    private Sprite originalSprite;
    private Sprite workingSprite;
    private Texture2D workingTexture;
    private Color32[] pixels;
    private float[] alphaValues;

    private long initialAlphaSum;
    private long removedAlphaSum;

    private float pixelsPerUnit;
    private Vector2 pivotPixels;
    private Vector2 previousPixelPosition;
    private bool wasPressed;

    private bool gestureActive;
    private bool gestureStartedOverUI;

    private readonly List<RaycastResult> uiHits =
        new List<RaycastResult>();

    public float CleaningProgress =>
        initialAlphaSum > 0
            ? (float)((double)removedAlphaSum / initialAlphaSum)
            : 0f;

    public bool IsCompleted => isCompleted;

    private void Awake()
    {
        dirtRenderer = GetComponent<SpriteRenderer>();
        originalSprite = dirtRenderer.sprite;

        if (inputCamera == null)
            inputCamera = Camera.main;

        if (inputCamera == null || originalSprite == null)
        {
            Debug.LogError(
                "DirtCleaner: назначь камеру и спрайт грязи.", this);
            enabled = false;
            return;
        }

        Texture2D source = originalSprite.texture;

        if (!source.isReadable)
        {
            Debug.LogError(
                "DirtCleaner: включи Read/Write у текстуры грязи.", this);
            enabled = false;
            return;
        }

        Rect rect = originalSprite.rect;
        int width = Mathf.RoundToInt(rect.width);
        int height = Mathf.RoundToInt(rect.height);

        Color[] sourcePixels = source.GetPixels(
            Mathf.RoundToInt(rect.x),
            Mathf.RoundToInt(rect.y),
            width,
            height);

        workingTexture = new Texture2D(
            width, height, TextureFormat.RGBA32, false);

        workingTexture.name = "Runtime Dirt";
        workingTexture.filterMode = source.filterMode;
        workingTexture.wrapMode = TextureWrapMode.Clamp;
        workingTexture.SetPixels(sourcePixels);
        workingTexture.Apply(false);

        pixels = workingTexture.GetPixels32();
        alphaValues = new float[pixels.Length];

        initialAlphaSum = 0;
        removedAlphaSum = 0;
        cleaningPercent = 0f;
        isCompleted = false;
        ResetGesture();

        for (int i = 0; i < pixels.Length; i++)
        {
            alphaValues[i] = pixels[i].a;
            initialAlphaSum += pixels[i].a;
        }

        if (initialAlphaSum == 0)
        {
            Debug.LogError(
                "DirtCleaner: текстура грязи полностью прозрачная.", this);
            enabled = false;
            return;
        }

        pixelsPerUnit = originalSprite.pixelsPerUnit;
        pivotPixels = originalSprite.pivot;

        Vector2 normalizedPivot = new Vector2(
            pivotPixels.x / width,
            pivotPixels.y / height);

        workingSprite = Sprite.Create(
            workingTexture,
            new Rect(0, 0, width, height),
            normalizedPivot,
            pixelsPerUnit,
            0,
            SpriteMeshType.FullRect);

        dirtRenderer.sprite = workingSprite;
    }

    private void Update()
    {
        if (isCompleted)
            return;

        if (!TryGetPointer(out Vector2 screenPosition))
        {
            ResetGesture();
            return;
        }

        if (!gestureActive)
        {
            gestureActive = true;
            gestureStartedOverUI = IsOverUI(screenPosition);
        }

        if (gestureStartedOverUI)
        {
            wasPressed = false;
            return;
        }

        Ray ray = inputCamera.ScreenPointToRay(screenPosition);
        Plane dirtPlane = new Plane(transform.forward, transform.position);

        if (!dirtPlane.Raycast(ray, out float distance))
        {
            wasPressed = false;
            return;
        }

        Vector3 localPosition =
            transform.InverseTransformPoint(ray.GetPoint(distance));

        Vector2 pixelPosition = new Vector2(
            localPosition.x * pixelsPerUnit + pivotPixels.x,
            localPosition.y * pixelsPerUnit + pivotPixels.y);

        Vector2 start = wasPressed
            ? previousPixelPosition
            : pixelPosition;

        float strokeLength = Vector2.Distance(start, pixelPosition);
        float spacing = Mathf.Max(1f, brushRadiusPixels * 0.25f);

        int sampleCount = wasPressed && strokeLength > 0.01f
            ? Mathf.Max(1, Mathf.CeilToInt(strokeLength / spacing))
            : 1;

        float alphaAmount =
            255f * Mathf.Max(0.1f, cleaningStrengthPerSecond)
            * Time.deltaTime / sampleCount;

        bool changed = false;

        for (int i = 0; i < sampleCount; i++)
        {
            float t = sampleCount == 1
                ? 1f
                : (i + 0.5f) / sampleCount;

            Vector2 point = Vector2.Lerp(start, pixelPosition, t);
            changed |= EraseCircle(point, alphaAmount);
        }

        if (changed)
        {
            cleaningPercent = CleaningProgress * 100f;

            float threshold = Mathf.Clamp(
                completionThresholdPercent, 1f, 100f);

            if (cleaningPercent >= threshold)
                CompleteCleaning();

            workingTexture.SetPixels32(pixels);
            workingTexture.Apply(false);
        }

        previousPixelPosition = pixelPosition;
        wasPressed = !isCompleted;
    }

    private bool IsOverUI(Vector2 screenPosition)
    {
        EventSystem eventSystem = EventSystem.current;

        if (eventSystem == null)
            return false;

        PointerEventData pointerData =
            new PointerEventData(eventSystem);

        pointerData.position = screenPosition;

        uiHits.Clear();
        eventSystem.RaycastAll(pointerData, uiHits);

        foreach (RaycastResult hit in uiHits)
        {
            if (hit.module is GraphicRaycaster)
                return true;
        }

        return false;
    }

    private bool TryGetPointer(out Vector2 position)
    {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
        {
            position =
                Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.isPressed)
        {
            position = Mouse.current.position.ReadValue();
            return true;
        }

        position = default;
        return false;
    }

    private bool EraseCircle(Vector2 center, float alphaAmount)
    {
        int radius = Mathf.Max(1, brushRadiusPixels);
        int width = workingTexture.width;
        int height = workingTexture.height;

        int minX = Mathf.Max(0, Mathf.FloorToInt(center.x - radius));
        int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(center.x + radius));
        int minY = Mathf.Max(0, Mathf.FloorToInt(center.y - radius));
        int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(center.y + radius));

        float radiusSquared = radius * radius;
        float softness = Mathf.Clamp(softEdgeFraction, 0.05f, 1f);
        float innerRadius = radius * (1f - softness);

        bool changed = false;

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float dx = x + 0.5f - center.x;
                float dy = y + 0.5f - center.y;
                float distanceSquared = dx * dx + dy * dy;

                if (distanceSquared >= radiusSquared)
                    continue;

                int index = y * width + x;
                byte oldAlpha = pixels[index].a;

                if (oldAlpha == 0)
                    continue;

                float distance = Mathf.Sqrt(distanceSquared);
                float edgePosition = Mathf.InverseLerp(
                    innerRadius, radius, distance);

                float weight =
                    1f - Mathf.SmoothStep(0f, 1f, edgePosition);

                alphaValues[index] = Mathf.Max(
                    0f,
                    alphaValues[index] - alphaAmount * weight);

                byte newAlpha =
                    (byte)Mathf.RoundToInt(alphaValues[index]);

                if (newAlpha == oldAlpha)
                    continue;

                pixels[index].a = newAlpha;
                removedAlphaSum += oldAlpha - newAlpha;
                changed = true;
            }
        }

        return changed;
    }

    private void CompleteCleaning()
    {
        if (isCompleted)
            return;

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].a = 0;
            alphaValues[i] = 0f;
        }

        removedAlphaSum = initialAlphaSum;
        cleaningPercent = 100f;
        isCompleted = true;
        ResetGesture();

        Debug.Log("DirtCleaner: очистка завершена.", this);
    }

    private void ResetGesture()
    {
        wasPressed = false;
        gestureActive = false;
        gestureStartedOverUI = false;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            ResetGesture();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            ResetGesture();
    }

    private void OnDisable()
    {
        ResetGesture();
    }

    private void OnDestroy()
    {
        if (dirtRenderer != null && workingSprite != null)
            dirtRenderer.sprite = originalSprite;

        if (workingSprite != null)
            Destroy(workingSprite);

        if (workingTexture != null)
            Destroy(workingTexture);
    }
}