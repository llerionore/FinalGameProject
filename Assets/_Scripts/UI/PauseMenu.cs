using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public AudioSource audioSource;
    public AudioClip clickSound;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        PlayClick();
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("Button is pressed");
    }

    public void ResumeGame()
    {
        PlayClick();
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartLevel()
    {
        PlayClick();
        Time.timeScale = 1f;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex);
    }

    public void ExitToMenu()
    {
        PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    private void PlayClick()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}
