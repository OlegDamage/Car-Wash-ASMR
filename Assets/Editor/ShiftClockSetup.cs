using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ShiftClockSetup
{
    [MenuItem("Car Wash ASMR/Setup Shift Clock Prototype")]
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
        TMP_Text source = all.Select(t => t.GetComponent<TMP_Text>())
            .FirstOrDefault(t => t != null && t.font != null);
        bool alreadyPresent = all.Any(t =>
            t.GetComponent<ShiftController>() != null ||
            t.GetComponent<ShiftClockUI>() != null ||
            t.name == "ShiftController" || t.name == "ShiftClock");
        if (safeAreas.Length != 1 || source == null || alreadyPresent)
        {
            Debug.LogError("Need one SafeArea, an existing TMP font, and no existing shift clock. No changes made.");
            return;
        }
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Setup Shift Clock Prototype");
        GameObject controller = new GameObject("ShiftController", typeof(ShiftController));
        Undo.RegisterCreatedObjectUndo(controller, "Create shift controller");
        SceneManager.MoveGameObjectToScene(controller, scene);

        GameObject clock = new GameObject("ShiftClock",
            typeof(RectTransform), typeof(CanvasRenderer),
            typeof(TextMeshProUGUI), typeof(ShiftClockUI));
        Undo.RegisterCreatedObjectUndo(clock, "Create clock label");
        SceneManager.MoveGameObjectToScene(clock, scene);
        clock.transform.SetParent(safeAreas[0], false);
        clock.layer = safeAreas[0].gameObject.layer;
        RectTransform rect = clock.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -240f);
        rect.sizeDelta = new Vector2(620f, 64f);
        TextMeshProUGUI label = clock.GetComponent<TextMeshProUGUI>();
        label.font = source.font;
        label.fontSize = 36f;
        label.alignment = TextAlignmentOptions.Center;
        label.color = new Color(0.19f, 0.23f, 0.25f, 1f);
        label.raycastTarget = false;
        label.richText = false;
        label.text = "10:00";
        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(undoGroup);
        Selection.activeGameObject = controller;
        Debug.Log("Clock objects created. Inspect their layout, then save the scene. No gameplay, persistence or build settings were changed.");
    }
}
