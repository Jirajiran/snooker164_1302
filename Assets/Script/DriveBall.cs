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
    int shotCount;
    public bool CanForce => canForce;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();

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

        if (Input.GetMouseButtonDown(0) && canForce)
        {
          RotationBall();
          ForceBall();
        }
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



    void RotationBall()
    {
        float rotateDirection = 0f;
        if (Input.GetKey(KeyCode.A))
            rotateDirection = -1f;
        if (Input.GetKey(KeyCode.D))
            rotateDirection = 1f;
        transform.Rotate(0f, rotateDirection * rotateSpeed * Time.deltaTime, 0f);
    }

    void ForceBall()
    {
        shotCount++;
        canForce = false;
        stillTime = 0f;
        ShowArrow(false);
        rb.AddForce(transform.forward * forcePower, ForceMode.Impulse);
       
    }

    void ResetPos()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        var euler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, euler.y, 0f);

        canForce = true;
        ShowArrow(true);
        GameManager.instance.OnCueStopped(shotCount);
    }

    public void RespawnAtCuePoint()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = cueRespawn.position;

        var euler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, euler.y, 0f);

        GetComponent<ball>().ShowBall();

        canForce = true;
        stillTime = 0f;
        ShowArrow(true);
    }
    bool IsAlmostStopped()
    {
        float stopSpeedSquared = stopSpeed * stopSpeed;
        return rb.linearVelocity.sqrMagnitude < stopSpeedSquared
            && rb.angularVelocity.sqrMagnitude < stopSpeedSquared;
    }

    void ShowArrow(bool visible)
    {
        showArrow.SetActive(visible);
    }
}
