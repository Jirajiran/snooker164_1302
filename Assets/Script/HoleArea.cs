using UnityEngine;

public class HoleArea : MonoBehaviour
{
    [Header("Pocket Settings")]
    [SerializeField] private int pocketId = 1;

    private void OnTriggerEnter(Collider other)
    {
        BallNu ball = other.GetComponent<BallNu>();
        if (ball != null)
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