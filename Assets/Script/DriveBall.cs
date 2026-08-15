using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DriveBall : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private float shootForce = 500f;

    [Header("Status")]
    public bool canControl = true;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!canControl) return;

        // หมุนซ้าย-ขวา ด้วยปุ่ม A/D หรือ ลูกศร ซ้าย/ขวา
        float rotateInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(rotateInput) > 0.01f)
        {
            Rotate(rotateInput);
        }
    }

    public void Rotate(float direction)
    {
        transform.Rotate(0, direction * rotateSpeed * Time.deltaTime, 0);
    }

    // ผูก Method นี้กับ UI Button (OnClick)
    public void OnFireButtonPressed()
    {
        if (!canControl) return;

        DisableControl();

        // ใส่แรงยิงไปข้างหน้าตามทิศทางแกน Z (forward) ของลูกขาว
        rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);

        // แจ้ง GameManager ว่าเริ่มการยิงแล้ว
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnShotExecuted();
        }
    }

    public void EnableControl()
    {
        canControl = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void DisableControl()
    {
        canControl = false;
    }
}