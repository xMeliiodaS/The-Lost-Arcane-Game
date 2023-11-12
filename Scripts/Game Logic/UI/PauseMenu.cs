using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;           // Reference to the Pause Menu
    [SerializeField] private GameObject settingUI;             // Reference to the Settings Menu


    /// <summary>
    /// Resumes the game from a paused state.
    /// </summary>
    public void Resume()
    {
        pauseMenuUI.SetActive(false);

        // Find the player object and disable pause in combat
        Transform player = FindPlayer.Instance.Player.transform;
        player.GetComponent<Combat>().SetPauseActive(false);

        // Unlock the camera rotation
        LockCameraa.GetInstance().UnlockCameraRotation();

        // Reset the time scale to normal
        Time.timeScale = 1f;
    }

    void Pause()
    {
        // Set the time scale to 0 to pause the game
        Time.timeScale = 0f;

        // Check if the settings UI is active, if so, do not proceed with pausing
        if (settingUI.activeSelf)
        {
            return;
        }
        pauseMenuUI.SetActive(true);
    }


    /// <summary>
    /// Navigate to the main lobby
    /// </summary>
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }


    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
