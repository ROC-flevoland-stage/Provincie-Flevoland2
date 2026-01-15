using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private bool isHeld;
    private Rigidbody rb;
    private Collider col;

    public bool IsHeld => isHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    public void OnPickedUp(Transform holdPoint)
    {
        isHeld = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (col != null)
            col.enabled = false;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnDropped()
    {
        isHeld = false;

        transform.SetParent(null);

        if (rb != null)
            rb.isKinematic = false;

        if (col != null)
            col.enabled = true;
    }
}
