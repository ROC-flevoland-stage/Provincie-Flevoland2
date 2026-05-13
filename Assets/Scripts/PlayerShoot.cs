using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject waterBulletPrefab; // Assign prefab
    public Transform shootPoint;         // Assign spawn point
    public Camera shootCamera;           // Sleep hier je gewenste camera in de Inspector
    public float maxAimDistance = 100f;
    public float bulletSpeed = 20f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            ShootToMouse();
    }

    void ShootToMouse()
    {
        if (waterBulletPrefab == null || shootPoint == null || shootCamera == null) return;

        // Ray vanaf de opgegeven camera naar de muispositie
        Ray ray = shootCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = ray.GetPoint(maxAimDistance);
        Vector3 direction = (targetPoint - shootPoint.position).normalized;

        // Instantieer bullet en geef velocity
        GameObject bullet = Instantiate(waterBulletPrefab, shootPoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;
    }
}
