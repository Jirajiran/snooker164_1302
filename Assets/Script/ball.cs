using UnityEngine;
using UnityEngine.EventSystems;

public enum BallColor
{
    White,
    Red,
    Yellow,
    Green,
    Brown, 
    Blue,
    Pink,
    Black
}
public class ball : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]private int point;

    [SerializeField]private BallColor color;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(point);
        //switch (color)
        //{
        //    case BallColor.Red:
        //        GameManager.instance.PlayerScore += point;
        //        break;
        //    case BallColor.Black:
        //        GameManager.instance.PlayerScore += point;
        //        break;
        //}
    }
}
