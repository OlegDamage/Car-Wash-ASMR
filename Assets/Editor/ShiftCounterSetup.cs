using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ShiftCounterSetup
{
    [MenuItem("Car Wash ASMR/Setup Shift Counter Prototype")]
    private static void Setup()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("Exit Play Mode first.");
            return;
        }
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != "Assets/Scenes/SampleScene.unity" || scene.isDirty)
        {
            Debug.LogError("Open and save SampleScene first.");
            return;
        }
        Transform[] all = scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true)).ToArray();
        Transform[] safeAreas = all.Where(t => t.name == "SafeArea").ToArray();
        TMP_Text source = all.Where(t => t.name == "ShiftClock")
            .Select(t => t.GetComponent<TMP_Text>()).FirstOrDefault(t => t != null);
        int controllers = all.Count(t => t.GetComponent<ShiftController>() != null);
        int cleaners = all.Count(t => t.GetComponent<DirtCleaner>() != null);
        bool alreadyPresent = all.Any(t => t.name == "ShiftCounter" ||
            t.GetComponent<ShiftCounterUI>() != null);
        if (safeAreas.Length != 1 || source == null || source.font == null ||
            controllers != 1 || cleaners != 1 || alreadyPresent)
        {
            Debug.LogError("Need one SafeArea, one ShiftController, one DirtCleaner, the existing ShiftClock font, and no existing ShiftCounter. No changes made.");
            return;
        }
        GameObject counter = new GameObject("ShiftCounter",
            typeof(RectTransform), typeof(CanvasRenderer),
            typeof(TextMeshProUGUI), typeof(ShiftCounterUI));
        Undo.RegisterCreatedObjectUndo(counter, "Create shift counter");
        SceneManager.MoveGameObjectToScene(counter, scene);
        counter.transform.SetParent(safeAreas[0], false);
        counter.layer = safeAreas[0].gameObject.layer;
        RectTransform rect = counter.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 260f);
        rect.sizeDelta = new Vector2(620f, 64f);
        TextMeshProUGUI label = counter.GetComponent<TextMeshProUGUI>();
        label.font = source.font;
        label.fontSize = 36f;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.raycastTarget = false;
        label.richText = false;
        label.text = "Cars washed: 0";
        EditorSceneManager.MarkSceneDirty(scene);
        Selection.activeGameObject = counter;
        Debug.Log("Counter label created. Inspect layout and save the scene. Existing clock, gameplay, saves and build settings were not changed by this setup.");
    }
}
