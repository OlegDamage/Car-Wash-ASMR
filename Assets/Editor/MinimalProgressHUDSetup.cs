using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MinimalProgressHUDSetup
{
    [MenuItem("Car Wash ASMR/Setup Minimal Progress HUD")]
    private static void Setup()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("Сначала выйди из Play Mode.");
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != "Assets/Scenes/SampleScene.unity")
        {
            Debug.LogError("Открой SampleScene и сделай её активной.");
            return;
        }

        Transform[] all = scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true)).ToArray();
        Transform[] progressMatches = all.Where(t => t.name == "CleaningProgressText").ToArray();
        Transform[] hintMatches = all.Where(t => t.name == "CleaningHintText").ToArray();
        if (progressMatches.Length != 1 || hintMatches.Length != 1)
        {
            Debug.LogError("Нужны единственные CleaningProgressText и CleaningHintText. Ничего не изменено.");
            return;
        }

        TMP_Text percent = progressMatches[0].GetComponent<TMP_Text>();
        TMP_Text hint = hintMatches[0].GetComponent<TMP_Text>();
        CleaningProgressUI progressUI = progressMatches[0].GetComponent<CleaningProgressUI>();
        CleaningHintUI hintUI = hintMatches[0].GetComponent<CleaningHintUI>();
        Transform parent = progressMatches[0].parent;
        Sprite rounded = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        if (percent == null || hint == null || progressUI == null || hintUI == null ||
            parent == null || parent.name != "SafeArea" || hintMatches[0].parent != parent ||
            rounded == null)
        {
            Debug.LogError("Проверь текстовые компоненты, их скрипты, SafeArea и встроенный UI-спрайт. Ничего не изменено.");
            return;
        }

        if (parent.Find("CleaningProgressBar") != null)
        {
            Debug.LogWarning("CleaningProgressBar уже существует. Повторная настройка не выполнена.");
            return;
        }

        SerializedObject settings = new SerializedObject(progressUI);
        SerializedObject hintSettings = new SerializedObject(hintUI);
        if (settings.FindProperty("dirtCleaner").objectReferenceValue == null ||
            hintSettings.FindProperty("dirtCleaner").objectReferenceValue == null ||
            settings.FindProperty("progressBar") == null)
        {
            Debug.LogError("Проверь ссылки DirtCleaner и дождись компиляции новых скриптов. Ничего не изменено.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Setup Minimal Progress HUD");
        Undo.RecordObjects(new Object[] { percent, percent.rectTransform, hint, hint.rectTransform },
            "Layout progress text");
        SetRect(percent.rectTransform, 40f, 560f, 72f);
        SetRect(hint.rectTransform, 156f, 660f, 48f);
        percent.enableAutoSizing = false;
        percent.fontSize = 56f;
        percent.alignment = TextAlignmentOptions.Center;
        percent.color = Color.white;
        percent.raycastTarget = false;
        percent.text = "0%";
        hint.enableAutoSizing = false;
        hint.fontSize = 32f;
        hint.alignment = TextAlignmentOptions.Center;
        hint.color = new Color(0.88f, 0.9f, 0.9f, 1f);
        hint.raycastTarget = false;
        hint.text = "Swipe to clean";
        hint.enabled = true;

        GameObject bar = new GameObject("CleaningProgressBar",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Slider));
        Undo.RegisterCreatedObjectUndo(bar, "Create progress bar");
        Undo.SetTransformParent(bar.transform, parent, "Parent progress bar");
        bar.layer = parent.gameObject.layer;
        SetRect(bar.GetComponent<RectTransform>(), 124f, 560f, 20f);
        Image track = bar.GetComponent<Image>();
        track.sprite = rounded;
        track.type = Image.Type.Sliced;
        track.color = new Color(0.75f, 0.8f, 0.8f, 0.18f);
        track.raycastTarget = false;

        GameObject fill = new GameObject("Fill",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(fill, "Create progress fill");
        Undo.SetTransformParent(fill.transform, bar.transform, "Parent progress fill");
        fill.layer = bar.layer;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.localPosition = Vector3.zero;
        fillRect.localRotation = Quaternion.identity;
        fillRect.localScale = Vector3.one;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = fillRect.offsetMax = Vector2.zero;
        Image fillImage = fill.GetComponent<Image>();
        fillImage.sprite = rounded;
        fillImage.type = Image.Type.Sliced;
        fillImage.color = new Color(0.55f, 0.77f, 0.7f, 1f);
        fillImage.raycastTarget = false;

        Slider slider = bar.GetComponent<Slider>();
        slider.transition = Selectable.Transition.None;
        slider.interactable = false;
        slider.navigation = new Navigation { mode = Navigation.Mode.None };
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.direction = Slider.Direction.LeftToRight;
        slider.fillRect = fillRect;
        slider.handleRect = null;
        slider.SetValueWithoutNotify(0f);
        settings.FindProperty("progressBar").objectReferenceValue = slider;
        settings.ApplyModifiedProperties();
        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(group);
        Selection.activeGameObject = bar;
        Debug.Log("Минималистичный прогресс настроен. Сохрани сцену, проверь 0%, очистку, 100% и сброс. Кнопки и фон не изменены.");
    }

    private static void SetRect(RectTransform rect, float top, float width, float height)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.localRotation = Quaternion.identity;
        rect.localScale = Vector3.one;
        rect.anchoredPosition3D = new Vector3(0f, -top, 0f);
        rect.sizeDelta = new Vector2(width, height);
    }
}
