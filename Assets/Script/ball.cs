using UnityEngine;

public enum BallColor
{
    White,
    Red,
    Yellow,
    Green,
    Brown,
    Blue,
    Pink,
    Black
}

public class ball : MonoBehaviour
{
    [SerializeField] int point;

    MeshRenderer meshRenderer;
    Collider col;

    public int Point => point;
    public bool IsCue => point == 0;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
    }

    public void Apply(int p, Material mat)
    {
        point = p;
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
        if (mat != null && meshRenderer != null)
            meshRenderer.material = mat;
    }

    public void HideBall()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
        if (col == null)
            col = GetComponent<Collider>();

        if (meshRenderer != null)
            meshRenderer.enabled = false;
        if (col != null)
            col.enabled = false;
    }

    public void ShowBall()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
        if (col == null)
            col = GetComponent<Collider>();

        if (meshRenderer != null)
            meshRenderer.enabled = true;
        if (col != null)
            col.enabled = true;
    }
}
