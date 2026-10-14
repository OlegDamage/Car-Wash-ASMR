using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(LevelRestart))]
public class CarSequence : MonoBehaviour
{
    private const string CurrentCarKey =
        "CarWashASMR.CurrentCarIndex";

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
    private int currentCarIndex;
    private bool transitionRequested;

    private void Awake()
    {
        if (nextButton != null)
            nextButton.interactable = false;

        if (cleanRenderer == null ||
            dirtRenderer == null ||
            dirtCleaner == null ||
            nextButton == null ||
            cars == null ||
            cars.Length == 0)
        {
            Debug.LogError(
                "CarSequence: назначь компоненты и пары машин.", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i] == null ||
                cars[i].cleanSprite == null ||
                cars[i].dirtSprite == null)
            {
                Debug.LogError(
                    $"CarSequence: не заполнена пара машины {i}.", this);
                enabled = false;
                return;
            }
        }

        levelRestart = GetComponent<LevelRestart>();

        currentCarIndex = Mathf.Clamp(
            PlayerPrefs.GetInt(CurrentCarKey, 0),
            0,
            cars.Length - 1);

        cleanRenderer.sprite = cars[currentCarIndex].cleanSprite;
        dirtRenderer.sprite = cars[currentCarIndex].dirtSprite;
    }

    private void LateUpdate()
    {
        nextButton.interactable =
            dirtCleaner.IsCompleted && !transitionRequested;
    }

    public void NextCar()
    {
        if (!enabled ||
            transitionRequested ||
            !dirtCleaner.IsCompleted)
        {
            return;
        }

        Scene scene = SceneManager.GetActiveScene();

        if (string.IsNullOrEmpty(scene.path) ||
            !Application.CanStreamedLevelBeLoaded(scene.path))
        {
            Debug.LogError(
                "CarSequence: текущая сцена отсутствует в Scene List.",
                this);
            return;
        }

        transitionRequested = true;
        nextButton.interactable = false;

        int nextIndex = (currentCarIndex + 1) % cars.Length;

        PlayerPrefs.SetInt(CurrentCarKey, nextIndex);
        PlayerPrefs.Save();

        levelRestart.RestartLevel();
    }
}