using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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

        float rotateInput = 0f;
        bool isMouseClick = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) rotateInput = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) rotateInput = 1f;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isMouseClick = true;
        }

        rotateInput = Input.GetAxis("Horizontal");
        isMouseClick = Input.GetMouseButtonDown(0);


        // 1. หมุนลูกบอลด้วย A / D หรือ ลูกศร
        if (Mathf.Abs(rotateInput) > 0.01f)
        {
            Rotate(rotateInput);
        }

        // 2. ยิงลูกบอลเมื่อคลิกเมาส์ซ้าย 1 ครั้ง
        if (isMouseClick)
        {
            OnFireButtonPressed();
        }
    }

    public void Rotate(float direction)
    {
        transform.Rotate(0, direction * rotateSpeed * Time.deltaTime, 0);
    }

    // เรียกยิงจาก UI Button หรือการคลิกเมาส์ซ้าย
    public void OnFireButtonPressed()
    {
        if (!canControl) return;

        DisableControl();
        rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);

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