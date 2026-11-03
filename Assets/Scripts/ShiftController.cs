using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-200)]
public class ShiftController : MonoBehaviour
{
    public static ShiftController Instance { get; private set; }

    [SerializeField, Range(0, 23)] private int startHour = 10;
    [SerializeField, Range(1, 24)] private int endHour = 18;
    [SerializeField, Min(1f)] private float shiftDurationSeconds = 300f;

    private double elapsedSeconds;
    private bool focused;
    private bool paused;
    private bool skipNextFrame = true;
    private DirtCleaner currentCleaner;
    private bool currentCarCounted;

    public int CarsWashed { get; private set; }
    public bool IsFinished => elapsedSeconds >= shiftDurationSeconds;
    public bool IsPaused => !focused || paused;
    public int CurrentMinute
    {
        get
        {
            if (IsFinished)
                return endHour * 60;
            double progress = elapsedSeconds / shiftDurationSeconds;
            int minute = startHour * 60 +
                (int)(progress * (endHour - startHour) * 60);
            return Mathf.Clamp(minute, startHour * 60, endHour * 60);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        Instance = null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        if (transform.parent != null)
        {
            Debug.LogError("ShiftController must be on a separate root object.", this);
            enabled = false;
            return;
        }
        ValidateSettings();
        Instance = this;
        focused = Application.isFocused;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (Instance == this && currentCleaner == null)
            BindCleaner(SceneManager.GetActiveScene());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance == this && mode == LoadSceneMode.Single)
            BindCleaner(scene);
    }

    private void BindCleaner(Scene scene)
    {
        currentCleaner = null;
        currentCarCounted = false;
        if (scene.path != "Assets/Scenes/SampleScene.unity")
            return;
        int count = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (DirtCleaner cleaner in root.GetComponentsInChildren<DirtCleaner>(true))
            {
                currentCleaner = cleaner;
                count++;
            }
        }
        if (count != 1)
        {
            currentCleaner = null;
            Debug.LogError("ShiftController: expected exactly one DirtCleaner in SampleScene. Counting disabled for this scene.", this);
        }
    }

    private void Update()
    {
        if (Instance != this || IsPaused || IsFinished)
            return;
        if (skipNextFrame)
        {
            skipNextFrame = false;
            return;
        }
        elapsedSeconds = System.Math.Min(shiftDurationSeconds,
            elapsedSeconds + Time.unscaledDeltaTime);
        if (IsFinished)
            Debug.Log("Shift clock reached closing time. Car count frozen; gameplay blocking is pending.", this);
    }

    private void LateUpdate()
    {
        if (Instance != this || IsPaused || IsFinished || currentCarCounted ||
            currentCleaner == null || !currentCleaner.isActiveAndEnabled ||
            !currentCleaner.IsCompleted)
            return;
        currentCarCounted = true;
        CarsWashed++;
        Debug.Log($"Shift: washed cars = {CarsWashed}", this);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        focused = hasFocus;
        skipNextFrame = true;
    }

    private void OnApplicationPause(bool isPaused)
    {
        paused = isPaused;
        skipNextFrame = true;
    }

    private void OnValidate()
    {
        ValidateSettings();
    }

    private void ValidateSettings()
    {
        startHour = Mathf.Clamp(startHour, 0, 23);
        endHour = Mathf.Clamp(endHour, startHour + 1, 24);
        if (float.IsNaN(shiftDurationSeconds) ||
            float.IsInfinity(shiftDurationSeconds))
            shiftDurationSeconds = 300f;
        shiftDurationSeconds = Mathf.Max(1f, shiftDurationSeconds);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this)
            Instance = null;
    }
}
