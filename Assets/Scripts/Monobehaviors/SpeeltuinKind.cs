using UnityEngine;

public class SpeeltuinKind : MonoBehaviour
{
    public Vector3 home;
    public Transform[] hidingSpots;

    public DialogueTree normal;
    public DialogueTree hiding;

    private void Start()
    {
        home = transform.position;
        DialogueVariables.Instance.CreateVariable<bool>(
            "verstopertjeActief",
            false,
            (v) => {
                if ((bool)v)
                    Hide();
                else
                    transform.position = home;
            });
    }

    public void Talk()
    {
        if (DialogueVariables.Instance.GetVariable<bool>("verstopertjeActief"))
            DialogueManager.Instance.Startdialogue(hiding);
        else
            DialogueManager.Instance.Startdialogue(normal);
    }

    public void Hide()
    {
        if (hidingSpots.Length == 0)
            return;
        // Kies een willekeurige schuilplek
        Transform randomSpot = hidingSpots[Random.Range(0, hidingSpots.Length)];
        transform.position = randomSpot.position;
        transform.rotation = randomSpot.rotation;
    }
}
