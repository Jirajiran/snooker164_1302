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

    [Header("Active Balls Runtime")]
    [SerializeField] private List<BallNu> activeBalls = new List<BallNu>();

    [Header("Game State")]
    [SerializeField] private int allPointNu = 0;
    [SerializeField] private float stopThreshold = 0.05f;
    public bool isMoving { get; private set; } = false;

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
        isMoving = true;
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

        ResetTurn();
    }

    public void OnBallPotted(BallNu ball)
    {
        if (ball.ballType == BallType.Cue)
        {
            ResetCueBallPosition();
        }
        else
        {
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
        foreach (BallSpawnPoint point in spawnPoints)
        {
            if (point.ballType == BallType.Cue && cueBall != null)
            {
                cueBall.transform.position = point.transform.position;
                cueBall.transform.rotation = point.transform.rotation;

                Rigidbody cueRb = cueBall.GetComponent<Rigidbody>();
                if (cueRb != null)
                {
                    cueRb.linearVelocity = Vector3.zero;
                    cueRb.angularVelocity = Vector3.zero;
                }
                break;
            }
        }
    }

    private void ResetTurn()
    {
        if (cueBall != null) cueBall.EnableControl();
    }
}