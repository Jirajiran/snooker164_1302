using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] private int playerScore;
    public int PlayerScore { get { return playerScore; } set { playerScore = value; } }

    public static GameManager instance;
    void Start()
    {
        instance = this;
    }


    void Update()
    {
        
    }
}
