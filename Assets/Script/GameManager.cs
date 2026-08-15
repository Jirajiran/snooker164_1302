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

    [Header("Spawn")]
    [SerializeField] BallSpawn[] ballSpawns;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] BallLook[] ballLooks;

    [Header("UI")]
    [SerializeField] TMP_Text pointText;
    [SerializeField] TMP_Text endGameText;
    [SerializeField] Animator pauseAnimator;
    [SerializeField] Animator endGameAnimator;

    bool isPaused;
    bool gameEnded;

    public bool IsLocked => isPaused || gameEnded;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (pauseAnimator != null)
            pauseAnimator.SetBool("pauseUi", false);
        if (endGameAnimator != null)
            endGameAnimator.SetBool("ShowEndGame", false);
        SpawnAllBalls();
        if (pointText != null)
            pointText.text = playerScore.ToString();
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
            pointText.text = playerScore.ToString();
            DriveBall.Instance.RespawnAtCuePoint();
            return;
        }

        playerScore += pottedBall.Point;
        pottedBall.HideBall();
        pointText.text = playerScore.ToString();
        TryEndGame(-1);
    }

    public void OnCueStopped(int shots)
    {
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
        Time.timeScale = 0f;
            pauseAnimator.SetBool("pauseUi", false);
            endGameAnimator.SetBool("ShowEndGame", true);
            endGameText.text = win
                ? $"You are win\nYour score is {playerScore}"
                : $"Your are lose\nYour score is {playerScore}";
    }

    public void PauseGame()
    {
        if (gameEnded)
            return;
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
            pauseAnimator.SetBool("pauseUi", isPaused);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
