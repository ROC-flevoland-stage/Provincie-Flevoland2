using UnityEngine;

public class FastTravelManager : MonoBehaviour
{
    [SerializeField] private Transform player;

    public void FastTravelTo(Transform destination)
    {
        if (player == null || destination == null) return;


        var cc = player.GetComponent<CharacterController>();

        if (cc != null) cc.enabled = false;

        player.position = destination.position;
        if (cc != null) cc.enabled = true;

        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
