using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRestart : MonoBehaviour
{
    private bool restartRequested;

    [ContextMenu("Restart Current Level")]
    public void RestartLevel()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("LevelRestart: перезапуск доступен только в Play.", this);
            return;
        }
        ShiftController shift = ShiftController.Instance;
        if (shift != null)
        {
            shift.RequestRestart();
            return;
        }
        ReloadScene();
    }

    public void ReloadForNextCar()
    {
        ShiftController shift = ShiftController.Instance;
        if (shift != null && !shift.CanRequestNextCar) return;
        ReloadScene();
    }

    private void ReloadScene()
    {
        if (!Application.isPlaying || restartRequested) return;
        Scene scene = SceneManager.GetActiveScene();
        if (string.IsNullOrEmpty(scene.path) || !Application.CanStreamedLevelBeLoaded(scene.path))
        {
            Debug.LogError("LevelRestart: добавь текущую сцену в Scene List активного Build Profile.", this);
            return;
        }
        restartRequested = true;
        SceneManager.LoadScene(scene.path, LoadSceneMode.Single);
    }
}
