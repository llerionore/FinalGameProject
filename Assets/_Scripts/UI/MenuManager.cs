using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject helpPanel;

    public AudioSource audioSource;
    public AudioClip clickSound;

    private void PlayClick()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void StartGame()
    {
        PlayClick();
        SceneManager.LoadScene("Scene1");
    }

    public void QuitGame()
    {
        PlayClick();
        Application.Quit();
    }

    public void OpenHelp()
    {
        PlayClick();
        mainPanel.SetActive(false);
        helpPanel.SetActive(true);
    }

    public void ClosePanels()
    {
        PlayClick();
        mainPanel.SetActive(true);
        helpPanel.SetActive(false);
    }
}
