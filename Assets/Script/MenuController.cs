using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] string loadingSceneName = "Loading";
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip typingClip;
    [SerializeField] float loadDelay = 0.08f;

    void Awake()
    {
        WireButton(startButton, StartGame);
        WireButton(exitButton, ExitGame);
    }

    void WireButton(Button btn, UnityEngine.Events.UnityAction onClick)
    {
        if (btn == null)
            return;

        btn.onClick.RemoveListener(onClick);
        btn.onClick.AddListener(PlayTyping);
        btn.onClick.AddListener(onClick);

        var hook = btn.GetComponent<UiPointerEnterHook>();
        if (hook == null)
            hook = btn.gameObject.AddComponent<UiPointerEnterHook>();
        hook.onEnter = PlayTyping;
    }

    void PlayTyping()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayTyping();
            return;
        }
        if (sfxSource != null && typingClip != null)
            sfxSource.PlayOneShot(typingClip);
    }

    public void StartGame()
    {
        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        if (loadDelay > 0f)
            yield return new WaitForSecondsRealtime(loadDelay);
        SceneManager.LoadScene(loadingSceneName);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
