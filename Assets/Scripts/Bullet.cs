using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float maxLifetime = 5f;

    Rigidbody rb;
    Vector3 dir = Vector3.forward;
    float lifetime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Call immediately after Instantiate to set travel direction and optional speed
    public void Initialize(Vector3 direction, float speedOverride = -1f)
    {
        if (direction.sqrMagnitude > 0f)
            dir = direction.normalized;

        if (speedOverride > 0f)
            speed = speedOverride;

        transform.rotation = Quaternion.LookRotation(dir);

        if (rb != null)
            rb.linearVelocity = dir * speed;
    }

    void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (rb == null)
            transform.position += dir * speed * Time.fixedDeltaTime;
        else
            rb.linearVelocity = dir * speed; // keep velocity consistent
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hit"))
            Destroy(gameObject);
    }
}
