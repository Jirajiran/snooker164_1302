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
    private Collider col;

    // กันเรียก OnPotted ซ้ำ (แก้ปัญหา trigger "ซ้อน")
    public bool IsPotted { get; private set; } = false;

    // ตำแหน่ง/หมุนตอนเกิดครั้งแรก - เป็น "แหล่งความจริงเดียว" (single source of truth)
    // ให้ทั้งการ reset ลูกขาวและ restart ทั้งกระดานเรียกใช้จุดเดียวกัน ไม่ต้องวน loop หา spawnPoint ซ้ำ
    public Vector3 SpawnPosition { get; private set; }
    public Quaternion SpawnRotation { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
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

    // เรียกจาก GameManager ตอน Spawn ครั้งแรกเท่านั้น
    public void SetSpawnTransform(Vector3 pos, Quaternion rot)
    {
        SpawnPosition = pos;
        SpawnRotation = rot;
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
        if (IsPotted) return;
        IsPotted = true;

        // ปิด Collider ทันที กัน HoleArea ยิง OnTriggerEnter ซ้ำ
        if (col != null) col.enabled = false;

        if (Rb != null)
        {
            Rb.linearVelocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
        }

        gameObject.SetActive(false);
    }

    public void ResetPottedState()
    {
        IsPotted = false;
        if (col != null) col.enabled = true;
    }

    // คืนลูกบอลกลับไปที่ตำแหน่ง spawn เดิม + เปิดใช้งานใหม่ทั้งหมด
    // ใช้ทั้งตอน GameManager.RestartGame() (แพ้แล้วเริ่มใหม่)
    public void ResetToSpawn()
    {
        ResetPottedState();

        transform.position = SpawnPosition;
        transform.rotation = SpawnRotation;
        gameObject.SetActive(true);

        if (Rb != null)
        {
            Rb.linearVelocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
        }
    }
}
