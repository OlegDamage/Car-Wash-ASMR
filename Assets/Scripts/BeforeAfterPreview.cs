using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(-50)]
[RequireComponent(typeof(Button))]
public class BeforeAfterPreview : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    [SerializeField] private DirtCleaner dirtCleaner;
    [SerializeField] private SpriteRenderer sourceDirtRenderer;
    [SerializeField] private SpriteRenderer previewRenderer;

    private Button previewButton;
    private bool initialized;
    private bool holding;
    private int holdingPointerId;

    private void Awake()
    {
        previewButton = GetComponent<Button>();
        previewButton.interactable = false;
        HidePreview();

        if (dirtCleaner == null ||
            sourceDirtRenderer == null ||
            previewRenderer == null ||
            sourceDirtRenderer == previewRenderer ||
            sourceDirtRenderer.sprite == null)
        {
            Debug.LogError(
                "BeforeAfterPreview: назначь DirtCleaner, " +
                "исходный SpriteRenderer и отдельный слой сравнения.",
                this);

            enabled = false;
            return;
        }

        previewRenderer.sprite = sourceDirtRenderer.sprite;
        previewRenderer.sharedMaterial =
            sourceDirtRenderer.sharedMaterial;
        previewRenderer.color = sourceDirtRenderer.color;
        previewRenderer.flipX = sourceDirtRenderer.flipX;
        previewRenderer.flipY = sourceDirtRenderer.flipY;
        previewRenderer.sortingLayerID =
            sourceDirtRenderer.sortingLayerID;
        previewRenderer.sortingOrder =
            sourceDirtRenderer.sortingOrder + 1;

        initialized = true;
    }

    private void LateUpdate()
    {
        if (!initialized)
            return;

        bool canPreview =
            dirtCleaner.isActiveAndEnabled &&
            dirtCleaner.IsCompleted;

        previewButton.interactable = canPreview;

        if (!canPreview)
            HidePreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!initialized ||
            !isActiveAndEnabled ||
            holding ||
            eventData.button != PointerEventData.InputButton.Left ||
            !previewButton.IsInteractable() ||
            !dirtCleaner.IsCompleted)
        {
            return;
        }

        holding = true;
        holdingPointerId = eventData.pointerId;
        previewRenderer.enabled = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (holding && eventData.pointerId == holdingPointerId)
            HidePreview();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (holding && eventData.pointerId == holdingPointerId)
            HidePreview();
    }

    private void HidePreview()
    {
        holding = false;

        if (previewRenderer != null)
            previewRenderer.enabled = false;
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused)
            HidePreview();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            HidePreview();
    }

    private void OnDisable()
    {
        HidePreview();

        if (previewButton != null)
            previewButton.interactable = false;
    }
}