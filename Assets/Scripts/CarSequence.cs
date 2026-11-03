using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(LevelRestart))]
public class CarSequence : MonoBehaviour
{
    private const string CurrentCarKey = "CarWashASMR.CurrentCarIndex";
    [Serializable]
    private class CarPair
    {
        public Sprite cleanSprite;
        public Sprite dirtSprite;
    }
    [SerializeField] private SpriteRenderer cleanRenderer;
    [SerializeField] private SpriteRenderer dirtRenderer;
    [SerializeField] private DirtCleaner dirtCleaner;
    [SerializeField] private Button nextButton;
    [SerializeField] private CarPair[] cars;
    private LevelRestart levelRestart;
    private CanvasGroup nextGroup;
    private int currentCarIndex;
    private bool transitionRequested;

    private void Awake()
    {
        if (nextButton != null)
        {
            nextGroup = nextButton.GetComponent<CanvasGroup>();
            if (nextGroup == null) nextGroup = nextButton.gameObject.AddComponent<CanvasGroup>();
            SetNextVisible(false);
        }
        if (cleanRenderer == null || dirtRenderer == null || dirtCleaner == null ||
            nextButton == null || cars == null || cars.Length == 0)
        {
            Debug.LogError("CarSequence: назначь компоненты и пары машин.", this);
            enabled = false;
            return;
        }
        for (int i = 0; i < cars.Length; i++)
            if (cars[i] == null || cars[i].cleanSprite == null || cars[i].dirtSprite == null)
            {
                Debug.LogError($"CarSequence: не заполнена пара машины {i}.", this);
                enabled = false;
                return;
            }
        levelRestart = GetComponent<LevelRestart>();
        ShiftController shift = ShiftController.Instance;
        currentCarIndex = shift != null ? shift.GetOrSelectCarIndex(cars.Length) :
            Mathf.Clamp(PlayerPrefs.GetInt(CurrentCarKey, 0), 0, cars.Length - 1);
        cleanRenderer.sprite = cars[currentCarIndex].cleanSprite;
        dirtRenderer.sprite = cars[currentCarIndex].dirtSprite;
    }

    private void LateUpdate()
    {
        ShiftController shift = ShiftController.Instance;
        SetNextVisible(dirtCleaner.isActiveAndEnabled && dirtCleaner.IsCompleted &&
            !transitionRequested && (shift == null || shift.CanRequestNextCar));
    }

    private void SetNextVisible(bool visible)
    {
        if (nextButton != null) nextButton.interactable = visible;
        if (nextGroup == null) return;
        nextGroup.alpha = visible ? 1f : 0f;
        nextGroup.interactable = visible;
        nextGroup.blocksRaycasts = visible;
    }

    public void NextCar()
    {
        if (!enabled || transitionRequested || !dirtCleaner.IsCompleted) return;
        ShiftController shift = ShiftController.Instance;
        if (shift != null && !shift.CanRequestNextCar) return;
        Scene scene = SceneManager.GetActiveScene();
        if (string.IsNullOrEmpty(scene.path) || !Application.CanStreamedLevelBeLoaded(scene.path))
        {
            Debug.LogError("CarSequence: текущая сцена отсутствует в Scene List.", this);
            return;
        }
        int nextIndex = CarSelection.Choose(cars.Length, currentCarIndex);
        if (shift != null)
        {
            if (!shift.TryAdvanceCar(nextIndex)) return;
        }
        else
        {
            PlayerPrefs.SetInt(CurrentCarKey, nextIndex);
            PlayerPrefs.Save();
        }
        transitionRequested = true;
        SetNextVisible(false);
        levelRestart.ReloadForNextCar();
    }

    private void OnDisable() { SetNextVisible(false); }
}
