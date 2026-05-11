using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI设置")]
    public GameObject pauseMenu;

    private bool isPaused;

    protected virtual void Awake()
    {
        Time.timeScale = 1f;
        SetPauseMenuVisible(false);
        isPaused = false;
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void ResumeGame()
    {
        SetPauseMenuVisible(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        SetPauseMenuVisible(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void GoToMainMenu()
    {
        LoadScene("MenuSecne");
    }

    public void RestartGame()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    protected void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    protected void SetPauseMenuVisible(bool visible)
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(visible);
        }
    }
}
