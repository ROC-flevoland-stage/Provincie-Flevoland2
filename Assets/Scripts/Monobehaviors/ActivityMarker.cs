using UnityEngine;

public class ActivityMarker : MonoBehaviour
{
    [Header("Bobing Settings")]
    public Vector2 bobingExtents = new Vector2(-0.2f, 0.2f);
    public Vector3 bobingAxis = Vector3.up;
    public float bobingSpeed = 1f;

    [Header("Rotation Settings")]
    public Vector2 rotationExtents = new Vector2(0f, 360f);
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 1f;

    [Header("Materials")]
    public Material Active;
    public Material Inactive;

    private Vector3 home;
    private bool isActive = true;

    void Start()
    {
        home = transform.position;
    }

    void Update()
    {
        if (!isActive) return;

        transform.position = home + bobingAxis * Mathf.Sin(Time.time * bobingSpeed) * (bobingExtents.y - bobingExtents.x) / 2f + bobingAxis * bobingExtents.x;
        // Rotation logic
        if (Mathf.Approximately(rotationExtents.x, rotationExtents.y) || (rotationExtents.x == 0f && rotationExtents.y == 360f))
        {
            // Continuous rotation
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            // Oscillating rotation
            float rotationAmount = Mathf.Sin(Time.time * rotationSpeed) * (rotationExtents.y - rotationExtents.x) / 2f + rotationExtents.x;
            transform.rotation = Quaternion.Euler(rotationAxis * rotationAmount);
        }
    }

    public void SetMarkerActive(bool active)
    {
        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
            renderer.material = active ? Active : Inactive;
        isActive = active;
        if (!isActive)
        {
            transform.position = home;
            transform.rotation = Quaternion.identity;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw bobing extents
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position + bobingAxis * bobingExtents.x, transform.position + bobingAxis * bobingExtents.y);

        // Draw bobing positions on extents
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + bobingAxis * bobingExtents.x, 0.05f);
        Gizmos.DrawWireSphere(transform.position + bobingAxis * bobingExtents.y, 0.05f);

        // Draw rotation axis
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + rotationAxis.normalized);
    }
}
