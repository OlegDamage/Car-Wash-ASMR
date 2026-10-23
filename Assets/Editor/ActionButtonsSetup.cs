using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ActionButtonsSetup
{
    [MenuItem("Car Wash ASMR/Setup Before and Next Buttons")]
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

        Transform[] objects = scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true)).ToArray();
        string[] names = { "RestartButton", "BeforeButton", "NextButton" };
        GameObject[] targets = new GameObject[3];
        for (int i = 0; i < names.Length; i++)
        {
            Transform[] matches = objects.Where(t => t.name == names[i]).ToArray();
            if (matches.Length != 1)
            {
                Debug.LogError("Нужен ровно один объект " + names[i] + ". Ничего не изменено.");
                return;
            }
            targets[i] = matches[0].gameObject;
        }

        Button sourceButton = targets[0].GetComponent<Button>();
        Image sourceImage = targets[0].GetComponent<Image>();
        Button[] buttons = new Button[2];
        Image[] images = new Image[2];
        TMP_Text[] labels = new TMP_Text[2];
        if (sourceButton == null || sourceImage == null || sourceImage.sprite == null)
        {
            Debug.LogError("Проверь компоненты и подложку RestartButton. Ничего не изменено.");
            return;
        }

        for (int i = 0; i < 2; i++)
        {
            buttons[i] = targets[i + 1].GetComponent<Button>();
            images[i] = targets[i + 1].GetComponent<Image>();
            TMP_Text[] found = targets[i + 1].GetComponentsInChildren<TMP_Text>(true);
            if (buttons[i] == null || images[i] == null || found.Length != 1 ||
                !targets[i + 1].activeInHierarchy)
            {
                Debug.LogError("Проверь активность объекта, Button, Image и TMP-надпись у " + names[i + 1] + ". Ничего не изменено.");
                return;
            }
            labels[i] = found[0];
        }
        if (targets[1].GetComponent<BeforeAfterPreview>() == null)
        {
            Debug.LogError("На BeforeButton отсутствует BeforeAfterPreview. Ничего не изменено.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Setup Before and Next Buttons");
        for (int i = 0; i < 2; i++)
        {
            Undo.RecordObjects(new Object[] { buttons[i], images[i], labels[i] },
                "Setup Before and Next Buttons");
            images[i].sprite = sourceImage.sprite;
            images[i].color = Color.white;
            images[i].type = Image.Type.Simple;
            images[i].preserveAspect = false;
            images[i].raycastTarget = true;
            buttons[i].transition = Selectable.Transition.ColorTint;
            buttons[i].colors = sourceButton.colors;
            buttons[i].targetGraphic = images[i];
            labels[i].color = Color.white;
            labels[i].raycastTarget = false;

            CanvasGroup canvasGroup = targets[i + 1].GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = Undo.AddComponent<CanvasGroup>(targets[i + 1]);
            Undo.RecordObject(canvasGroup, "Hide action button until cleaning completes");
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(group);
        Selection.activeGameObject = targets[1];
        Debug.Log("Before и Next настроены и скрыты до завершения мойки. Сохрани сцену и проверь полный цикл в Play Mode. Положение, размеры и обработчики не изменены.");
    }
}
