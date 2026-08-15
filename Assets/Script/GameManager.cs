using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DriveBall cueBall;
    [SerializeField] private UiShowP scoreUI;

    [Header("Ball Prefab")]
    [SerializeField] private GameObject ballPrefab; // Prefab ลูกบอลพื้นฐาน

    [Header("Spawner Settings")]
    // ลาก Empty Objects ที่มีคอมโพเนนต์ BallSpawnPoint มาใส่
    [SerializeField] private List<BallSpawnPoint> spawnPoints = new List<BallSpawnPoint>();


    private List<BallNu> activeBalls = new List<BallNu>();

    [Header("Game State")]
    [SerializeField] private int allPointNu = 0;
    [SerializeField] private float stopThreshold = 0.05f;
    public bool isMoving { get; private set; } = false;

    [Header("Win / Lose Rule")]
    [SerializeField] private int maxShots = 5;   // ยิงได้ทั้งหมดกี่ครั้ง
    [SerializeField] private int winScore = 9;   // ต้องได้คะแนนเท่านี้ขึ้นไปถึงจะชนะ

    private int currentShot = 0;
    private bool cueWasPotted = false;
    public bool isGameOver { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SpawnAllBalls();

        if (scoreUI != null) scoreUI.UpdateScoreUI(allPointNu);
        if (cueBall != null) cueBall.EnableControl();
    }

    private void SpawnAllBalls()
    {
        // ถ้าไม่ได้ลากใส่ใน Inspector ให้ค้นหา BallSpawnPoint ทั่วทั้ง Scene อัตโนมัติ
        if (spawnPoints.Count == 0)
        {
            spawnPoints.AddRange(FindObjectsOfType<BallSpawnPoint>());
        }

        foreach (BallSpawnPoint point in spawnPoints)
        {
            if (point == null) continue;

            // สร้างลูกบอลตรงตำแหน่ง Transform ของ Empty Object นั้นๆ
            GameObject spawnedObj = Instantiate(ballPrefab, point.transform.position, point.transform.rotation);
            BallNu ball = spawnedObj.GetComponent<BallNu>();

            if (ball != null)
            {
                // กำหนดสีและแต้มจาก BallType ที่ตั้งไว้บน Empty Object
                ball.SetBallType(point.ballType);

                // จำตำแหน่ง spawn ไว้ในตัวลูกบอลเอง เพื่อใช้ตอน reset/restart ทีหลัง
                // (ไม่ต้องวน loop หา spawnPoint ซ้ำอีกในภายหลัง)
                ball.SetSpawnTransform(point.transform.position, point.transform.rotation);

                activeBalls.Add(ball);

                // หากจุดนี้เป็นลูกขาว ให้ผูกกับ DriveBall
                if (point.ballType == BallType.Cue)
                {
                    cueBall = spawnedObj.GetComponent<DriveBall>();
                }
            }
        }
    }

    public void OnShotExecuted()
    {
        if (isGameOver) return;

        isMoving = true;
        currentShot++;
        StartCoroutine(WaitUntilBallsStop());
    }

    private IEnumerator WaitUntilBallsStop()
    {
        yield return new WaitForSeconds(0.5f);

        while (isMoving)
        {
            isMoving = false;
            foreach (BallNu ball in activeBalls)
            {
                if (ball != null && ball.gameObject.activeInHierarchy)
                {
                    if (ball.Rb.linearVelocity.sqrMagnitude > stopThreshold ||
                        ball.Rb.angularVelocity.sqrMagnitude > stopThreshold)
                    {
                        isMoving = true;
                        break;
                    }
                }
            }
            yield return null;
        }

        CheckGameState();
    }

    // เช็คหลังบอลหยุดนิ่งทุกครั้งว่า ยิงครบโควตาหรือยัง -> ตัดสินแพ้/ชนะ
    // ถ้ายังไม่ครบ -> เข้าสู่รอบยิงถัดไปตามปกติ
    private void CheckGameState()
    {
        if (currentShot >= maxShots)
        {
            if (allPointNu >= winScore)
            {
                WinGame();
            }
            else
            {
                LoseGame();
            }
        }
        else
        {
            ResetTurn();
        }
    }

    private void WinGame()
    {
        isGameOver = true;
        Debug.Log($"WIN! คะแนน {allPointNu}/{winScore} ใช้ไป {currentShot} ครั้ง");

        // หยุดควบคุมลูกขาวเมื่อจบเกม (ต่อ UI ชนะเพิ่มได้ตรงนี้)
        if (cueBall != null) cueBall.DisableControl();
    }

    private void LoseGame()
    {
        Debug.Log($"LOSE! คะแนน {allPointNu}/{winScore} หลังใช้ครบ {maxShots} ครั้ง -> Restart");
        RestartGame();
    }

    // Restart แบบไม่ Destroy/Instantiate ใหม่ - ใช้ลูกบอลเดิมที่มีอยู่แล้ว
    // ลดการสร้าง/ทำลาย object ซ้ำซ้อนโดยไม่จำเป็น
    public void RestartGame()
    {
        isGameOver = false;
        allPointNu = 0;
        currentShot = 0;
        cueWasPotted = false;

        if (scoreUI != null) scoreUI.UpdateScoreUI(allPointNu);

        foreach (BallNu ball in activeBalls)
        {
            if (ball != null) ball.ResetToSpawn();
        }

        if (cueBall != null) cueBall.EnableControl();
    }

    public void OnBallPotted(BallNu ball)
    {
        if (ball.ballType == BallType.Cue)
        {
            ResetCueBallPosition();
            cueWasPotted = true;
        }
        else
        {
            if (ball.IsPotted) return; // กัน AddScore ซ้ำถ้ามีการเรียกซ้อนกัน

            AddScore(ball.GetPoints());
            ball.OnPotted();
        }
    }

    public void AddScore(int points)
    {
        allPointNu += points;
        if (scoreUI != null) scoreUI.UpdateScoreUI(allPointNu);
    }

    public void ResetCueBallPosition()
    {
        if (cueBall == null) return;

        // ใช้ตำแหน่ง spawn ที่ลูกขาวจำไว้เอง แทนการวน loop หา spawnPoint
        BallNu cueBallNu = cueBall.GetComponent<BallNu>();
        if (cueBallNu != null)
        {
            cueBall.transform.position = cueBallNu.SpawnPosition;
            cueBall.transform.rotation = cueBallNu.SpawnRotation;
        }

        Rigidbody cueRb = cueBall.GetComponent<Rigidbody>();
        if (cueRb != null)
        {
            cueRb.linearVelocity = Vector3.zero;
            cueRb.angularVelocity = Vector3.zero;
        }
    }

    private void ResetTurn()
    {
        if (cueBall == null) return;

        // ถ้าลูกขาวเพิ่งถูกลงหลุมในรอบนี้ ให้เข้าโหมดวางตำแหน่งเอง (D-zone)
        CuePlacement placement = cueBall.GetComponent<CuePlacement>();
        if (cueWasPotted && placement != null)
        {
            placement.BeginPlacement();
        }
        else
        {
            cueBall.EnableControl();
        }

        cueWasPotted = false;
    }
}
