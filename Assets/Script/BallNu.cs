using UnityEngine;

public enum BallType
{
    Cue,     // ลูกขาว (0 คะแนน)
    Red,     // ลูกแดง (1 คะแนน)
    Yellow,  // ลูกเหลือง (2 คะแนน)
    Green,   // ลูกเขียว (3 คะแนน)
    Brown,   // ลูกน้ำตาล (4 คะแนน)
    Blue,    // ลูกน้ำเงิน (5 คะแนน)
    Pink,    // ลูกชมพู (6 คะแนน)
    Black    // ลูกดำ (7 คะแนน)
}

[RequireComponent(typeof(Rigidbody))]
public class BallNu : MonoBehaviour
{
    [Header("Ball Properties")]
    public BallType ballType = BallType.Red;

    [SerializeField] private int pointValue = 1; // แสดงใน Inspector แต่แก้ไขจาก Script อื่นไม่ได้

    public Rigidbody Rb { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        UpdatePointValue();
    }

    private void OnValidate()
    {
        UpdatePointValue();
    }

    public void SetBallType(BallType type)
    {
        ballType = type;
        UpdatePointValue();
    }

    private void UpdatePointValue()
    {
        switch (ballType)
        {
            case BallType.Cue: pointValue = 0; break;
            case BallType.Red: pointValue = 1; break;
            case BallType.Yellow: pointValue = 2; break;
            case BallType.Green: pointValue = 3; break;
            case BallType.Brown: pointValue = 4; break;
            case BallType.Blue: pointValue = 5; break;
            case BallType.Pink: pointValue = 6; break;
            case BallType.Black: pointValue = 7; break;
        }
    }

    public int GetPoints() => pointValue;

    public void OnPotted()
    {
        gameObject.SetActive(false);
    }
}