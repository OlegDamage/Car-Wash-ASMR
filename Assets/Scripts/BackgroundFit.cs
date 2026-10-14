using UnityEngine;

[DefaultExecutionOrder(100)]
[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundFit : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    private SpriteRenderer backgroundRenderer;

    private void Awake()
    {
        backgroundRenderer = GetComponent<SpriteRenderer>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null ||
            backgroundRenderer.sprite == null)
        {
            Debug.LogError(
                "BackgroundFit: назначь камеру и спрайт фона.", this);
            enabled = false;
            return;
        }

        if (!targetCamera.orthographic)
        {
            Debug.LogError(
                "BackgroundFit: нужна ортографическая камера.", this);
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        Vector2 spriteSize = backgroundRenderer.sprite.bounds.size;

        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            return;

        float viewHeight = targetCamera.orthographicSize * 2f;
        float viewWidth = viewHeight * targetCamera.aspect;

        float scale = Mathf.Max(
            viewWidth / spriteSize.x,
            viewHeight / spriteSize.y);

        transform.localScale = new Vector3(scale, scale, 1f);

        Vector3 cameraPosition = targetCamera.transform.position;

        transform.position = new Vector3(
            cameraPosition.x,
            cameraPosition.y,
            transform.position.z);
    }
}