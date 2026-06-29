using UnityEngine;

public class TeleportWithinScene : MonoBehaviour
{
    public Vector3 target;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller))
        {
            controller.enabled = false;
            other.transform.position = target;
            controller.enabled = true;
        }
        Debug.Log(other );
    }
}
