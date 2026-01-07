using UnityEngine;

public class FileMovement : MonoBehaviour
{
    [Tooltip("World-space movement direction. Default = south (-Z). Use Vector3.down for screen-down (-Y).")]
    public Vector3 moveDirection = Vector3.back;

    [Tooltip("Speed in units/second.")]
    public float speed = 2f;

    Rigidbody rb;
    Vector3 dir;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        dir = moveDirection.normalized;
        if (dir == Vector3.zero) dir = Vector3.back;
    }

    void FixedUpdate()
    {
        if (speed == 0f) return;

        if (rb != null)
        {
            // Kinematic: use MovePosition. Non-kinematic: set velocity.
            if (rb.isKinematic)
                rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
            else
                rb.linearVelocity = dir * speed;
        }
        else
        {
            transform.position += dir * speed * Time.fixedDeltaTime;
        }
    }
}
