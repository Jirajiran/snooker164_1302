using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera List")]
    // ลาก Camera ทั้ง 3 ตัวมาใส่ตามลำดับที่ต้องการสลับ (ตัวที่ 0 = เริ่มต้น)
    [SerializeField] private Camera[] cameras;

    [Header("Keyboard Shortcuts (Optional)")]
    [SerializeField] private bool allowNumberKeys = true;

    private int currentIndex = 0;

    private void Start()
    {
        if (cameras != null && cameras.Length > 0)
        {
            SetActiveCamera(0);
        }
    }

    private void Update()
    {
        if (!allowNumberKeys) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) SetActiveCamera(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetActiveCamera(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetActiveCamera(2);
    }

    // ผูกกับปุ่ม UI "<"
    public void PreviousCamera()
    {
        if (cameras == null || cameras.Length == 0) return;
        int newIndex = (currentIndex - 1 + cameras.Length) % cameras.Length;
        SetActiveCamera(newIndex);
    }

    // ผูกกับปุ่ม UI ">"
    public void NextCamera()
    {
        if (cameras == null || cameras.Length == 0) return;
        int newIndex = (currentIndex + 1) % cameras.Length;
        SetActiveCamera(newIndex);
    }

    public void SetActiveCamera(int index)
    {
        if (cameras == null || cameras.Length == 0) return;
        if (index < 0 || index >= cameras.Length) return;

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != null)
            {
                cameras[i].gameObject.SetActive(i == index);
            }
        }

        currentIndex = index;
    }
}
