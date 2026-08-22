#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioManagerSetup
{
    const string MenuPath = "Assets/Scenes/Menu.unity";
    const string GamePath = "Assets/Scenes/Scene_01.unity";

    [MenuItem("Snooker/Setup AudioManager & Clean Orphan Sources")]
    public static void SetupAll()
    {
        SetupMenuAudioManager();
        CleanGameSceneAudio();
        CleanMenuOrphans();
        AssetDatabase.SaveAssets();
        Debug.Log("[Snooker] AudioManager setup complete.");
    }

    [InitializeOnLoadMethod]
    static void AutoOnce()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            if (!System.IO.File.Exists(MenuPath))
                return;
            if (GameObject.Find("AudioManager") != null)
                return;
            SetupAll();
        };
    }

    static void SetupMenuAudioManager()
    {
        var scene = EditorSceneManager.OpenScene(MenuPath, OpenSceneMode.Single);
        var existing = GameObject.Find("AudioManager");
        if (existing == null)
            existing = new GameObject("AudioManager");

        var manager = existing.GetComponent<AudioManager>();
        if (manager == null)
            manager = existing.AddComponent<AudioManager>();

        var so = new SerializedObject(manager);
        AssignClip(so, "menuMusic", "Assets/Audio/CoolHipHop.mp3");
        AssignClip(so, "gameplayMusic", "Assets/Audio/Community_gamemusic.mp3");
        AssignClip(so, "winLoopMusic", "Assets/Audio/CoolHipHop.mp3");
        AssignClip(so, "loseMusic", "Assets/Audio/GameOver.mp3");
        AssignClip(so, "winSting", "Assets/Audio/WinGame.mp3");
        AssignClip(so, "typing", "Assets/Audio/Typing.mp3");
        AssignClip(so, "snookIt", "Assets/Audio/SnookIt.mp3");
        AssignClip(so, "holdHole", "Assets/Audio/HoldHole.mp3");
        AssignClip(so, "ballHit", "Assets/Audio/BallHit.mp3");
        AssignClip(so, "getPoint", "Assets/Audio/CoinSFX.mp3");
        AssignClip(so, "minusPoint", "Assets/Audio/MinusPoint.mp3");
        var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/Audio/AudioMixer.mixer");
        so.FindProperty("mixer").objectReferenceValue = mixer;
        so.ApplyModifiedPropertiesWithoutUndo();

        var sliderG = GameObject.Find("SliderG");
        if (sliderG != null && sliderG.GetComponent<AudioSettingsUI>() == null)
            sliderG.AddComponent<AudioSettingsUI>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    static void AssignClip(SerializedObject so, string prop, string assetPath)
    {
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
        so.FindProperty(prop).objectReferenceValue = clip;
    }

    static void CleanMenuOrphans()
    {
        var scene = EditorSceneManager.OpenScene(MenuPath, OpenSceneMode.Single);
        RemoveAudioSourcesOn("Canvas_Menu");
        RemoveAudioSourcesOn("MenuController");
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    static void CleanGameSceneAudio()
    {
        var scene = EditorSceneManager.OpenScene(GamePath, OpenSceneMode.Single);
        var canvas = GameObject.Find("Canvas_GameManager");
        if (canvas != null)
        {
            Object.DestroyImmediate(canvas.GetComponent<AudioManager>(), true);
            foreach (var src in canvas.GetComponents<AudioSource>())
                Object.DestroyImmediate(src, true);
        }
        RemoveAudioSourcesOn("PulsText");
        RemoveAudioSourcesOn("MinusText");
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    static void RemoveAudioSourcesOn(string objectName)
    {
        var go = GameObject.Find(objectName);
        if (go == null)
            return;
        foreach (var src in go.GetComponents<AudioSource>())
            Object.DestroyImmediate(src, true);
    }
}
#endif
