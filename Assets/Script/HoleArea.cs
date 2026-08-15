using UnityEngine;

public class HoleArea : MonoBehaviour
{
    [Header("Pocket Settings")]
    [SerializeField] private int pocketId = 1;

    private void OnTriggerEnter(Collider other)
    {
        BallNu ball = other.GetComponent<BallNu>();

        // เช็ค !ball.IsPotted กันไม่ให้เรียก SendBallToManager ซ้ำ
        // ในกรณีที่ OnTriggerEnter ยิงมากกว่า 1 ครั้งในช่วงเวลาไล่เลี่ยกัน
        if (ball != null && !ball.IsPotted)
        {
            SendBallToManager(ball);
        }
    }

    public void SendBallToManager(BallNu ball)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBallPotted(ball);
        }
    }
}
