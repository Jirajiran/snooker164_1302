using UnityEngine;

public class HoleArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // เช็กว่าวัตถุที่ชนมีสคริปต์ BallNu หรือไม่
        BallNu ball = other.GetComponent<BallNu>();

        if (ball != null)
        {
            // ส่งลูกบอลไปให้ GameManager จัดการต่อทันที
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnBallPotted(ball);
            }
        }
    }
}