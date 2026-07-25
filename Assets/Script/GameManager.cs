using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] private int playerScore;

    [SerializeField]
    private GameObject[] ballPositions;

    [SerializeField]
    private GameObject ballPrefab;
    public int PlayerScore { get { return playerScore; } set { playerScore = value; } }

    public static GameManager instance;
    void Start()
    {
        instance = this;
    }


    void Update()
    {
        SetBall(BallColor.White, 0);
        SetBall(BallColor.Red, 1);
        SetBall(BallColor.Yellow, 2);
        SetBall(BallColor.Green, 3);
        SetBall(BallColor.Brown, 4);
        SetBall(BallColor.Blue, 5);
        SetBall(BallColor.Pink, 6);
        SetBall(BallColor.Black, 7);

    }
    private void SetBall(BallColor colorB, int i)
    {

        GameObject obj = Instantiate(ballPrefab,ballPositions[i].transform.position, Quaternion.identity);
        ball b = obj.GetComponent<ball>();
        b.SetColorAndPoint(colorB);

    }
}
