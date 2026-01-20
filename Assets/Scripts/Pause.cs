using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseOverlay : MonoBehaviour
{
    public GameObject pauseUI; // panel with grey overlay + text
    private bool isPaused = false;
    public void Mainmenu()
    {
        SceneManager.LoadScene(0);
    }

    void Start()
    {
        Time.timeScale = 1f;      // make sure game is running
        isPaused = false;         
        pauseUI.SetActive(false); 
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;

            if (isPaused)
                Pause();
            else
                Resume();
        }
    }

    void Pause()
    {
        Time.timeScale = 0f;
        pauseUI.SetActive(true);
    }

    void Resume()
    {
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
    }
}

