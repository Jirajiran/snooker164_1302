using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music Sources")]
    [SerializeField] AudioSource musicGameplay;
    [SerializeField] AudioSource musicLose;
    [SerializeField] AudioSource musicWinLoop;

    [Header("SFX")]
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip winSting;
    [SerializeField] AudioClip snookIt;
    [SerializeField] AudioClip holdHole;
    [SerializeField] AudioClip ballHit;
    [SerializeField] AudioClip typing;

    [Header("UI Typing")]
    [SerializeField] bool playTypingOnClick = true;
    [SerializeField] bool playTypingOnPointerEnter = true;
    [SerializeField] string[] ignoreButtonNames = { "ButtonShot" };

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StopAllMusic();
        if (musicGameplay != null)
        {
            musicGameplay.loop = true;
            musicGameplay.Play();
        }
        WireUiTyping();
    }

    void WireUiTyping()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            if (btn == null || ShouldIgnore(btn))
                continue;

            if (playTypingOnClick)
                btn.onClick.AddListener(PlayTyping);

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

    void StopAllMusic()
    {
        if (musicGameplay != null)
            musicGameplay.Stop();
        if (musicLose != null)
            musicLose.Stop();
        if (musicWinLoop != null)
            musicWinLoop.Stop();
    }

    public void PlayGameplayMusic()
    {
        StopAllMusic();
        if (musicGameplay == null)
            return;
        musicGameplay.loop = true;
        musicGameplay.Play();
    }

    public void PlayWinMusic()
    {
        StopAllMusic();
        if (musicWinLoop != null)
        {
            musicWinLoop.loop = true;
            musicWinLoop.Play();
        }
        if (sfxSource != null && winSting != null)
            sfxSource.PlayOneShot(winSting);
    }

    public void PlayLoseMusic()
    {
        StopAllMusic();
        if (musicLose == null)
            return;
        musicLose.loop = true;
        musicLose.Play();
    }

    public void SetGameplayMusicPaused(bool paused)
    {
        if (musicGameplay == null)
            return;
        if (paused)
            musicGameplay.Pause();
        else
            musicGameplay.UnPause();
    }

    public void PlaySnookIt()
    {
        PlaySfx(snookIt);
    }

    public void PlayHoldHole()
    {
        PlaySfx(holdHole);
    }

    public void PlayBallHit(float volume = 1f)
    {
        if (sfxSource == null || ballHit == null)
            return;
        sfxSource.PlayOneShot(ballHit, Mathf.Clamp01(volume));
    }

    public void PlayTyping()
    {
        PlaySfx(typing);
    }

    void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;
        sfxSource.PlayOneShot(clip);
    }
}

public class UiPointerEnterHook : MonoBehaviour, IPointerEnterHandler
{
    public System.Action onEnter;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
    }
}
