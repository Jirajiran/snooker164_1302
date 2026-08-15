using UnityEngine;
using UnityEngine.UI;

public class UiShowP : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Text scoreText;

    private void Start()
    {
        DisplayScoreString(0);
    }

    public void UpdateScoreUI(int allPoint)
    {
        DisplayScoreString(allPoint);
    }

    public void DisplayScoreString(int allPoint)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {allPoint}";
        }
    }
}