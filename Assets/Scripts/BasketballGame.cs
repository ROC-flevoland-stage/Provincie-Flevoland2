using UnityEngine;

public class BasketballGame : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform handPoint;
    public Transform hoopTarget;
    private GameObject currentBall;

    void Start()
    {
        //SpawnBall();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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

        Vector3 direction = (hoopTarget.position - currentBall.transform.position);

        rb.AddForce(direction.normalized * 20f + Vector3.up * 8f, ForceMode.Impulse);

        currentBall = null;

    }
}