using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    /// <summary>
    /// Loads a scene by its name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to be loaded</param>
    public static void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

    /// <summary>
    /// Loads a scene by its index.
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene to be loaded</param>
    public static void LoadScene(int sceneIndex) => SceneManager.LoadScene(sceneIndex);

    /// <summary>
    /// Loads a scene asynchronously by its name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to be loaded</param>
    public static void LoadSceneAsync(string sceneName) => SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

    /// <summary>
    /// Loads a scene asynchronously by its index.
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene to be loaded</param>
    public static void LoadSceneAsync(int sceneIndex) => SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

    /// <summary>
    /// Reloads the current active scene.
    /// </summary>
    public static void ReloadCurrentScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    /// <summary>
    /// Quits the game.
    /// </summary>
    public static void QuitGame() => Application.Quit();
}
