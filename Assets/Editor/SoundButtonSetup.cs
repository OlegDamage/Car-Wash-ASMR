using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SoundButtonSetup
{
    [MenuItem("Car Wash ASMR/Setup Sound Button")]
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

        Transform[] transforms = scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
            .ToArray();
        Transform[] sounds = transforms.Where(t => t.name == "SoundButton").ToArray();
        Transform[] restarts = transforms.Where(t => t.name == "RestartButton").ToArray();
        if (sounds.Length != 1 || restarts.Length != 1)
        {
            Debug.LogError("Нужны ровно один SoundButton и один RestartButton.");
            return;
        }

        GameObject sound = sounds[0].gameObject;
        Button button = sound.GetComponent<Button>();
        Image background = sound.GetComponent<Image>();
        RectTransform rect = sound.GetComponent<RectTransform>();
        SoundToggleUI toggle = sound.GetComponent<SoundToggleUI>();
        Button restartButton = restarts[0].GetComponent<Button>();
        Image restartImage = restarts[0].GetComponent<Image>();
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(
            "Assets/Art/Icons/UI/car_wash_ui_icons.png").OfType<Sprite>().ToArray();
        Sprite on = sprites.FirstOrDefault(s => s.name == "icon_sound_on");
        Sprite off = sprites.FirstOrDefault(s => s.name == "icon_sound_off");
        Transform existingIcon = sound.transform.Find("Icon");

        if (button == null || background == null || rect == null || toggle == null ||
            restartButton == null || restartImage == null || restartImage.sprite == null ||
            on == null || off == null ||
            (existingIcon != null && (existingIcon.GetComponent<Image>() == null ||
                                     existingIcon.GetComponent<RectTransform>() == null)))
        {
            Debug.LogError("Не найдены необходимые компоненты или спрайты. Ничего не изменено.");
            return;
        }

        SerializedObject settings = new SerializedObject(toggle);
        SerializedProperty source = settings.FindProperty("cleaningSource");
        if (source == null || source.objectReferenceValue == null)
        {
            Debug.LogError("У SoundToggleUI не назначен Cleaning Source. Ничего не изменено.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Setup Sound Button");
        Undo.RecordObjects(new Object[] { rect, background, button, toggle }, "Setup Sound Button");

        rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one;
        rect.anchoredPosition = new Vector2(-48, -48);
        rect.sizeDelta = new Vector2(144, 144);
        rect.localScale = Vector3.one;
        background.sprite = restartImage.sprite;
        background.color = Color.white;
        background.type = Image.Type.Simple;
        background.preserveAspect = true;
        background.raycastTarget = true;
        button.transition = Selectable.Transition.ColorTint;
        button.colors = restartButton.colors;
        button.targetGraphic = background;

        GameObject icon;
        if (existingIcon == null)
        {
            icon = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            Undo.RegisterCreatedObjectUndo(icon, "Create sound icon");
            Undo.SetTransformParent(icon.transform, sound.transform, "Parent sound icon");
        }
        else
        {
            icon = existingIcon.gameObject;
        }

        RectTransform iconRect = icon.GetComponent<RectTransform>();
        Image image = icon.GetComponent<Image>();
        Undo.RecordObjects(new Object[] { icon, iconRect, image }, "Configure sound icon");
        icon.SetActive(true);
        iconRect.anchorMin = iconRect.anchorMax = iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.localPosition = Vector3.zero;
        iconRect.anchoredPosition = Vector2.zero;
        iconRect.localRotation = Quaternion.identity;
        iconRect.localScale = Vector3.one;
        iconRect.sizeDelta = new Vector2(88, 88);
        image.sprite = on;
        image.color = Color.white;
        image.type = Image.Type.Simple;
        image.preserveAspect = true;
        image.raycastTarget = false;

        SerializedProperty label = settings.FindProperty("buttonLabel");
        Component labelComponent = label.objectReferenceValue as Component;
        if (labelComponent != null)
        {
            Undo.RecordObject(labelComponent.gameObject, "Hide sound label");
            labelComponent.gameObject.SetActive(false);
        }

        settings.FindProperty("buttonIcon").objectReferenceValue = image;
        settings.FindProperty("soundOnSprite").objectReferenceValue = on;
        settings.FindProperty("soundOffSprite").objectReferenceValue = off;
        settings.ApplyModifiedProperties();
        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(group);
        Selection.activeGameObject = sound;
        Debug.Log("SoundButton настроен. Сохрани сцену и проверь звук в Play Mode. On Click не изменён.", sound);
    }
}
