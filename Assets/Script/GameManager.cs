using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class BallSpawn
{
    public BallColor ballColor;
    public Transform point;
}

[System.Serializable]
public class BallLook
{
    public BallColor ballColor;
    public Material ballMaterial;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    const int WinScore = 9;
    const int MaxShots = 5;
    const int CuePotPenalty = 4;

    [SerializeField] int playerScore;
    [SerializeField] int currentTurn;

    [Header("Spawn")]
    [SerializeField] BallSpawn[] ballSpawns;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] BallLook[] ballLooks;

    [Header("UI")]
    [SerializeField] TMP_Text pointText;
    [SerializeField] TMP_Text endGameText;
    [SerializeField] TMP_Text TurnText;
    [SerializeField] TMP_Text GetPoint;
    [SerializeField] TMP_Text MinusPoint;
    [SerializeField] Animator pauseAnimator;
    [SerializeField] Animator endGameAnimator;
    [SerializeField] Animator UxPoint;
    [SerializeField] Animator characterAnimator;
    [SerializeField] string menuSceneName = "Menu";

    bool isPaused;
    bool gameEnded;

    public bool IsLocked => isPaused || gameEnded;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        pauseAnimator.SetBool("pauseUi", false);
        endGameAnimator.SetBool("ShowEndGame", false);
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("WinPose", false);
            characterAnimator.SetBool("LosePose", false);
        }
        SpawnAllBalls();
        pointText.text = $"Point : {playerScore}";
        currentTurn = 0;
        UpdateTurnText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseGame();
    }

    void SpawnAllBalls()
    {
        if (ballPrefab == null || ballSpawns == null)
            return;

        foreach (BallSpawn spawn in ballSpawns)
        {
            if (spawn.point == null || spawn.ballColor == BallColor.White)
                continue;
            SetBall(spawn.ballColor, spawn.point);
        }
    }

    void SetBall(BallColor ballColor, Transform point)
    {
        GameObject ballObject = Instantiate(ballPrefab, point.position, Quaternion.identity);
        ball spawnedBall = ballObject.GetComponent<ball>();
        if (spawnedBall == null)
            spawnedBall = ballObject.AddComponent<ball>();

        spawnedBall.Apply((int)ballColor, GetBallMaterial(ballColor));
    }

    Material GetBallMaterial(BallColor ballColor)
    {
        if (ballLooks == null)
            return null;
        foreach (BallLook look in ballLooks)
        {
            if (look.ballColor == ballColor)
                return look.ballMaterial;
        }
        return null;
    }

    public void BallPotted(ball pottedBall)
    {
        if (gameEnded || pottedBall == null)
            return;

        if (pottedBall.GetComponent<DriveBall>() != null)
        {
            playerScore = Mathf.Max(0, playerScore - CuePotPenalty);
            pointText.text = $"Point : {playerScore}";
            ShowUxPoint(-CuePotPenalty);
            DriveBall.Instance.RespawnAtCuePoint();
            return;
        }

        int gained = pottedBall.Point;
        playerScore += gained;
        pottedBall.HideBall();
        pointText.text = $"Point : {playerScore}";
        ShowUxPoint(gained);
        TryEndGame(-1);
    }

    void ShowUxPoint(int delta)
    {
        if (UxPoint == null)
            return;

        if (delta > 0)
        {
            if (GetPoint != null)
                GetPoint.text = $"+{delta}";
            UxPoint.SetTrigger("GetPointAni");
        }
        else if (delta < 0)
        {
            if (MinusPoint != null)
                MinusPoint.text = delta.ToString();
            UxPoint.SetTrigger("MinusPointAni");
        }
    }

    public void SetTurn(int turn)
    {
        currentTurn = turn;
        UpdateTurnText();
    }

    void UpdateTurnText()
    {
        if (TurnText != null)
            TurnText.text = $"Turn : {currentTurn} / {MaxShots}";
    }

    public void OnCueStopped(int shots)
    {
        SetTurn(shots);
        TryEndGame(shots);
    }

    void TryEndGame(int shots)
    {
        if (gameEnded)
            return;

        bool win = playerScore > WinScore;
        bool lose = shots >= 0 && shots >= MaxShots && !win;
        if (!win && !lose)
            return;

        gameEnded = true;
        if (pauseAnimator != null)
            pauseAnimator.SetBool("pauseUi", false);
        if (endGameAnimator != null)
            endGameAnimator.SetBool("ShowEndGame", true);
        if (endGameText != null)
            endGameText.text = win
                ? $"You are win\nYour score is {playerScore} > 9"
                : $"Your are lose\nYour score is {playerScore} < 9";

        if (AudioManager.instance != null)
        {
            if (win)
                AudioManager.instance.PlayWinMusic();
            else
                AudioManager.instance.PlayLoseMusic();
        }

        if (characterAnimator != null)
        {
            characterAnimator.SetBool("WinPose", win);
            characterAnimator.SetBool("LosePose", !win);
        }
    }

    public void PauseGame()
    {
        if (gameEnded)
            return;
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        if (pauseAnimator != null)
            pauseAnimator.SetBool("pauseUi", isPaused);
        if (AudioManager.instance != null)
            AudioManager.instance.SetGameplayMusicPaused(isPaused);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
