using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraWidthFit : MonoBehaviour
{
    [SerializeField, Min(0.01f)]
    private float referenceOrthographicSize = 8f;

    [SerializeField, Min(0.01f)]
    private float referenceAspect = 9f / 16f;

    private Camera targetCamera;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
        FitWidth();
    }

    private void LateUpdate()
    {
        FitWidth();
    }

    private void FitWidth()
    {
        if (!targetCamera.orthographic)
            return;

        float aspect = targetCamera.aspect;

        if (aspect <= 0f)
            return;

        targetCamera.orthographicSize =
            referenceOrthographicSize * referenceAspect / aspect;
    }
}