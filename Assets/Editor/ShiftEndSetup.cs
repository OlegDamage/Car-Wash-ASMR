using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ShiftEndSetup
{
    [MenuItem("Car Wash ASMR/Setup Shift End UI")]
    private static void Setup()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        { Debug.LogError("Exit Play Mode first."); return; }
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != "Assets/Scenes/SampleScene.unity" || scene.isDirty)
        { Debug.LogError("Open and save SampleScene first."); return; }
        Transform[] all = scene.GetRootGameObjects()
            .SelectMany(r => r.GetComponentsInChildren<Transform>(true)).ToArray();
        Transform[] safe = all.Where(t => t.name == "SafeArea").ToArray();
        TMP_Text fontSource = all.Where(t => t.name == "ShiftClock")
            .Select(t => t.GetComponent<TMP_Text>()).FirstOrDefault(t => t != null);
        if (safe.Length != 1 || fontSource == null || fontSource.font == null ||
            all.Count(t => t.GetComponent<ShiftController>() != null) != 1 ||
            all.Count(t => t.GetComponent<ShiftCounterUI>() != null) != 1 ||
            all.Any(t => t.GetComponent<ShiftEndUI>() != null || t.name == "ShiftEndRoot"))
        { Debug.LogError("Need existing clock/counter/controller and one SafeArea; ShiftEndRoot must not exist. No changes made."); return; }

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Setup Shift End UI");
        GameObject root = new GameObject("ShiftEndRoot", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(root, "Create shift end UI");
        SceneManager.MoveGameObjectToScene(root, scene);
        root.transform.SetParent(safe[0], false);
        root.layer = safe[0].gameObject.layer;
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero; rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = rootRect.offsetMax = Vector2.zero;
        GameObject panel = new GameObject("Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(root.transform, false); panel.layer = root.layer;
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero; panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = panelRect.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0.04f, 0.06f, 0.08f, 0.94f);
        panel.GetComponent<Image>().raycastTarget = true;
        TMP_Text text = MakeText(panel.transform, "Message", fontSource.font, new Vector2(0,160), new Vector2(900,300), 42);
        text.text = "Shift finished";
        Button confirm = MakeButton(panel.transform, "Confirm", fontSource.font, new Vector2(0,-80), "Next shift", out TMP_Text confirmText);
        Button cancel = MakeButton(panel.transform, "Cancel", fontSource.font, new Vector2(0,-230), "Continue washing", out TMP_Text unused);
        ShiftEndUI ui = root.AddComponent<ShiftEndUI>();
        SerializedObject settings = new SerializedObject(ui);
        settings.FindProperty("panel").objectReferenceValue = panel;
        settings.FindProperty("message").objectReferenceValue = text;
        settings.FindProperty("confirmButton").objectReferenceValue = confirm;
        settings.FindProperty("confirmLabel").objectReferenceValue = confirmText;
        settings.FindProperty("cancelButton").objectReferenceValue = cancel;
        settings.ApplyModifiedProperties();
        panel.SetActive(false);
        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(group);
        Selection.activeGameObject = root;
        Debug.Log("Shift end UI created and wired. Save scene, then test. No old UI was moved.");
    }

    private static TMP_Text MakeText(Transform parent, string name, TMP_FontAsset font, Vector2 position, Vector2 size, float fontSize)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false); go.layer = parent.gameObject.layer;
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f,0.5f);
        rect.anchoredPosition = position; rect.sizeDelta = size;
        TMP_Text label = go.GetComponent<TMP_Text>();
        label.font = font; label.fontSize = fontSize; label.color = Color.white;
        label.alignment = TextAlignmentOptions.Center; label.raycastTarget = false;
        label.richText = false;
        return label;
    }

    private static Button MakeButton(Transform parent, string name, TMP_FontAsset font, Vector2 position, string title, out TMP_Text label)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false); go.layer = parent.gameObject.layer;
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f,0.5f);
        rect.anchoredPosition = position; rect.sizeDelta = new Vector2(680,120);
        Image image = go.GetComponent<Image>(); image.color = new Color(0.25f,0.3f,0.33f,1f);
        Button button = go.GetComponent<Button>(); button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        label = MakeText(go.transform, "Label", font, Vector2.zero, new Vector2(650,110), 38);
        label.text = title;
        return button;
    }
}
