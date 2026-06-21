using UnityEngine;

/// <summary>
/// Objeto pensado para agarre con DesktopVRRecorder (collider solido + rigidbody).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DesktopGrabbable : MonoBehaviour
{
    [SerializeField] private Color objectColor = Color.white;

    private void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.mass = 0.35f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        foreach (var col in GetComponents<Collider>())
        {
            col.isTrigger = false;
            if (col is MeshCollider meshCol)
            {
                meshCol.convex = true;
            }
        }

        ApplyColor();
    }

    private void ApplyColor()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            return;
        }

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader")
        {
            mat = new Material(Shader.Find("Standard"));
        }

        mat.color = objectColor;
        renderer.material = mat;
    }

    public void SetColor(Color color)
    {
        objectColor = color;
        ApplyColor();
    }
}
