using UnityEngine;

public class BasketballGame : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform handPoint;
    // public Transform hoopTarget;   Removed cuz we need to hit 1-10 and not just 1 hoop
    private GameObject currentBall;
    public Camera playerCamera;
    public float shootCooldown = 1.5f;
    private bool canShoot = true;

    void Start()
    {
        //SpawnBall();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && canShoot)
        {
            Shoot();
        }
    }

    void SpawnBall()
    {
        currentBall = Instantiate(ballPrefab, handPoint.position, handPoint.rotation);
    }

    void Shoot()
    {
        SpawnBall();
        if (currentBall == null) return;

        currentBall.transform.SetParent(null);

        Rigidbody rb = currentBall.GetComponent<Rigidbody>();

        rb.AddForce((playerCamera.transform.forward + Vector3.up * 0.8f) * 15f, ForceMode.Impulse);

        currentBall = null;


    }
}