using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class DirtCleaner : MonoBehaviour
{
    [SerializeField] private Camera inputCamera;
    [SerializeField, Min(1)] private int brushRadiusPixels = 24;

    [Header("Progress — runtime debug")]
    [SerializeField, Range(0f, 100f)]
    private float cleaningPercent;

    private SpriteRenderer dirtRenderer;
    private Sprite originalSprite;
    private Sprite workingSprite;
    private Texture2D workingTexture;
    private Color32[] pixels;

    private int initialDirtyPixelCount;
    private int cleanedPixelCount;

    private float pixelsPerUnit;
    private Vector2 pivotPixels;
    private Vector2 previousPixelPosition;
    private bool wasPressed;

    public float CleaningProgress =>
        initialDirtyPixelCount > 0
            ? (float)cleanedPixelCount / initialDirtyPixelCount
            : 0f;

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

        initialDirtyPixelCount = 0;
        cleanedPixelCount = 0;
        cleaningPercent = 0f;

        foreach (Color32 pixel in pixels)
        {
            if (pixel.a > 0)
                initialDirtyPixelCount++;
        }

        if (initialDirtyPixelCount == 0)
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
        if (!TryGetPointer(out Vector2 screenPosition))
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

        float spacing = Mathf.Max(1f, brushRadiusPixels * 0.5f);
        int steps = Mathf.Max(
            1,
            Mathf.CeilToInt(Vector2.Distance(start, pixelPosition) / spacing));

        bool changed = false;

        for (int i = 0; i <= steps; i++)
        {
            Vector2 point = Vector2.Lerp(
                start, pixelPosition, (float)i / steps);

            changed |= EraseCircle(point);
        }

        if (changed)
        {
            workingTexture.SetPixels32(pixels);
            workingTexture.Apply(false);

            cleaningPercent = CleaningProgress * 100f;
        }

        previousPixelPosition = pixelPosition;
        wasPressed = true;
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

    private bool EraseCircle(Vector2 center)
    {
        int radius = Mathf.Max(1, brushRadiusPixels);
        int width = workingTexture.width;
        int height = workingTexture.height;

        int minX = Mathf.Max(0, Mathf.FloorToInt(center.x - radius));
        int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(center.x + radius));
        int minY = Mathf.Max(0, Mathf.FloorToInt(center.y - radius));
        int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(center.y + radius));

        float radiusSquared = radius * radius;
        bool changed = false;

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float dx = x + 0.5f - center.x;
                float dy = y + 0.5f - center.y;

                if (dx * dx + dy * dy > radiusSquared)
                    continue;

                int index = y * width + x;

                if (pixels[index].a == 0)
                    continue;

                pixels[index].a = 0;
                cleanedPixelCount++;
                changed = true;
            }
        }

        return changed;
    }

    private void OnDisable()
    {
        wasPressed = false;
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