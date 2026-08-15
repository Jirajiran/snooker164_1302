using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    const int WinScore = 9;
    const int MaxShots = 5;
    const int CuePotPenalty = 4;

    [SerializeField] int playerScore;

    [Header("Spawn")]
    [SerializeField] GameObject[] ballPositions;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Material[] ballMaterials;

    [Header("UI")]
    [SerializeField] TMP_Text pointText;
    [SerializeField] TMP_Text endGameText;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject endGamePanel;

    int shotCount;
    bool isPaused;
    bool gameEnded;

    public int PlayerScore
    {
        get => playerScore;
        set => playerScore = value;
    }

    public bool IsLocked => isPaused || gameEnded;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (endGamePanel != null)
            endGamePanel.SetActive(false);
        SpawnAllBalls();
        RefreshPointUI();
    }

    Material GetBallMaterial(BallColor colorB)
    {
        if (colorB == BallColor.White)
            return null;

        int colorIndex = (int)colorB - 1;
        if (ballMaterials == null || colorIndex < 0 || colorIndex >= ballMaterials.Length)
            return null;
        return ballMaterials[colorIndex];
    }

    int GetPoint(BallColor colorB)
    {
        return colorB switch
        {
            BallColor.White => 0,
            BallColor.Red => 1,
            BallColor.Yellow => 2,
            BallColor.Green => 3,
            BallColor.Brown => 4,
            BallColor.Blue => 5,
            BallColor.Pink => 6,
            BallColor.Black => 7,
            _ => 0
        };
    }

    void SetBall(BallColor colorB, int i)
    {
        if (ballPrefab == null || ballPositions == null || i < 0 || i >= ballPositions.Length)
            return;
        if (ballPositions[i] == null)
            return;

        GameObject obj = Instantiate(ballPrefab, ballPositions[i].transform.position, Quaternion.identity);
        ball b = obj.GetComponent<ball>();
        if (b == null)
            b = obj.AddComponent<ball>();

        b.Apply(GetPoint(colorB), GetBallMaterial(colorB));
    }

    void SpawnAllBalls()
    {
        if (ballPositions == null || ballPositions.Length == 0)
            return;

        int redCount = Mathf.Min(15, ballPositions.Length);
        for (int i = 0; i < redCount; i++)
            SetBall(BallColor.Red, i);

        TrySet(BallColor.Yellow, 15);
        TrySet(BallColor.Green, 16);
        TrySet(BallColor.Brown, 17);
        TrySet(BallColor.Blue, 18);
        TrySet(BallColor.Pink, 19);
        TrySet(BallColor.Black, 20);
    }

    void TrySet(BallColor colorB, int i)
    {
        if (ballPositions != null && i < ballPositions.Length)
            SetBall(colorB, i);
    }

    public void AddScore(int p)
    {
        if (gameEnded)
            return;
        playerScore += p;
        RefreshPointUI();
        if (playerScore > WinScore)
            EndGame(true);
    }

    public void OnCuePotted()
    {
        if (gameEnded)
            return;
        playerScore = Mathf.Max(0, playerScore - CuePotPenalty);
        RefreshPointUI();
        if (DriveBall.Instance != null)
            DriveBall.Instance.RespawnAtCuePoint();
    }

    public void OnShotUsed()
    {
        if (gameEnded)
            return;
        shotCount++;
    }

    public void OnCueStopped()
    {
        if (gameEnded)
            return;
        if (playerScore > WinScore)
            EndGame(true);
        else if (shotCount >= MaxShots)
            EndGame(false);
    }

    void RefreshPointUI()
    {
        if (pointText != null)
            pointText.text = playerScore.ToString();
    }

    public void PauseGame()
    {
        if (gameEnded)
            return;
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        if (pausePanel != null)
            pausePanel.SetActive(isPaused);
    }

    void EndGame(bool win)
    {
        gameEnded = true;
        Time.timeScale = 0f;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (endGamePanel != null)
            endGamePanel.SetActive(true);
        if (endGameText != null)
            endGameText.text = win
                ? $"ชนะ\nคะแนน {playerScore}"
                : $"แพ้\nคะแนน {playerScore}";
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
