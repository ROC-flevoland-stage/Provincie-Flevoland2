using UnityEngine;

public class TeleportWithinScene : MonoBehaviour
{
    public Vector3 target;

    void OnTriggerEnter(Collider other)
    {
        other.transform.position = target;
    }
}
