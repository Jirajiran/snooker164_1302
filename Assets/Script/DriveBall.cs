using UnityEngine;

public class DriveBall : MonoBehaviour
{
    public static DriveBall Instance { get; private set; }

    [SerializeField] bool canForce = true;
    [SerializeField] GameObject showArrow;
    [SerializeField] int forcePower = 20;
    float rotateSpeed = 90f;
    float stopSpeed = 0.08f;
    float stillDuration = 0.25f;
    [SerializeField] Transform cueRespawn;

    Rigidbody rb;
    float stillTime;

    public bool CanForce => canForce;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start()
    {
        ShowArrow(true);
    }

    void Update()
    {
        if (!canForce || GameManager.instance == null || GameManager.instance.IsLocked)
            return;
        InputBall();
    }

    void FixedUpdate()
    {
        if (canForce || rb == null)
            return;

        if (IsAlmostStopped())
        {
            stillTime += Time.fixedDeltaTime;
            if (stillTime >= stillDuration)
                ResetPos();
        }
        else
        {
            stillTime = 0f;
        }
    }

    void InputBall()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            RotationBall();

        if (Input.GetMouseButtonDown(0))
            ForceBall();
    }

    void RotationBall()
    {
        float dir = 0f;
        if (Input.GetKey(KeyCode.A))
            dir = -1f;
        if (Input.GetKey(KeyCode.D))
            dir = 1f;
        transform.Rotate(0f, dir * rotateSpeed * Time.deltaTime, 0f);
    }

    void ForceBall()
    {
        if (rb == null)
            return;

        canForce = false;
        stillTime = 0f;
        ShowArrow(false);
        rb.AddForce(transform.forward * forcePower, ForceMode.Impulse);
        GameManager.instance.OnShotUsed();
    }

    void ResetPos()
    {
        ClearMotion();
        FlattenRotationXZ();
        canForce = true;
        ShowArrow(true);
        if (GameManager.instance != null)
            GameManager.instance.OnCueStopped();
    }

    public void RespawnAtCuePoint()
    {
        ClearMotion();

        if (cueRespawn != null)
            transform.position = cueRespawn.position;

        FlattenRotationXZ();

        var b = GetComponent<ball>();
        if (b != null)
            b.ShowBall();

        canForce = true;
        stillTime = 0f;
        ShowArrow(true);
    }

    void ClearMotion()
    {
        if (rb == null)
            return;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void FlattenRotationXZ()
    {
        var e = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, e.y, 0f);
    }

    bool IsAlmostStopped()
    {
        float sq = stopSpeed * stopSpeed;
        return rb.linearVelocity.sqrMagnitude < sq
            && rb.angularVelocity.sqrMagnitude < sq;
    }

    void ShowArrow(bool on)
    {
        if (showArrow != null)
            showArrow.SetActive(on);
    }
}
