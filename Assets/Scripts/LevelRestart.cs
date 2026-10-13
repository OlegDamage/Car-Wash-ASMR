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
            Debug.LogWarning(
                "LevelRestart: перезапуск доступен только в Play.", this);
            return;
        }

        if (restartRequested)
            return;

        Scene currentScene = SceneManager.GetActiveScene();

        if (string.IsNullOrEmpty(currentScene.path) ||
            !Application.CanStreamedLevelBeLoaded(currentScene.path))
        {
            Debug.LogError(
                "LevelRestart: добавь текущую сцену в Scene List " +
                "активного Build Profile.", this);
            return;
        }

        restartRequested = true;

        SceneManager.LoadScene(
            currentScene.path,
            LoadSceneMode.Single);
    }
}