using UnityEngine;

public class AreaHole : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var b = other.GetComponent<ball>();
        if (b == null)
            b = other.GetComponentInParent<ball>();
        if (b == null)
            return;

        if (b.IsCue)
        {
            GameManager.instance.OnCuePotted();
            return;
        }

        GameManager.instance.AddScore(b.Point);
        b.HideBall();
    }
}
