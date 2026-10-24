using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PrivacyReleaseSetup
{
    [MenuItem("Car Wash ASMR/Prepare Privacy Release")]
    private static void Prepare()
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
        if (safeAreas.Length != 1 || source == null ||
            all.Any(t => t.name == "PrivacyPolicyButton"))
        {
            Debug.LogError("Need one SafeArea, a TMP font, and no existing PrivacyPolicyButton. No changes made.");
            return;
        }
        string settingsPath = "ProjectSettings/ProjectSettings.asset";
        string manifestPath = "Packages/manifest.json";
        string settings = File.ReadAllText(settingsPath);
        string manifest = File.ReadAllText(manifestPath);
        const string analyticsPattern = @"(?m)^  submitAnalytics: [01]\r?$";
        const string modulePattern = @"(?m)^    ""com\.unity\.modules\.unityanalytics""\s*:\s*""1\.0\.0"",?\r?\n";
        if (Regex.Matches(settings, analyticsPattern).Count != 1 ||
            Regex.Matches(manifest, modulePattern).Count != 1)
        {
            Debug.LogError("Unexpected settings or manifest. No changes made; inspect before continuing.");
            return;
        }
        GameObject buttonObject = new GameObject("PrivacyPolicyButton",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image),
            typeof(Button), typeof(PrivacyPolicyLink));
        Undo.RegisterCreatedObjectUndo(buttonObject, "Add privacy policy button");
        buttonObject.transform.SetParent(safeAreas[0], false);
        buttonObject.layer = safeAreas[0].gameObject.layer;
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(1f, 0f);
        rect.anchoredPosition = new Vector2(-48f, 48f);
        rect.sizeDelta = new Vector2(300f, 96f);
        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.55f);
        image.raycastTarget = true;
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        UnityEventTools.AddPersistentListener(button.onClick,
            buttonObject.GetComponent<PrivacyPolicyLink>().OpenPolicy);
        GameObject labelObject = new GameObject("Label",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);
        labelObject.layer = buttonObject.layer;
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(8f, 4f);
        labelRect.offsetMax = new Vector2(-8f, -4f);
        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.font = source.font;
        label.fontSize = 28f;
        label.text = "Privacy Policy";
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.raycastTarget = false;
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene))
        {
            Undo.PerformUndo();
            Debug.LogError("Scene save failed. Settings not changed.");
            return;
        }
        File.WriteAllText(settingsPath, Regex.Replace(settings, analyticsPattern, "  submitAnalytics: 0"));
        File.WriteAllText(manifestPath, Regex.Replace(manifest, modulePattern, ""));
        AssetDatabase.Refresh();
        Debug.Log("Privacy button saved; submitAnalytics set to 0; legacy analytics module removed from manifest. Let Unity resolve packages. Inspect the diff, test, then set a NEW Bundle Version Code before building. This is not proof of no network traffic.");
    }
}
