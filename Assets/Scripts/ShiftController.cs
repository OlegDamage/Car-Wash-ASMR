using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-200)]
public class ShiftController : MonoBehaviour
{
    public enum ShiftState { Working, FinishingLastCar, Finished }
    public static ShiftController Instance { get; private set; }
    [SerializeField, Range(0, 23)] private int startHour = 10;
    [SerializeField, Range(1, 24)] private int endHour = 18;
    [SerializeField, Min(1f)] private float shiftDurationSeconds = 300f;
    private double elapsedSeconds;
    private bool focused;
    private bool paused;
    private bool skipNextFrame = true;
    private bool loading;
    private bool cleanerBound;
    private DirtCleaner currentCleaner;
    private DirtCleaner overtimeCleaner;
    private bool cleanerInitiallyEnabled;
    private bool currentCarCounted;
    private string currentResultId;
    private ShiftSave.Statistics savedStatistics;
    private int lastShownCarIndex = -1;

    public ShiftState State { get; private set; }
    public int CarsWashed { get; private set; }
    public int CurrentCarIndex { get; private set; } = -1;
    public bool RestartConfirmationRequested { get; private set; }
    public bool IsResultSaved { get; private set; }
    public bool HasSavedStatistics => savedStatistics != null;
    public int CompletedShifts => savedStatistics == null ? 0 : savedStatistics.completedShifts;
    public long TotalCarsWashed => savedStatistics == null ? 0 : savedStatistics.totalCars;
    public int LastSavedShiftCars => savedStatistics == null ? 0 : savedStatistics.lastShiftCars;
    public bool IsFinished => State == ShiftState.Finished;
    public bool IsPaused => !focused || paused || RestartConfirmationRequested;
    public bool CanRequestNextCar => State == ShiftState.Working && !IsPaused && !loading;
    public int CurrentMinute
    {
        get
        {
            if (State != ShiftState.Working) return endHour * 60;
            double progress = elapsedSeconds / shiftDurationSeconds;
            return Mathf.Clamp(startHour * 60 +
                (int)(progress * (endHour - startHour) * 60),
                startHour * 60, endHour * 60);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState() { Instance = null; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        if (transform.parent != null)
        {
            Debug.LogError("ShiftController must be on a separate root object.", this);
            enabled = false;
            return;
        }
        ValidateSettings();
        Instance = this;
        focused = Application.isFocused;
        currentResultId = System.Guid.NewGuid().ToString("N");
        if (ShiftSave.TryLoad(out ShiftSave.Statistics data, out string error))
        {
            savedStatistics = data;
            Debug.Log($"Shift save loaded: shifts={CompletedShifts}, total cars={TotalCarsWashed}, last shift={LastSavedShiftCars}", this);
        }
        else Debug.LogError($"Shift save could not be loaded: {error}", this);
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public int GetOrSelectCarIndex(int carCount)
    {
        if (CurrentCarIndex < 0 || CurrentCarIndex >= carCount)
            CurrentCarIndex = CarSelection.Choose(carCount, lastShownCarIndex);
        lastShownCarIndex = CurrentCarIndex;
        return CurrentCarIndex;
    }

    private void Start()
    {
        if (Instance == this && !cleanerBound) BindCleaner(SceneManager.GetActiveScene());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance != this || mode != LoadSceneMode.Single) return;
        loading = false;
        skipNextFrame = true;
        BindCleaner(scene);
    }

    private void BindCleaner(Scene scene)
    {
        cleanerBound = true;
        currentCleaner = null;
        currentCarCounted = false;
        if (scene.path != "Assets/Scenes/SampleScene.unity") return;
        int count = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (DirtCleaner cleaner in root.GetComponentsInChildren<DirtCleaner>(true))
            { currentCleaner = cleaner; count++; }
        if (count != 1)
        {
            currentCleaner = null;
            Debug.LogError("ShiftController: expected exactly one DirtCleaner in SampleScene.", this);
            return;
        }
        cleanerInitiallyEnabled = currentCleaner.enabled;
        if (State == ShiftState.FinishingLastCar &&
            !object.ReferenceEquals(currentCleaner, overtimeCleaner)) FinishShift();
        ApplyInputState();
    }

    private void Update()
    {
        if (Instance != this) return;
        if (!IsPaused && !loading && State == ShiftState.Working)
        {
            if (skipNextFrame) skipNextFrame = false;
            else
            {
                elapsedSeconds = System.Math.Min(shiftDurationSeconds,
                    elapsedSeconds + Time.unscaledDeltaTime);
                if (elapsedSeconds >= shiftDurationSeconds) CloseAdmission();
            }
        }
        ApplyInputState();
    }

    private void CloseAdmission()
    {
        if (currentCleaner != null && currentCleaner.IsCompleted)
        {
            CountCurrentCar();
            FinishShift();
        }
        else if (currentCleaner != null && cleanerInitiallyEnabled &&
            currentCleaner.CleaningProgress >= 0.01f)
        {
            overtimeCleaner = currentCleaner;
            State = ShiftState.FinishingLastCar;
        }
        else FinishShift();
    }

    private void LateUpdate()
    {
        if (Instance != this || IsPaused || loading || IsFinished ||
            currentCleaner == null || !currentCleaner.isActiveAndEnabled ||
            !currentCleaner.IsCompleted) return;
        CountCurrentCar();
        if (State == ShiftState.FinishingLastCar) FinishShift();
    }

    private void CountCurrentCar()
    {
        if (currentCarCounted) return;
        currentCarCounted = true;
        CarsWashed++;
    }

    private void FinishShift()
    {
        if (IsFinished) return;
        State = ShiftState.Finished;
        overtimeCleaner = null;
        RestartConfirmationRequested = false;
        ApplyInputState();
        TrySaveCompletedResult();
    }

    public void TrySaveCompletedResult()
    {
        if (!IsFinished || IsResultSaved) return;
        if (ShiftSave.TryCommit(currentResultId, CarsWashed,
            out ShiftSave.Statistics data, out string error))
        {
            savedStatistics = data;
            IsResultSaved = true;
            Debug.Log($"Shift saved: shifts={CompletedShifts}, total cars={TotalCarsWashed}, last shift={LastSavedShiftCars}", this);
        }
        else Debug.LogError($"Shift save failed. Result is still in memory; use Retry save. {error}", this);
    }

    private void ApplyInputState()
    {
        if (currentCleaner == null) return;
        bool allowed = cleanerInitiallyEnabled && !IsPaused && !IsFinished && !loading;
        if (currentCleaner.enabled != allowed) currentCleaner.enabled = allowed;
        if (!allowed)
        {
            AudioSource source = currentCleaner.GetComponent<AudioSource>();
            if (source != null) { source.volume = 0f; source.Stop(); }
        }
    }

    public bool TryAdvanceCar(int nextIndex)
    {
        if (!CanRequestNextCar || nextIndex < 0 || currentCleaner == null ||
            !currentCleaner.IsCompleted) return false;
        CountCurrentCar();
        CurrentCarIndex = nextIndex;
        return true;
    }

    public void RequestRestart()
    {
        if (loading || IsFinished || !focused || paused) return;
        RestartConfirmationRequested = true;
        ApplyInputState();
    }

    public void CancelRestart()
    {
        RestartConfirmationRequested = false;
        skipNextFrame = true;
        ApplyInputState();
    }

    public void StartNewShift()
    {
        if (loading || (!IsFinished && !RestartConfirmationRequested)) return;
        if (IsFinished && !IsResultSaved)
        {
            TrySaveCompletedResult();
            if (!IsResultSaved) return;
        }
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != "Assets/Scenes/SampleScene.unity" ||
            !Application.CanStreamedLevelBeLoaded(scene.path))
        {
            Debug.LogError("Cannot start shift: SampleScene is not available in Scene List.", this);
            return;
        }
        loading = true;
        ApplyInputState();
        elapsedSeconds = 0d;
        CarsWashed = 0;
        CurrentCarIndex = -1;
        currentCarCounted = false;
        overtimeCleaner = null;
        RestartConfirmationRequested = false;
        IsResultSaved = false;
        currentResultId = System.Guid.NewGuid().ToString("N");
        State = ShiftState.Working;
        skipNextFrame = true;
        SceneManager.LoadScene(scene.path, LoadSceneMode.Single);
    }

    private void OnApplicationFocus(bool hasFocus)
    { focused = hasFocus; skipNextFrame = true; ApplyInputState(); }
    private void OnApplicationPause(bool isPaused)
    { paused = isPaused; skipNextFrame = true; ApplyInputState(); }
    private void OnValidate() { ValidateSettings(); }

    private void ValidateSettings()
    {
        startHour = Mathf.Clamp(startHour, 0, 23);
        endHour = Mathf.Clamp(endHour, startHour + 1, 24);
        if (float.IsNaN(shiftDurationSeconds) || float.IsInfinity(shiftDurationSeconds))
            shiftDurationSeconds = 300f;
        shiftDurationSeconds = Mathf.Max(1f, shiftDurationSeconds);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;
    }
}
