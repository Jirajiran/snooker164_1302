#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;

/// <summary>
/// Builds template hierarchy into Menu / Loading scenes and wires controller refs.
/// Menu: Snooker → Setup Menu &amp; Loading Hierarchy
/// </summary>
public static class MenuLoadingHierarchySetup
{
    const string MenuPath = "Assets/Scenes/Menu.unity";
    const string LoadingPath = "Assets/Scenes/Loading.unity";

    [MenuItem("Snooker/Setup Menu & Loading Hierarchy")]
    public static void SetupAll()
    {
        SetupMenuScene();
        SetupLoadingScene();
        AssetDatabase.SaveAssets();
        Debug.Log("[Snooker] Menu & Loading hierarchy ready.");
    }

    [MenuItem("Snooker/Setup Loading Hierarchy Only")]
    public static void SetupLoadingOnly()
    {
        SetupLoadingScene();
        AssetDatabase.SaveAssets();
        Debug.Log("[Snooker] Loading hierarchy ready.");
    }

    [InitializeOnLoadMethod]
    static void AutoOnce()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            bool needMenu = System.IO.File.Exists(MenuPath)
                && !System.IO.File.ReadAllText(MenuPath).Contains("Button_Start");
            bool needLoading = System.IO.File.Exists(LoadingPath)
                && !System.IO.File.ReadAllText(LoadingPath).Contains("ProgressFrame");

            if (needMenu && needLoading)
                SetupAll();
            else if (needMenu)
            {
                SetupMenuScene();
                AssetDatabase.SaveAssets();
            }
            else if (needLoading)
            {
                SetupLoadingScene();
                AssetDatabase.SaveAssets();
            }
        };
    }

    static void SetupMenuScene()
    {
        var scene = EditorSceneManager.OpenScene(MenuPath, OpenSceneMode.Single);
        ClearNonCameraRoots(scene);

        EnsureCamera(new Color(0.08f, 0.12f, 0.1f, 1f));
        EnsureEventSystem();

        var canvas = CreateCanvas("Canvas_Menu");
        CreateImage(canvas.transform, "Background", stretchFull: true,
            color: new Color(0.08f, 0.12f, 0.1f, 1f));

        var title = CreateTmp(canvas.transform, "Title", "SNOOKER",
            new Vector2(0, 220), new Vector2(800, 120), 72);
        title.fontStyle = FontStyles.Bold;

        var startBtn = CreateButton(canvas.transform, "Button_Start", "START",
            new Vector2(0, 20), new Vector2(360, 90));
        var exitBtn = CreateButton(canvas.transform, "Button_Exit", "EXIT",
            new Vector2(0, -110), new Vector2(360, 90));

        var controllerGo = new GameObject("MenuController");
        var controller = controllerGo.AddComponent<MenuController>();
        var so = new SerializedObject(controller);
        so.FindProperty("loadingSceneName").stringValue = "Loading";
        so.FindProperty("startButton").objectReferenceValue = startBtn;
        so.FindProperty("exitButton").objectReferenceValue = exitBtn;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    static void SetupLoadingScene()
    {
        var scene = EditorSceneManager.OpenScene(LoadingPath, OpenSceneMode.Single);
        ClearNonCameraRoots(scene);

        EnsureCamera(new Color(0.06f, 0.08f, 0.1f, 1f));
        EnsureEventSystem();

        var canvas = CreateCanvas("Canvas_Loading");
        CreateImage(canvas.transform, "Background", stretchFull: true,
            color: new Color(0.06f, 0.08f, 0.1f, 1f));

        var status = CreateTmp(canvas.transform, "Status", "Loading...",
            new Vector2(0, 80), new Vector2(600, 60), 40);

        var frame = CreateImage(canvas.transform, "ProgressFrame",
            anchoredPos: new Vector2(0, -20), size: new Vector2(720, 48),
            color: new Color(0.15f, 0.18f, 0.2f, 1f));

        var fill = CreateImage(frame.transform, "Fill",
            anchoredPos: Vector2.zero, size: new Vector2(700, 32),
            color: new Color(0.25f, 0.85f, 0.45f, 1f));
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        fill.fillAmount = 0f;

        var percent = CreateTmp(canvas.transform, "Percent", "0%",
            new Vector2(0, -90), new Vector2(200, 50), 28);

        var controllerGo = new GameObject("LoadingController");
        var controller = controllerGo.AddComponent<LoadingController>();
        var so = new SerializedObject(controller);
        so.FindProperty("gameSceneName").stringValue = "Scene_01";
        so.FindProperty("fillBar").objectReferenceValue = fill;
        so.FindProperty("percentText").objectReferenceValue = percent;
        so.FindProperty("statusText").objectReferenceValue = status;
        so.FindProperty("minShowSeconds").floatValue = 1.2f;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    static void ClearNonCameraRoots(UnityEngine.SceneManagement.Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.GetComponent<Camera>() != null)
                Object.DestroyImmediate(root);
            else
                Object.DestroyImmediate(root);
        }
    }

    static void EnsureCamera(Color bg)
    {
        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = bg;
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.transform.position = new Vector3(0, 0, -10);
        camGo.AddComponent<AudioListener>();
        camGo.AddComponent<UniversalAdditionalCameraData>();
    }

    static void EnsureEventSystem()
    {
        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        var module = es.AddComponent<InputSystemUIInputModule>();
        module.AssignDefaultActions();
    }

    static Canvas CreateCanvas(string name)
    {
        var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    static Image CreateImage(Transform parent, string name, Vector2 anchoredPos = default, Vector2 size = default,
        Color color = default, bool stretchFull = false)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        if (stretchFull)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        else
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size == default ? new Vector2(100, 100) : size;
        }
        var img = go.GetComponent<Image>();
        img.color = color == default ? Color.white : color;
        return img;
    }

    static TextMeshProUGUI CreateTmp(Transform parent, string name, string text, Vector2 pos, Vector2 size, float fontSize)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        return tmp;
    }

    static Button CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size)
    {
        var img = CreateImage(parent, name, pos, size, new Color(0.2f, 0.55f, 0.35f, 1f));
        var button = img.gameObject.AddComponent<Button>();
        var colors = button.colors;
        colors.highlightedColor = new Color(0.3f, 0.7f, 0.45f, 1f);
        colors.pressedColor = new Color(0.15f, 0.4f, 0.25f, 1f);
        button.colors = colors;
        CreateTmp(img.transform, "Label", label, Vector2.zero, size, 36);
        return button;
    }
}
#endif
