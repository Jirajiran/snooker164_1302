using UnityEngine;

public enum BallType
{
    Cue,     // 0 แต้ม (ขาว)
    Red,     // 1 แต้ม (แดง)
    Yellow,  // 2 แต้ม (เหลือง)
    Green,   // 3 แต้ม (เขียว)
    Brown,   // 4 แต้ม (น้ำตาล)
    Blue,    // 5 แต้ม (น้ำเงิน)
    Pink,    // 6 แต้ม (ชมพู)
    Black    // 7 แต้ม (ดำ)
}

[RequireComponent(typeof(Rigidbody))]
public class BallNu : MonoBehaviour
{
    [Header("Ball Properties")]
    public BallType ballType = BallType.Red;
    [SerializeField] private int pointValue = 1;

    public Rigidbody Rb { get; private set; }
    private Renderer ballRenderer;
    private static MaterialPropertyBlock propBlock;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        EnsureRenderer();
        UpdateBallProperties();
    }

    private void OnValidate()
    {
        EnsureRenderer();
        UpdateBallProperties();
    }

    private void EnsureRenderer()
    {
        if (ballRenderer == null)
        {
            ballRenderer = GetComponent<Renderer>();
        }
    }

    public void SetBallType(BallType type)
    {
        ballType = type;
        UpdateBallProperties();
    }

    private void UpdateBallProperties()
    {
        // 1. คำนวณแต้ม
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

        // 2. เปลี่ยนสีด้วย MaterialPropertyBlock (ปลอดภัย 100% ไม่ฟ้อง Error)
        if (ballRenderer != null)
        {
            Color targetColor = Color.white;
            switch (ballType)
            {
                case BallType.Cue: targetColor = Color.white; break;
                case BallType.Red: targetColor = Color.red; break;
                case BallType.Yellow: targetColor = Color.yellow; break;
                case BallType.Green: targetColor = Color.green; break;
                case BallType.Brown: targetColor = new Color(0.4f, 0.2f, 0.1f); break;
                case BallType.Blue: targetColor = Color.blue; break;
                case BallType.Pink: targetColor = new Color(1f, 0.41f, 0.71f); break;
                case BallType.Black: targetColor = Color.black; break;
            }

            if (propBlock == null) propBlock = new MaterialPropertyBlock();

            ballRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", targetColor);       // สำหรับ Built-in Render Pipeline
            propBlock.SetColor("_BaseColor", targetColor);   // สำหรับ URP (Universal Render Pipeline)
            ballRenderer.SetPropertyBlock(propBlock);
        }
    }

    public int GetPoints() => pointValue;

    public void OnPotted()
    {
        gameObject.SetActive(false);
    }
}