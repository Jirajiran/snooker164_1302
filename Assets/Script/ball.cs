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
    Collider ballCollider;

    public int Point => point;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ballCollider = GetComponent<Collider>();
    }

    public void Apply(int points, Material ballMaterial)
    {
        point = points;

        meshRenderer = GetComponent<MeshRenderer>();
        if (ballMaterial != null && meshRenderer != null)
            meshRenderer.material = ballMaterial;
    }

    public void HideBall()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ballCollider = GetComponent<Collider>();
        meshRenderer.enabled = false;
        ballCollider.enabled = false;
    }

    public void ShowBall()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ballCollider = GetComponent<Collider>();
        meshRenderer.enabled = true;
        ballCollider.enabled = true;
    }
}
