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
    [SerializeField]
    private int point;

    [SerializeField]
    private BallColor colorB;

    private MeshRenderer mr;


    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        GameManager.instance.PlayerScore += point;
        Destroy(gameObject);
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

    public void SetColorAndPoint(BallColor colorB)
    {
       switch(colorB)
        {
            case BallColor.White:
                point = 0;
                mr.material.color = Color.white;
                break;
            case BallColor.Red:
                point = 1;
                mr.material.color = Color.red;
                break;
            case BallColor.Yellow:
                point = 2;
                mr.material.color = Color.yellow;
                break;
            case BallColor.Green:
                point = 3;
                mr.material.color = Color.green;
                break;
            case BallColor.Brown:
                point = 4;
                mr.material.color = new Color(0.6f, 0.3f, 0.1f);
                break;
            case BallColor.Blue:
                point = 5;
                mr.material.color = Color.blue;
                break;
            case BallColor.Pink:
                point = 6;
                mr.material.color = new Color(1f, 0.4f, 0.7f);
                break;
            case BallColor.Black:
                point = 7;
                mr.material.color = Color.black;
                break;
        }
    }
}
