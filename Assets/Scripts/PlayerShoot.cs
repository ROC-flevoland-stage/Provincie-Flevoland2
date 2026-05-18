using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject waterBulletPrefab; // assign prefab
    public Transform shootPoint;         // assign spawn point
    public float maxAimDistance = 100f;
    public float bulletSpeed = 20f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            ShootToMouse();
    }

    void ShootToMouse()
    {
        if (waterBulletPrefab == null || shootPoint == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, maxAimDistance) ? hit.point : ray.GetPoint(maxAimDistance);
        Vector3 direction = (targetPoint - shootPoint.position).normalized;
        if (direction.sqrMagnitude <= 0f) direction = shootPoint.forward;

        var instance = Instantiate(waterBulletPrefab, shootPoint.position, Quaternion.LookRotation(direction));
        instance.transform.rotation = Quaternion.LookRotation(direction);

        // If prefab has a Rigidbody, set its velocity.
        var rb = instance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
            return;
        }

        // Otherwise try Bullet component that may move itself; if not present, add a simple mover.
        var bullet = instance.GetComponent<Bullet>();
        if (bullet != null)
        {
            // If Bullet implements direction-setting methods, try common names safely via reflection fallback:
            var m = bullet.GetType().GetMethod("Initialize");
            if (m != null) m.Invoke(bullet, new object[] { direction, bulletSpeed });
            else
            {
                // best-effort: set transform and hope Bullet reads transform.forward
                instance.transform.rotation = Quaternion.LookRotation(direction);
            }
            return;
        }

        // Final fallback: add simple mover
        var mover = instance.AddComponent<SimpleMover>();
        mover.Initialize(direction, bulletSpeed);
    }
}

public class SimpleMover : MonoBehaviour
{
    Vector3 dir = Vector3.forward;
    public float speed = 20f;
    float lifetime;
    public float maxLifetime = 5f;

    public void Initialize(Vector3 direction, float spd)
    {
        if (direction.sqrMagnitude > 0f) dir = direction.normalized;
        speed = spd > 0f ? spd : speed;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hit")) Destroy(gameObject);
    }
}
