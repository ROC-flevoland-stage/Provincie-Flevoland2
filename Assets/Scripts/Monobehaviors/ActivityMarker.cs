using UnityEngine;

public class ActivityMarker : MonoBehaviour
{
    public Vector2 bobingExtents = new Vector2(-0.2f, 0.2f);
    public Vector3 bobingAxis = Vector3.up;
    public float bobingSpeed = 1f;
    public Vector2 rotationExtents = new Vector2(0f, 360f);
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 1f;

    private Vector3 home;

    void Start()
    {
        home = transform.position;
    }

    void Update()
    {
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
