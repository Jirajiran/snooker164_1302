using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum MusicTrack
{
    None,
    Menu,
    Gameplay,
    Win,
    Lose
}

public enum SfxId
{
    Typing,
    SnookIt,
    HoldHole,
    BallHit,
    GetPoint,
    MinusPoint
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music Clips")]
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip gameplayMusic;
    [SerializeField] AudioClip winLoopMusic;
    [SerializeField] AudioClip loseMusic;
    [SerializeField] AudioClip winSting;

    [Header("SFX Clips")]
    [SerializeField] AudioClip typing;
    [SerializeField] AudioClip snookIt;
    [SerializeField] AudioClip holdHole;
    [SerializeField] AudioClip ballHit;
    [SerializeField] AudioClip getPoint;
    [SerializeField] AudioClip minusPoint;

    [Header("Mixer")]
    [SerializeField] AudioMixer mixer;

    bool playTypingOnClick = true;
    bool playTypingOnPointerEnter = true;
    string[] ignoreButtonNames = { "ButtonShot" };

    AudioSource musicSource;
    AudioSource sfxSource;
    MusicTrack currentTrack = MusicTrack.None;

    public AudioMixer Mixer => mixer;

    const string ParamMaster = "MasterVolume";
    const string ParamMusic = "MusicVolume";
    const string ParamVfx = "VFXVolume";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureSources();
        ApplySavedVolumes();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Menu")
            PlayMusic(MusicTrack.Menu);
        WireUiTypingInScene();
        if (sceneName == "Menu")
            EnsureAudioSettingsUi();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        WireUiTypingInScene();
        if (scene.name == "Menu")
            EnsureAudioSettingsUi();
    }

    void EnsureAudioSettingsUi()
    {
        var sliderG = GameObject.Find("SliderG");
        if (sliderG == null)
            return;
        if (sliderG.GetComponent<AudioSettingsUI>() == null)
            sliderG.AddComponent<AudioSettingsUI>();
    }

    void EnsureSources()
    {
        musicSource = GetOrAddSource("MusicSource");
        sfxSource = GetOrAddSource("SfxSource");
        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        if (mixer == null)
            return;

        AudioMixerGroup[] musicGroups = mixer.FindMatchingGroups("Music");
        AudioMixerGroup[] vfxGroups = mixer.FindMatchingGroups("VFX");
        if (musicGroups.Length > 0)
            musicSource.outputAudioMixerGroup = musicGroups[0];
        if (vfxGroups.Length > 0)
            sfxSource.outputAudioMixerGroup = vfxGroups[0];
    }

    public void ApplySavedVolumes()
    {
        if (mixer == null)
            return;

        mixer.SetFloat(ParamMaster, Setting.LoadVolumeMaster());
        mixer.SetFloat(ParamMusic, Setting.LoadVolumeMusic());
        mixer.SetFloat(ParamVfx, Setting.LoadVolumeVfx());
    }

    AudioSource GetOrAddSource(string childName)
    {
        Transform child = transform.Find(childName);
        if (child == null)
        {
            var go = new GameObject(childName);
            go.transform.SetParent(transform, false);
            child = go.transform;
        }
        var source = child.GetComponent<AudioSource>();
        if (source == null)
            source = child.gameObject.AddComponent<AudioSource>();
        return source;
    }

    public void PlayMusic(MusicTrack track, bool playSting = false)
    {
        StopMusic();
        currentTrack = track;

        AudioClip clip = track switch
        {
            MusicTrack.Menu => menuMusic,
            MusicTrack.Gameplay => gameplayMusic,
            MusicTrack.Win => winLoopMusic,
            MusicTrack.Lose => loseMusic,
            _ => null
        };

        if (clip == null)
        {
            if (track != MusicTrack.None)
                Debug.LogWarning($"[AudioManager] Missing music clip for {track}");
            return;
        }

        if (musicSource == null)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();

        if (playSting && winSting != null && sfxSource != null)
            sfxSource.PlayOneShot(winSting);
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
        currentTrack = MusicTrack.None;
    }

    public void SetMusicPaused(bool paused)
    {
        if (musicSource == null || currentTrack == MusicTrack.None)
            return;
        if (paused)
            musicSource.Pause();
        else
            musicSource.UnPause();
    }

    public void PlaySfx(SfxId id, float volume = 1f)
    {
        AudioClip clip = id switch
        {
            SfxId.Typing => typing,
            SfxId.SnookIt => snookIt,
            SfxId.HoldHole => holdHole,
            SfxId.BallHit => ballHit,
            SfxId.GetPoint => getPoint,
            SfxId.MinusPoint => minusPoint,
            _ => null
        };

        if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] Missing SFX clip for {id}");
            return;
        }
        if (sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    public void WireUiTypingInScene()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button btn in buttons)
        {
            if (btn == null || ShouldIgnore(btn))
                continue;

            if (playTypingOnClick)
            {
                btn.onClick.RemoveListener(PlayTyping);
                btn.onClick.AddListener(PlayTyping);
            }

            if (playTypingOnPointerEnter)
            {
                var hook = btn.gameObject.GetComponent<UiPointerEnterHook>();
                if (hook == null)
                    hook = btn.gameObject.AddComponent<UiPointerEnterHook>();
                hook.onEnter = PlayTyping;
            }
        }
    }

    bool ShouldIgnore(Button btn)
    {
        if (ignoreButtonNames == null)
            return false;
        string name = btn.gameObject.name;
        foreach (string ignore in ignoreButtonNames)
        {
            if (!string.IsNullOrEmpty(ignore) && name == ignore)
                return true;
        }
        return false;
    }

    public void PlayGameplayMusic() => PlayMusic(MusicTrack.Gameplay);
    public void PlayWinMusic() => PlayMusic(MusicTrack.Win, playSting: true);
    public void PlayLoseMusic() => PlayMusic(MusicTrack.Lose);
    public void SetGameplayMusicPaused(bool paused) => SetMusicPaused(paused);
    public void PlaySnookIt() => PlaySfx(SfxId.SnookIt);
    public void PlayHoldHole() => PlaySfx(SfxId.HoldHole);
    public void PlayBallHit(float volume = 1f) => PlaySfx(SfxId.BallHit, volume);
    public void PlayTyping() => PlaySfx(SfxId.Typing);
    public void PlayGetPoint() => PlaySfx(SfxId.GetPoint);
    public void PlayMinusPoint() => PlaySfx(SfxId.MinusPoint);
}

public class UiPointerEnterHook : MonoBehaviour, IPointerEnterHandler
{
    public System.Action onEnter;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
    }
}
