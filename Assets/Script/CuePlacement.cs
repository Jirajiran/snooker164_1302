using UnityEngine;

// ใส่ Script นี้ไว้ที่ GameObject เดียวกับ DriveBall (ตัวลูกขาว)
[RequireComponent(typeof(DriveBall))]
[RequireComponent(typeof(Rigidbody))]
public class CuePlacement : MonoBehaviour
{
    [Header("D-Zone Settings")]
    // ลาก Collider (ตั้งเป็น IsTrigger) ของโซน D บนโต๊ะมาใส่ตรงนี้
    [SerializeField] private Collider dZoneCollider;

    // Layer ของพื้นโต๊ะ ใช้สำหรับยิง Raycast หาตำแหน่งเมาส์บนโต๊ะ
    [SerializeField] private LayerMask tableLayer;

    private DriveBall driveBall;
    private Rigidbody rb;

    public bool IsPlacing { get; private set; } = false;

    private void Awake()
    {
        driveBall = GetComponent<DriveBall>();
        rb = GetComponent<Rigidbody>();
    }

    // เรียกจาก GameManager ตอนลูกขาวถูกลงหลุมและทุกลูกหยุดนิ่งแล้ว
    public void BeginPlacement()
    {
        IsPlacing = true;
        driveBall.DisableControl();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // ล็อคฟิสิกส์ไว้ระหว่างที่ผู้เล่นกำลังลากวาง
    }

    private void Update()
    {
        if (!IsPlacing) return;

        // ลากลูกขาวด้วยเมาส์ซ้ายค้าง (ปรับเป็น Touch ได้ถ้าทำบนมือถือ)
        if (Input.GetMouseButton(0))
        {
            TryMoveToMouse();
        }

        // กด Space เพื่อยืนยันตำแหน่งและเริ่มควบคุมยิงได้
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmPlacement();
        }
    }

    private void TryMoveToMouse()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, tableLayer))
        {
            // อนุญาตให้วางได้เฉพาะในโซน D เท่านั้น
            if (dZoneCollider != null && dZoneCollider.bounds.Contains(hit.point))
            {
                Vector3 newPos = hit.point;
                newPos.y = transform.position.y; // คงความสูงของลูกบอลไว้เท่าเดิม
                transform.position = newPos;
            }
        }
    }

    // ผูกกับปุ่ม UI "ยืนยันตำแหน่ง" ก็ได้ ไม่จำเป็นต้องใช้ปุ่ม Space อย่างเดียว
    public void ConfirmPlacement()
    {
        IsPlacing = false;
        rb.isKinematic = false;
        driveBall.EnableControl();
    }
}
