using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMinigame2 : MonoBehaviour
{
    // Scene naam van me minigame
    public string sceneName = "ShootingMinigameSem";

    // Zorg ervoor dat de collider aanstaat en dat het een trigger is!!
    void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(sceneName);
    }
}